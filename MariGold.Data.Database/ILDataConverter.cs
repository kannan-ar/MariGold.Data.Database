namespace MariGold.Data
{
    using System;
    using System.Data;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;

    /// <summary>
    /// Creates dynamic methods to iterate through data reader and generate CLR objects using IL Emit.
    /// </summary>
    public sealed class ILDataConverter : DataConverter
    {
        private static ConcurrentDictionary<Type, Delegate> methods;

        private Dictionary<string, MethodInfo> GetDrMethod(Type recordType)
        {
            Dictionary<string, MethodInfo> list = new Dictionary<string, MethodInfo>();

            list.Add("Int32", recordType.GetMethod("GetInt32"));
            list.Add("String", recordType.GetMethod("GetString"));
            list.Add("Boolean", recordType.GetMethod("GetBoolean"));
            list.Add("Char", recordType.GetMethod("GetChar"));
            list.Add("DateTime", recordType.GetMethod("GetDateTime"));
            list.Add("Decimal", recordType.GetMethod("GetDecimal"));
            list.Add("Double", recordType.GetMethod("GetDouble"));
            list.Add("Single", recordType.GetMethod("GetFloat"));
            list.Add("Int16", recordType.GetMethod("GetInt16"));
            list.Add("Int64", recordType.GetMethod("GetInt64"));

            return list;
        }

        private void InitProp(
            ILGenerator il,
            Dictionary<string, PropertyTree> dict,
            Dictionary<LocalBuilder, Type> objList,
            List<string> endItems,
            LocalBuilder obj = null)
        {
            foreach (var item in dict)
            {
                PropertyTree tree = item.Value;

                LocalBuilder property = il.DeclareLocal(item.Value.PropertyType);

                if (obj == null)
                {
                    il.Emit(OpCodes.Ldarg_0);
                }
                else
                {
                    il.Emit(OpCodes.Ldloc, obj);
                }

                il.Emit(OpCodes.Newobj, item.Value.PropertyType.GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc, property);
                il.Emit(OpCodes.Ldloc, property);

                il.Emit(OpCodes.Callvirt, item.Value.SetMethod);

                if (endItems.Contains(item.Key))
                {
                    objList.Add(property, item.Value.PropertyType);
                }

                if (tree.Items.Count > 0)
                {
                    InitProp(il, tree.Items, objList, endItems, property);
                }
            }
        }

        private Action<T, IDataRecord> GetPropertyTreeFunc<T>(Dictionary<string, PropertyTree> properties, List<string> candidateProperties)
        {
            Delegate del;
            Dictionary<LocalBuilder, Type> objList = new Dictionary<LocalBuilder, Type>();
            Type recordType = typeof(IDataRecord);
            Type intType = typeof(Int32);
            Type stringType = typeof(String);
            Dictionary<string, MethodInfo> drMethods = GetDrMethod(recordType);

            MethodInfo fieldCount = recordType.GetProperty("FieldCount").GetGetMethod();
            MethodInfo isDBNull = recordType.GetMethod("IsDBNull");
            MethodInfo getName = recordType.GetMethod("GetName");
            MethodInfo compare = stringType.GetMethod("Compare", new Type[] {
                    stringType,
                    stringType,
                    typeof(bool)
                });

            var method = new DynamicMethod("", null, new Type[] { typeof(T), typeof(IDataRecord) }, true);
            ILGenerator il = method.GetILGenerator();

            Label loopStart = il.DefineLabel();
            Label end = il.DefineLabel();
            var count = il.DeclareLocal(intType);
            var index = il.DeclareLocal(intType);
            var fieldName = il.DeclareLocal(stringType);

            InitProp(il, properties, objList, candidateProperties);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, fieldCount);
            il.Emit(OpCodes.Stloc, count);

            il.Emit(OpCodes.Ldc_I4, -1);
            il.Emit(OpCodes.Stloc, index);

            il.MarkLabel(loopStart);

            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Ldc_I4, 1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, index);

            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Ldloc, count);
            il.Emit(OpCodes.Bge, end);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Callvirt, isDBNull);
            il.Emit(OpCodes.Brtrue, loopStart);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Callvirt, getName);
            il.Emit(OpCodes.Stloc, fieldName);

            foreach (var objItem in objList)
            {
                foreach (PropertyInfo property in objItem.Value.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    Label bottomSection = il.DefineLabel();
                    Label loadSection = il.DefineLabel();

                    bool isNullable;
                    Type propType = GetTypeName(property.PropertyType, out isNullable);

                    if (!drMethods.ContainsKey(propType.Name))
                    {
                        continue;
                    }

                    il.Emit(OpCodes.Ldloc, fieldName);
                    il.Emit(OpCodes.Ldstr, property.Name);
                    il.Emit(OpCodes.Ldc_I4, 1);
                    il.Emit(OpCodes.Call, compare);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Brtrue, loadSection);

                    if (!Config.UnderscoreToPascalCase)
                    {
                        il.Emit(OpCodes.Br_S, bottomSection);
                    }

                    il.Emit(OpCodes.Ldloc, fieldName);
                    il.Emit(OpCodes.Ldstr, ConvertCamelStringToUnderscore(property.Name));

                    il.Emit(OpCodes.Ldc_I4, 1);
                    il.Emit(OpCodes.Call, compare);

                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Brfalse, bottomSection);

                    MethodInfo drMethod;
                    drMethods.TryGetValue(propType.Name, out drMethod);

                    il.MarkLabel(loadSection);

                    il.Emit(OpCodes.Ldloc, objItem.Key);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldloc, index);
                    il.Emit(OpCodes.Callvirt, drMethod);
                    
                    if (isNullable)
                    {
                        il.Emit(OpCodes.Newobj, property.PropertyType.GetConstructor(new[] { propType }));
                    }
                    
                    il.Emit(OpCodes.Callvirt, property.GetSetMethod());
                    il.Emit(OpCodes.Br, loopStart);

                    il.MarkLabel(bottomSection);
                }
            }

            il.Emit(OpCodes.Br, loopStart);

            il.MarkLabel(end);

            il.Emit(OpCodes.Ret);

            del = method.CreateDelegate(typeof(Action<T, IDataRecord>));
            return (Action<T, IDataRecord>)del;
        }

        private Action<T, IDataRecord> GetPropertyTreeAction<T>(Expression<Func<T, object>>[] properties)
        {
            Action<T, IDataRecord> act = null;

            if (properties != null)
            {
                List<string> candidate;
                Dictionary<string, PropertyTree> propertyTree = ParseTree<T>(properties, out candidate);
                act = GetPropertyTreeFunc<T>(propertyTree, candidate);
            }

            return act;
        }

        private MemberExpression GetExpression(LambdaExpression exp)
        {
            if (exp == null)
            {
                return null;
            }

            MemberExpression ex = exp.Body as MemberExpression;

            if (ex == null)
            {
                UnaryExpression uex = exp.Body as UnaryExpression;

                if (uex != null)
                {
                    ex = uex.Operand as MemberExpression;
                }
            }

            return ex;
        }

        private Dictionary<string, PropertyTree> ParseTree<T>(Expression<Func<T, object>>[] properties,
            out List<string> candidateProperties)
        {
            Dictionary<string, PropertyTree> dict = new Dictionary<string, PropertyTree>();
            candidateProperties = new List<string>();

            foreach (var exp in properties)
            {
                MemberExpression ex = GetExpression(exp);

                if (ex != null)
                {
                    List<Tuple<string, PropertyTree>> firstParse = new List<Tuple<string, PropertyTree>>();

                    while (ex != null)
                    {
                        PropertyTree tree = new PropertyTree();
                        PropertyInfo info = ex.Member as PropertyInfo;

                        tree.PropertyType = info.PropertyType;
                        tree.SetMethod = info.GetSetMethod();

                        firstParse.Add(new Tuple<string, PropertyTree>(info.Name, tree));

                        ex = ex.Expression as MemberExpression;
                    }

                    Dictionary<string, PropertyTree> temp = dict;

                    for (int i = firstParse.Count - 1; i >= 0; --i)
                    {
                        PropertyTree tree;

                        if (i == 0)
                        {
                            candidateProperties.Add(firstParse[i].Item1);
                        }

                        if (!temp.TryGetValue(firstParse[i].Item1, out tree))
                        {
                            tree = new PropertyTree();

                            tree.PropertyType = firstParse[i].Item2.PropertyType;
                            tree.SetMethod = firstParse[i].Item2.SetMethod;

                            temp.Add(firstParse[i].Item1, tree);
                        }

                        temp = tree.Items;
                    }
                }
            }

            return dict;
        }

        private Type GetTypeName(Type type, out bool isNullable)
        {
            isNullable = false;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                isNullable = true;
                return Nullable.GetUnderlyingType(type);
            }
            else
            {
                return type;
            }
        }

        static ILDataConverter()
        {
            methods = new ConcurrentDictionary<Type, Delegate>();
        }

        private Func<IDataReader, T> GetReaderFunc<T>(IDataReader dr)
            where T : class, new()
        {
            Type type = typeof(T);
            Delegate del;

            if (!methods.TryGetValue(type, out del))
            {
                bool shouldDispose;
                Type readerType = typeof(IDataReader);
                Type recordType = typeof(IDataRecord);
                Type stringType = typeof(String);
                Type intType = typeof(Int32);
                Dictionary<string, MethodInfo> drMethods = GetDrMethod(recordType);
                Dictionary<string, string> columns = EntityManager<T>.Get(out shouldDispose);

                MethodInfo getName = recordType.GetMethod("GetName");
                MethodInfo compare = stringType.GetMethod("Compare", new Type[] {
                    stringType,
                    stringType,
                    typeof(bool)
                });
                MethodInfo fieldCount = recordType.GetProperty("FieldCount").GetGetMethod();
                MethodInfo isDBNull = recordType.GetMethod("IsDBNull");

                //New dynamic method with IDataReader type parameter and T type return value.
                var method = new DynamicMethod("", type, new[] { readerType }, true);
                var il = method.GetILGenerator();

                var entity = il.DeclareLocal(type);
                var dataRecord = il.DeclareLocal(recordType);
                var fieldName = il.DeclareLocal(stringType);
                var count = il.DeclareLocal(intType);
                var index = il.DeclareLocal(intType);

                Label end = il.DefineLabel();
                Label loopStart = il.DefineLabel();

                //Create new T type object and store at location 'entity'
                il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc, entity);

                //Convert input parameter of IDataReader to IDataRecord and store at location 'dataRecord'
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, recordType);
                il.Emit(OpCodes.Stloc, dataRecord);

                //Get field count of data reader input and store it in count variable
                il.Emit(OpCodes.Ldloc, dataRecord);
                il.Emit(OpCodes.Callvirt, fieldCount);
                il.Emit(OpCodes.Stloc, count);

                //Initally load -1 to the index variable
                il.Emit(OpCodes.Ldc_I4, -1);
                il.Emit(OpCodes.Stloc, index);

                //Starts loop through the data reader columns
                il.MarkLabel(loopStart);

                //Add +1 to the index variable
                il.Emit(OpCodes.Ldloc, index);
                il.Emit(OpCodes.Ldc_I4, 1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Stloc, index);

                //Go to the lable 'end' if the index varible is greather than count variable
                il.Emit(OpCodes.Ldloc, index);
                il.Emit(OpCodes.Ldloc, count);
                il.Emit(OpCodes.Bge, end);

                //Check the value in current index is DBNull
                il.Emit(OpCodes.Ldloc, dataRecord);
                il.Emit(OpCodes.Ldloc, index);
                il.Emit(OpCodes.Callvirt, isDBNull);

                //Go to next iteration if the value is DbNull.
                il.Emit(OpCodes.Brtrue, loopStart);

                //Get the field name of data reader at the index of 'index' variable 
                il.Emit(OpCodes.Ldloc, dataRecord);
                il.Emit(OpCodes.Ldloc, index);
                il.Emit(OpCodes.Callvirt, getName);
                il.Emit(OpCodes.Stloc, fieldName);

                //Stores the jump lable and property info of each property in the type 'T'
                Dictionary<Label, Tuple<PropertyInfo, Type, bool>> jumpTable = new Dictionary<Label, Tuple<PropertyInfo, Type, bool>>();

                //Scan through the 'T' type's properties and emit il to compare property names with data reader column name
                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    bool isNullable;
                    Type propType = GetTypeName(property.PropertyType, out isNullable);

                    //Skip the iteration if the property type is not convertible
                    if (!drMethods.ContainsKey(propType.Name))
                    {
                        continue;
                    }

                    Label label = il.DefineLabel();
                    jumpTable.Add(label, new Tuple<PropertyInfo, Type, bool>(property, propType, isNullable));

                    //Compare the data reader column name and property name
                    il.Emit(OpCodes.Ldloc, fieldName);

                    string columnName;
                    //Try to get custom mapped column name. If not avail, use the property name to compare with data reader field name.
                    if (columns != null && columns.TryGetValue(property.Name, out columnName))
                    {
                        il.Emit(OpCodes.Ldstr, columnName);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldstr, property.Name);
                    }

                    il.Emit(OpCodes.Ldc_I4, 1);
                    il.Emit(OpCodes.Call, compare);

                    //If both values are equal, go to the label where sets the property with the data reader value.
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Brtrue, label);

                    if (Config.UnderscoreToPascalCase)
                    {
                        il.Emit(OpCodes.Ldloc, fieldName);
                        il.Emit(OpCodes.Ldstr, ConvertCamelStringToUnderscore(property.Name));

                        il.Emit(OpCodes.Ldc_I4, 1);
                        il.Emit(OpCodes.Call, compare);

                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ceq);
                        il.Emit(OpCodes.Brtrue, label);
                    }
                }

                //It acts like default case. The control reaches here because no property names matches with data reader column name.
                //So goes to next loop iteration
                il.Emit(OpCodes.Br, loopStart);

                //Scan through the dictionary and emit instructions to set property with data reader values
                foreach (var jump in jumpTable)
                {
                    il.MarkLabel(jump.Key);

                    //Gets the exact method based on property type to fetch value from data reader
                    // MethodInfo drMethod = GetDrMethod(recordType, jump.Value.PropertyType.Name);
                    MethodInfo drMethod;

                    drMethods.TryGetValue(jump.Value.Item2.Name, out drMethod);

                    MethodInfo propMethod = jump.Value.Item1.GetSetMethod();

                    //fetch value from data reader and assigned it to the property
                    il.Emit(OpCodes.Ldloc, entity);
                    il.Emit(OpCodes.Ldloc, dataRecord);
                    il.Emit(OpCodes.Ldloc, index);
                    il.Emit(OpCodes.Callvirt, drMethod);

                    //If the property is a nullable type, add extra logic to convert the value into nullable type.
                    if (jump.Value.Item3)
                    {
                        il.Emit(OpCodes.Newobj, jump.Value.Item1.PropertyType.GetConstructor(new[] { jump.Value.Item2 }));
                    }

                    il.Emit(OpCodes.Callvirt, propMethod);
                    //Go to the next loop iteration
                    il.Emit(OpCodes.Br, loopStart);
                }

                il.MarkLabel(end);
                //Loads and returns the created entity.
                il.Emit(OpCodes.Ldloc, entity);
                il.Emit(OpCodes.Ret);

                del = method.CreateDelegate(typeof(Func<IDataReader, T>));

                if (!shouldDispose)
                {
                    methods.TryAdd(type, del);
                }
            }

            return (Func<IDataReader, T>)del;
        }

        public override void ClearType<T>()
        {
            Delegate value;
            methods.TryRemove(typeof(T), out value);
        }

        public override void ClearAllTypes()
        {
            methods.Clear();
        }

        /// <summary>
        /// Creates and returns the generic type T object and initilizes its public properties with the values of matching fields from given IDataReader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public override T Get<T>(IDataReader dr, Expression<Func<T, object>>[] properties = null)
        {
            T item = default(T);

            if (dr.Read())
            {
                Func<IDataReader, T> func = GetReaderFunc<T>(dr);

                item = func(dr);

                GetPropertyTreeAction(properties)?.Invoke(item, dr);
            }

            return item;
        }

        /// <summary>
        /// Creates and returns a list of generic type T from given IDataReader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public override IList<T> GetList<T>(IDataReader dr, Expression<Func<T, object>>[] properties = null)
        {
            IList<T> list = new List<T>();

            if (dr.Read())
            {
                Action<T, IDataRecord> act = GetPropertyTreeAction<T>(properties);
                Func<IDataReader, T> func = GetReaderFunc<T>(dr);

                do
                {
                    T obj = func(dr);
                    act?.Invoke(obj, dr);
                    list.Add(obj);
                }
                while (dr.Read());
            }

            return list;
        }

        /// <summary>
        /// Creates and returns an IEnumerable type T from given IDataReader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public override IEnumerable<T> GetEnumerable<T>(IDataReader dr, Expression<Func<T, object>>[] properties = null)
        {
            if (dr.Read())
            {
                Action<T, IDataRecord> act = GetPropertyTreeAction<T>(properties);
                Func<IDataReader, T> func = GetReaderFunc<T>(dr);

                do
                {
                    T obj = func(dr);
                    act?.Invoke(obj, dr);
                    yield return obj;

                }
                while (dr.Read());
            }
        }
    }
}
