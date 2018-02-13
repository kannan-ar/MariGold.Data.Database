namespace MariGold.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    internal sealed class MultiLevelILConverter : DataConverter
    {
        private readonly List<PropertyInfo> rootGroups;
        private readonly Dictionary<Type, List<PropertyInfo>> singleProperties;
        private readonly Dictionary<Type, Tuple<List<PropertyInfo>, List<string>, List<string>>> listProperties;

        private class PropertyItem
        {
            public LocalBuilder RootEntity { get; set; }
            public LocalBuilder CurrentEntity { get; set; }
            public PropertyInfo Info { get; set; }
            public bool IsLoaded { get; set; }
            public string[] GroupFields { get; set; }

            public PropertyItem Clone()
            {
                return new PropertyItem()
                {
                    RootEntity = this.RootEntity,
                    CurrentEntity = this.CurrentEntity,
                    Info = this.Info,
                    IsLoaded = this.IsLoaded,
                    GroupFields = this.GroupFields
                };
            }
        }

        Dictionary<string, string> columns = new Dictionary<string, string>();

        private bool IsBulitinType(Type type)
        {
            return (type == typeof(object) || Type.GetTypeCode(type) != TypeCode.Object);
        }
        /*
        private string GetFieldName(string propertyName)
        {
            string columnName;

            if (columns.TryGetValue(propertyName, out columnName))
            {
                propertyName = columnName;
            }
            else if (Config.UnderscoreToPascalCase)
            {
                propertyName = ConvertCamelStringToUnderscore(propertyName);
            }

            return propertyName;
        }
        */
        private void LoadFieldName(ILGenerator il, string fieldName, MethodInfo indexOf, LocalBuilder fieldIndex, Label nextLocation)
        {
            string columnName;

            il.Emit(OpCodes.Ldarg_0);

            if (columns.TryGetValue(fieldName, out columnName))
            {
                il.Emit(OpCodes.Ldstr, columnName);
            }
            else
            {
                il.Emit(OpCodes.Ldstr, fieldName);
            }

            il.Emit(OpCodes.Call, indexOf);
            il.Emit(OpCodes.Stloc, fieldIndex);

            if (Config.UnderscoreToPascalCase)
            {
                Label skipNextCheck = il.DefineLabel();

                il.Emit(OpCodes.Ldloc, fieldIndex);
                il.Emit(OpCodes.Ldc_I4, -1);
                il.Emit(OpCodes.Cgt);
                il.Emit(OpCodes.Brtrue, skipNextCheck);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldstr, ConvertCamelStringToUnderscore(fieldName));

                il.Emit(OpCodes.Call, indexOf);
                il.Emit(OpCodes.Stloc, fieldIndex);

                il.Emit(OpCodes.Ldloc, fieldIndex);
                il.Emit(OpCodes.Ldc_I4, -1);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Brtrue, nextLocation);

                il.MarkLabel(skipNextCheck);
            }
            else
            {
                il.Emit(OpCodes.Ldloc, fieldIndex);
                il.Emit(OpCodes.Ldc_I4, -1);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Brtrue, nextLocation);
            }
        }
        /*
        private List<string> GetFieldNames(List<string> fields)
        {
            for (int i = 0; fields.Count > i; i++)
            {
                fields[i] = GetFieldName(fields[i]);
            }

            return fields;
        }
        */
        private Type GetTypeName(Type type, out bool isNullable, out bool isList)
        {
            isNullable = false;
            isList = false;

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    isNullable = true;
                    type = Nullable.GetUnderlyingType(type);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(type) && type.GenericTypeArguments.Length > 0)
                {
                    isList = true;
                    type = type.GenericTypeArguments[0];
                }
            }

            return type;
        }

        private bool HasSingleProperty(Type type, PropertyInfo info)
        {
            List<PropertyInfo> propertyList;

            if (singleProperties.TryGetValue(type, out propertyList))
            {
                return propertyList.Contains(info);
            }

            return false;
        }

        private bool HasListProperty(Type type, PropertyInfo info, out List<string> filterFields, out List<string> groupFields)
        {
            Tuple<List<PropertyInfo>, List<string>, List<string>> propertyList;
            filterFields = new List<string>();
            groupFields = new List<string>();

            if (listProperties.TryGetValue(type, out propertyList))
            {
                filterFields = propertyList.Item2;
                groupFields = propertyList.Item3;

                return propertyList.Item1.Contains(info);
            }

            return false;
        }

        private void SkipIfGroupExists<T>(ILGenerator il, LocalBuilder entities, LocalBuilder objArray, Label rootLoopStart,
            Type intType, Type objectArrayType, Type entityListType, Type entityType, Type boolType, MethodInfo indexOf)
        {
            MethodInfo getCount = entityListType.GetProperty("Count").GetGetMethod();
            MethodInfo getItem = entityListType.GetProperty("Item").GetGetMethod();

            var loopStart = il.DefineLabel();
            var end = il.DefineLabel();
            var nextProperty = il.DefineLabel();

            var recordCount = il.DeclareLocal(intType);
            var index = il.DeclareLocal(intType);
            var entity = il.DeclareLocal(entityType);
            var fieldIndex = il.DeclareLocal(intType);
            var isLoaded = il.DeclareLocal(boolType);

            il.Emit(OpCodes.Ldloc, entities);
            il.Emit(OpCodes.Callvirt, getCount);
            il.Emit(OpCodes.Stloc, recordCount);

            LoopStart(il, loopStart, index, recordCount, end);

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, isLoaded);

            il.Emit(OpCodes.Ldloc, entities);
            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Callvirt, getItem);
            il.Emit(OpCodes.Stloc, entity);

            foreach (var groupField in rootGroups)
            {
                LoadFieldName(il, groupField.Name, indexOf, fieldIndex, nextProperty);
                /*
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldstr, GetFieldName(groupField.Name));
                il.Emit(OpCodes.Call, indexOf);
                il.Emit(OpCodes.Stloc, fieldIndex);

                il.Emit(OpCodes.Ldloc, fieldIndex);
                il.Emit(OpCodes.Ldc_I4, -1);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Brtrue, nextProperty);
                */
                il.Emit(OpCodes.Ldloc, entity);
                il.Emit(OpCodes.Callvirt, groupField.GetGetMethod());

                il.Emit(OpCodes.Ldloc, objArray);
                il.Emit(OpCodes.Ldloc, fieldIndex);
                il.Emit(OpCodes.Ldelem_Ref);
                il.Emit(OpCodes.Unbox_Any, groupField.PropertyType);

                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Brfalse, loopStart);

                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Stloc, isLoaded);

                il.MarkLabel(nextProperty);
            }

            il.Emit(OpCodes.Ldloc, isLoaded);
            il.Emit(OpCodes.Brtrue, rootLoopStart);

            il.Emit(OpCodes.Br, loopStart);
            il.MarkLabel(end);
        }

        private static List<object[]> GroupResults(string[] fields, List<object[]> records, string[] filterFields, object[] values,
            string[] groupFields)
        {
            List<object[]> resultList = new List<object[]>();

            List<int> filterIndices = new List<int>();
            List<int> groupIndices = new List<int>();

            foreach (string filterField in filterFields)
            {
                filterIndices.Add(Array.IndexOf<string>(fields, filterField));
            }

            foreach (string groupField in groupFields)
            {
                groupIndices.Add(Array.IndexOf<string>(fields, groupField));
            }

            foreach (object[] row in records)
            {
                bool? found = null;
                bool hasItem = false;

                for (int i = 0; i < filterIndices.Count; i++)
                {
                    if (filterIndices[i] != -1)
                    {
                        bool exists = Object.Equals(row[filterIndices[i]], values[i]);
                        found = (found ?? exists) & exists;
                    }
                }

                foreach (object[] item in resultList)
                {
                    for (int i = 0; i < groupIndices.Count; i++)
                    {
                        hasItem = Object.Equals(row[groupIndices[i]], item[groupIndices[i]]);

                        if (!hasItem)
                        {
                            break;
                        }
                    }

                    if (hasItem)
                    {
                        break;
                    }
                }

                if (found != null && found.Value && !hasItem)
                {
                    resultList.Add(row);
                }
            }

            return resultList;
        }

        private static int IndexOf(string[] array, string value)
        {
            for (int i = 0; array.Length > i; ++i)
            {
                if (string.Compare(array[i], value, true) == 0)
                {
                    return i;
                }
            }

            return -1;
        }
        private void LoopStart(ILGenerator il, Label loopStart, LocalBuilder index, LocalBuilder recordCount, Label end)
        {
            il.Emit(OpCodes.Ldc_I4, -1);
            il.Emit(OpCodes.Stloc, index);

            il.MarkLabel(loopStart);

            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Ldc_I4, 1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, index);

            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Ldloc, recordCount);
            il.Emit(OpCodes.Bge, end);
        }

        private LocalBuilder LoadEntityTree(ILGenerator il, List<PropertyItem> entities)
        {
            LocalBuilder entity = null;

            if (entities.Count > 0 && !entities[entities.Count - 1].IsLoaded)
            {
                int index = 0;

                for (int i = entities.Count - 1; i >= 0; --i)
                {
                    var item = entities[i];

                    if (item.CurrentEntity == null)
                    {
                        continue;
                    }

                    if (item.IsLoaded)
                    {
                        index = i + 1;
                        break;
                    }
                }

                for (; entities.Count > index; ++index)
                {
                    var editItem = entities[index];

                    if (editItem.Info != null && !editItem.IsLoaded)
                    {
                        if (editItem.CurrentEntity == null)
                        {
                            editItem.CurrentEntity = il.DeclareLocal(editItem.Info.PropertyType);
                        }

                        il.Emit(OpCodes.Newobj, editItem.Info.PropertyType.GetConstructor(Type.EmptyTypes));
                        il.Emit(OpCodes.Stloc, editItem.CurrentEntity);

                        il.Emit(OpCodes.Ldloc, editItem.RootEntity);
                        il.Emit(OpCodes.Ldloc, editItem.CurrentEntity);
                        il.Emit(OpCodes.Callvirt, editItem.Info.GetSetMethod());

                        editItem.IsLoaded = true;
                    }
                }
            }

            if (entities.Count > 0)
            {
                entity = entities[entities.Count - 1].RootEntity;
            }

            return entity;
        }

        private Tuple<List<PropertyInfo>, List<PropertyInfo>> ScanEntityProperties(
            ILGenerator il,
            Type entityType,
            Dictionary<string, MethodInfo> drMethods,
            MethodInfo indexOf,
            LocalBuilder fieldIndex,
            LocalBuilder isLoaded,
            LocalBuilder entity,
            LocalBuilder objArray,
            bool shouldLoad)
        {
            List<PropertyInfo> listProperties = new List<PropertyInfo>();
            List<PropertyInfo> classProperties = new List<PropertyInfo>();

            foreach (PropertyInfo property in entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                bool isNullable;
                bool isList;

                Type propertyType = GetTypeName(property.PropertyType, out isNullable, out isList);

                if (shouldLoad && !isList && drMethods.ContainsKey(propertyType.Name))
                {
                    Label nextProperty = il.DefineLabel();

                    LoadFieldName(il, property.Name, indexOf, fieldIndex, nextProperty);
                    /*
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldstr, GetFieldName(property.Name));
                    il.Emit(OpCodes.Call, indexOf);
                    il.Emit(OpCodes.Stloc, fieldIndex);

                    il.Emit(OpCodes.Ldloc, fieldIndex);
                    il.Emit(OpCodes.Ldc_I4, -1);
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Brtrue, nextProperty);
                    */
                    il.Emit(OpCodes.Ldloc, objArray);
                    il.Emit(OpCodes.Ldloc, fieldIndex);
                    il.Emit(OpCodes.Ldelem_Ref);
                    il.Emit(OpCodes.Isinst, typeof(System.DBNull));
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Cgt_Un);
                    il.Emit(OpCodes.Brtrue_S, nextProperty);

                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Stloc, isLoaded);

                    il.Emit(OpCodes.Ldloc, entity);
                    il.Emit(OpCodes.Ldloc, objArray);
                    il.Emit(OpCodes.Ldloc, fieldIndex);
                    il.Emit(OpCodes.Ldelem_Ref);
                    il.Emit(OpCodes.Unbox_Any, propertyType);

                    if (isNullable)
                    {
                        il.Emit(OpCodes.Newobj, property.PropertyType.GetConstructor(new[] { propertyType }));
                    }

                    il.Emit(OpCodes.Callvirt, property.GetSetMethod());

                    il.MarkLabel(nextProperty);
                }
                else if (isList)
                {
                    listProperties.Add(property);
                }
                else if (property.PropertyType.IsClass && !IsBulitinType(property.PropertyType))
                {
                    classProperties.Add(property);
                }
            }

            return new Tuple<List<PropertyInfo>, List<PropertyInfo>>(classProperties, listProperties);
        }

        private void LoadGroupResults(ILGenerator il, LocalBuilder row, List<string> filterFields, List<string> groupFields,
            MethodInfo indexOf, Type recordSetType, Type objectArrayType, Type stringArrayType, Type intType)
        {
            MethodInfo groupResults = typeof(MultiLevelILConverter).GetMethod("GroupResults",
                BindingFlags.NonPublic | BindingFlags.Static);

            var filterFieldArray = il.DeclareLocal(stringArrayType);
            var filterValueArray = il.DeclareLocal(objectArrayType);
            var groupFieldArray = il.DeclareLocal(stringArrayType);
            var fieldIndex = il.DeclareLocal(intType);

            il.Emit(OpCodes.Ldc_I4, filterFields.Count);
            il.Emit(OpCodes.Newarr, typeof(string));
            il.Emit(OpCodes.Stloc, filterFieldArray);

            il.Emit(OpCodes.Ldc_I4, filterFields.Count);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc, filterValueArray);

            il.Emit(OpCodes.Ldc_I4, groupFields.Count);
            il.Emit(OpCodes.Newarr, typeof(string));
            il.Emit(OpCodes.Stloc, groupFieldArray);

            for (int i = 0; filterFields.Count > i; ++i)
            {
                Label nextValue = il.DefineLabel();

                il.Emit(OpCodes.Ldloc, filterFieldArray);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldstr, filterFields[i]);
                il.Emit(OpCodes.Stelem_Ref);

                LoadFieldName(il, filterFields[i], indexOf, fieldIndex, nextValue);
                /*
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldstr, GetFieldName(filterFields[i]));
                il.Emit(OpCodes.Call, indexOf);
                il.Emit(OpCodes.Stloc, fieldIndex);

                il.Emit(OpCodes.Ldloc, fieldIndex);
                il.Emit(OpCodes.Ldc_I4, -1);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Brtrue, nextValue);
                */
                il.Emit(OpCodes.Ldloc, filterValueArray);
                il.Emit(OpCodes.Ldc_I4, i);

                il.Emit(OpCodes.Ldloc, row);
                il.Emit(OpCodes.Ldloc, fieldIndex);
                il.Emit(OpCodes.Ldelem_Ref);

                il.Emit(OpCodes.Stelem_Ref);

                il.MarkLabel(nextValue);
            }

            for (int i = 0; groupFields.Count > i; ++i)
            {
                il.Emit(OpCodes.Ldloc, groupFieldArray);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldstr, groupFields[i]);
                il.Emit(OpCodes.Stelem_Ref);
            }

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, filterFieldArray);
            il.Emit(OpCodes.Ldloc, filterValueArray);
            il.Emit(OpCodes.Ldloc, groupFieldArray);

            il.Emit(OpCodes.Call, groupResults);
        }

        private List<PropertyItem> CloneProperties(List<PropertyItem> list)
        {
            List<PropertyItem> newList = new List<PropertyItem>();

            foreach (PropertyItem item in list)
            {
                newList.Add(item.Clone());
            }

            return newList;
        }

        private void RecursiveLoadProperties(
            ILGenerator il,
            List<PropertyItem> entities,
            Type rootEntityType,
            LocalBuilder fieldIndex,
            LocalBuilder objArray,
            Type boolType,
            Type intType,
            Type recordSetType,
            Type objectArrayType,
            Type stringArrayType,
            Dictionary<string, MethodInfo> drMethods,
            MethodInfo indexOf,
            MethodInfo getCount,
            MethodInfo getItem,
            Tuple<List<PropertyInfo>, List<PropertyInfo>> properties)
        {
            foreach (PropertyInfo info in properties.Item1)
            {
                Tuple<List<PropertyInfo>, List<PropertyInfo>> subProperties = null;
                Type entityType = info.PropertyType;
                var newEntities = CloneProperties(entities);
                bool loaded = false;
                var entity = il.DeclareLocal(entityType);

                if (HasSingleProperty(rootEntityType, info))
                {
                    Label nextObject = il.DefineLabel();
                    loaded = true;

                    var isLoaded = il.DeclareLocal(boolType);

                    LocalBuilder rootEntity = LoadEntityTree(il, entities);

                    il.Emit(OpCodes.Newobj, entityType.GetConstructor(Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc, entity);

                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Stloc, isLoaded);

                    subProperties = ScanEntityProperties(il, entityType, drMethods, indexOf, fieldIndex, isLoaded, entity,
                        objArray, true);

                    il.Emit(OpCodes.Ldloc, isLoaded);
                    il.Emit(OpCodes.Brfalse, nextObject);

                    il.Emit(OpCodes.Ldloc, rootEntity);
                    il.Emit(OpCodes.Ldloc, entity);
                    il.Emit(OpCodes.Callvirt, info.GetSetMethod());

                    il.MarkLabel(nextObject);
                }
                else
                {
                    subProperties = ScanEntityProperties(il, entityType, drMethods, indexOf, fieldIndex, null, null,
                        objArray, false);
                }

                var latest = newEntities[newEntities.Count - 1];
                latest.CurrentEntity = entity;
                latest.Info = info;
                latest.IsLoaded = loaded;

                newEntities.Add(new PropertyItem() { RootEntity = entity });

                RecursiveLoadProperties(il, newEntities, entityType, fieldIndex, objArray, boolType, intType, recordSetType,
                    objectArrayType, stringArrayType, drMethods, indexOf, getCount, getItem, subProperties);
            }

            foreach (PropertyInfo info in properties.Item2)
            {
                List<string> filterFields;
                List<string> groupFields;

                if (HasListProperty(rootEntityType, info, out filterFields, out groupFields))
                {
                    // filterFields = GetFieldNames(filterFields);
                    // groupFields = GetFieldNames(groupFields);

                    Tuple<List<PropertyInfo>, List<PropertyInfo>> subProperties = null;
                    Type propType = info.PropertyType;
                    Type genericType = propType.GenericTypeArguments[0];
                    Type listType = typeof(List<>).MakeGenericType(genericType);
                    MethodInfo addMethod = listType.GetMethod("Add");
                    var newEntities = entities.ToList();

                    var isLoaded = il.DeclareLocal(boolType);
                    var objSubArray = il.DeclareLocal(objectArrayType);
                    Label nextObject = il.DefineLabel();
                    Label loopStart = il.DefineLabel();
                    Label end = il.DefineLabel();
                    LocalBuilder rootEntity = LoadEntityTree(il, entities);

                    LocalBuilder listVar = il.DeclareLocal(listType);
                    var count = il.DeclareLocal(intType);
                    var index = il.DeclareLocal(intType);

                    il.Emit(OpCodes.Newobj, listType.GetConstructor(Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc, listVar);

                    LocalBuilder recordSet = il.DeclareLocal(recordSetType);

                    if (filterFields.Count > 0)
                    {
                        LoadGroupResults(il, objArray, filterFields, groupFields, indexOf, recordSetType, objectArrayType,
                            stringArrayType, intType);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldarg_1);
                    }

                    il.Emit(OpCodes.Stloc, recordSet);

                    il.Emit(OpCodes.Ldloc, recordSet);
                    il.Emit(OpCodes.Callvirt, getCount);
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

                    il.Emit(OpCodes.Ldloc, recordSet);
                    il.Emit(OpCodes.Ldloc, index);
                    il.Emit(OpCodes.Callvirt, getItem);
                    il.Emit(OpCodes.Stloc, objSubArray);

                    LocalBuilder listItem = il.DeclareLocal(genericType);

                    il.Emit(OpCodes.Newobj, genericType.GetConstructor(Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc, listItem);

                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Stloc, isLoaded);

                    subProperties = ScanEntityProperties(il, genericType, drMethods, indexOf, fieldIndex, isLoaded, listItem,
                        objSubArray, true);

                    il.Emit(OpCodes.Ldloc, isLoaded);
                    il.Emit(OpCodes.Brfalse, nextObject);

                    newEntities.Add(new PropertyItem() { RootEntity = listItem });
                    RecursiveLoadProperties(il, newEntities, genericType, fieldIndex, objSubArray, boolType, intType, recordSetType,
                        objectArrayType, stringArrayType, drMethods, indexOf, getCount, getItem, subProperties);

                    il.Emit(OpCodes.Ldloc, listVar);
                    il.Emit(OpCodes.Ldloc, listItem);
                    il.Emit(OpCodes.Callvirt, addMethod);

                    il.MarkLabel(nextObject);

                    il.Emit(OpCodes.Br, loopStart);
                    il.MarkLabel(end);

                    il.Emit(OpCodes.Ldloc, rootEntity);
                    il.Emit(OpCodes.Ldloc, listVar);
                    il.Emit(OpCodes.Callvirt, info.GetSetMethod());
                }
            }
        }

        private Func<string[], List<object[]>, int, List<T>> CreateFunc<T>(string[] fields)
        {
            Type entityType = typeof(T);
            Type entityListType = typeof(List<T>);
            Type recordSetType = typeof(List<object[]>);
            Type objectArrayType = typeof(object[]);
            Type stringType = typeof(String);
            Type intType = typeof(Int32);
            Type stringArrayType = typeof(string[]);
            Type recordType = typeof(IDataRecord);
            Type boolType = typeof(bool);

            Dictionary<string, MethodInfo> drMethods = GetDrMethod(recordType);

            //MethodInfo indexOf = typeof(Array).GetMethod("IndexOf", new Type[] { typeof(string[]), typeof(string) });
            MethodInfo indexOf = typeof(MultiLevelILConverter).GetMethod("IndexOf", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo getCount = recordSetType.GetProperty("Count").GetGetMethod();
            MethodInfo getItem = recordSetType.GetProperty("Item").GetGetMethod();
            MethodInfo addMethod = entityListType.GetMethod("Add");

            var method = new DynamicMethod("", entityListType, new[] { stringArrayType, recordSetType, intType }, true);
            var il = method.GetILGenerator();

            var entities = il.DeclareLocal(entityListType);
            var fieldIndex = il.DeclareLocal(intType);
            var recordCount = il.DeclareLocal(intType);
            var objArray = il.DeclareLocal(objectArrayType);
            var index = il.DeclareLocal(intType);
            var isLoaded = il.DeclareLocal(boolType);
            var entity = il.DeclareLocal(entityType);

            var newArraySection = il.DefineLabel();
            var loopStart = il.DefineLabel();
            var end = il.DefineLabel();

            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stloc, recordCount);

            il.Emit(OpCodes.Ldloc, recordCount);
            il.Emit(OpCodes.Ldc_I4, -1);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Brfalse, newArraySection);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, getCount);
            il.Emit(OpCodes.Stloc, recordCount);

            il.MarkLabel(newArraySection);

            il.Emit(OpCodes.Newobj, entityListType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, entities);

            LoopStart(il, loopStart, index, recordCount, end);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Callvirt, getItem);
            il.Emit(OpCodes.Stloc, objArray);

            if (rootGroups.Count > 0)
            {
                SkipIfGroupExists<T>(il, entities, objArray, loopStart, intType, objectArrayType, entityListType,
                    entityType, boolType, indexOf);
            }

            il.Emit(OpCodes.Newobj, entityType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, entity);

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, isLoaded);

            var properties = ScanEntityProperties(il, entityType, drMethods, indexOf, fieldIndex, isLoaded, entity, objArray, true);

            il.Emit(OpCodes.Ldloc, isLoaded);
            il.Emit(OpCodes.Brfalse, loopStart);

            il.Emit(OpCodes.Ldloc, entities);
            il.Emit(OpCodes.Ldloc, entity);
            il.Emit(OpCodes.Callvirt, addMethod);

            List<PropertyItem> entityList = new List<PropertyItem>();
            entityList.Add(new PropertyItem() { RootEntity = entity });

            RecursiveLoadProperties(il, entityList, entityType, fieldIndex, objArray, boolType, intType, recordSetType,
                objectArrayType, stringArrayType, drMethods, indexOf, getCount, getItem, properties);

            il.Emit(OpCodes.Br, loopStart);
            il.MarkLabel(end);

            il.Emit(OpCodes.Ldloc, entities);
            il.Emit(OpCodes.Ret);

            return (Func<string[], List<object[]>, int, List<T>>)method.CreateDelegate(
                typeof(Func<string[], List<object[]>, int, List<T>>));
        }

        public MultiLevelILConverter(IMultiLevelParser parser)
        {
            this.rootGroups = parser.RootGroupFields;
            this.singleProperties = parser.SingleProperties;
            this.listProperties = parser.ListProperties;
        }

        private List<object[]> GetRecordSet(IDataReader dr, out string[] fields)
        {
            List<object[]> recordSet = new List<object[]>();
            int fieldCount = dr.FieldCount;
            fields = new string[fieldCount];

            if (dr.Read())
            {
                for (int i = 0; i < fieldCount; ++i)
                {
                    fields[i] = dr.GetName(i);
                }

                do
                {
                    object[] row = new object[fieldCount];

                    for (int i = 0; i < fieldCount; ++i)
                    {
                        row[i] = dr.GetValue(i);
                    }

                    recordSet.Add(row);

                } while (dr.Read());
            }

            return recordSet;
        }

        public override void ClearAllTypes()
        {
            throw new NotImplementedException();
        }

        public override void ClearType<T>()
        {
            throw new NotImplementedException();
        }

        public override T Get<T>(IDataReader dr)
        {
            T item = default(T);

            string[] fields;
            List<object[]> results = GetRecordSet(dr, out fields);

            var func = CreateFunc<T>(fields);

            var items = func(fields, results, 1);

            if (items != null && items.Count > 0)
            {
                item = items[0];
            }

            return item;
        }

        public override IEnumerable<T> GetEnumerable<T>(IDataReader dr)
        {
            string[] fields;
            List<object[]> results = GetRecordSet(dr, out fields);

            var func = CreateFunc<T>(fields);

            var items = func(fields, results, -1);

            foreach (var item in items)
            {
                yield return item;
            }
        }

        public override IList<T> GetList<T>(IDataReader dr)
        {
            string[] fields;
            List<object[]> results = GetRecordSet(dr, out fields);

            var func = CreateFunc<T>(fields);

            return func(fields, results, -1);
        }
    }
}
