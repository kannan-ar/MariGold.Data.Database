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
        private readonly Dictionary<List<PropertyInfo>, Tuple<Dictionary<string, string>, List<string>, List<string>>> propertiesList;

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

        private void LoadFieldName(ILGenerator il, string fieldName, MethodInfo indexOf, LocalBuilder fieldIndex,
            Dictionary<string, string> customMapping, Label nextLocation)
        {
            string columnName;

            il.Emit(OpCodes.Ldarg_0);

            if (customMapping != null && customMapping.ContainsKey(fieldName))
            {
                il.Emit(OpCodes.Ldstr, customMapping[fieldName]);

                il.Emit(OpCodes.Call, indexOf);
                il.Emit(OpCodes.Stloc, fieldIndex);

                il.Emit(OpCodes.Ldloc, fieldIndex);
                il.Emit(OpCodes.Ldc_I4, -1);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Brtrue, nextLocation);

                return;
            }

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

        private bool IsPropertyInfoMatch(PropertyInfo info1, PropertyInfo info2)
        {
            if (info1 == null || info2 == null)
            {
                return false;
            }

            return info1.Name == info2.Name && info1.PropertyType == info2.PropertyType;
        }

        private bool HasSingleProperty(Int32 position, PropertyInfo info, out bool hasChain, out Dictionary<string, string> customMapping)
        {
            customMapping = null;
            hasChain = false;
            bool hasFound = false;

            foreach (var infoList in propertiesList)
            {
                if (infoList.Key.Count <= position)
                {
                    continue;
                }

                if (!hasFound && infoList.Key.Count == (position + 1) && IsPropertyInfoMatch(infoList.Key[position], info))
                {
                    customMapping = infoList.Value.Item1;
                    hasFound = true;
                }
                else if (infoList.Key.Count > 1 && IsPropertyInfoMatch(infoList.Key[position], info))
                {
                    hasChain = true;
                }
            }

            return hasFound;
        }

        private bool HasListProperty(Int32 position, PropertyInfo info, out bool hasChain, out List<string> filterFields, out List<string> groupFields,
            out Dictionary<string, string> customMapping)
        {
            //Tuple<List<Tuple<PropertyInfo, Dictionary<string, string>>>, List<string>, List<string>> propertyList;
            hasChain = false;
            customMapping = null;
            filterFields = null;
            groupFields = null;
            bool hasFound = false;

            foreach (var infoList in propertiesList)
            {
                if (infoList.Key.Count <= position)
                {
                    continue;
                }

                if (!hasFound && infoList.Key.Count == (position + 1) && infoList.Key[position] == info)
                {
                    customMapping = infoList.Value.Item1;
                    filterFields = infoList.Value.Item2;
                    groupFields = infoList.Value.Item3;

                    hasFound = true;
                }
                else if (infoList.Key.Count > 1 && infoList.Key[position] == info)
                {
                    hasChain = true;
                }
            }

            /*
            if (listProperties.TryGetValue(type, out propertyList))
            {
                filterFields = propertyList.Item2;
                groupFields = propertyList.Item3;

                foreach (var property in propertyList.Item1)
                {
                    if (property.Item1 == info)
                    {
                        customMapping = property.Item2;
                        return true;
                    }
                }
            }
            */
            return hasFound;
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
                LoadFieldName(il, groupField.Name, indexOf, fieldIndex, null, nextProperty);

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

        private static List<object[]> GroupResults(string[] fields, List<object[]> records, int[] filterIndices, object[] values,
            int[] groupIndices)
        {
            List<object[]> resultList = new List<object[]>();

            foreach (object[] row in records)
            {
                bool? found = null;
                bool hasItem = false;

                for (int i = 0; i < filterIndices.Length; i++)
                {
                    if (filterIndices[i] == -1)
                    {
                        break;
                    }

                    bool exists = Object.Equals(row[filterIndices[i]], values[i]);
                    found = (found ?? exists) & exists;
                }

                if (found != null && found.Value)
                {
                    foreach (object[] item in resultList)
                    {
                        for (int i = 0; i < groupIndices.Length; i++)
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

                    if (!hasItem)
                    {
                        resultList.Add(row);
                    }
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
            Type rootEntityType,
            Type entityType,
            Dictionary<string, MethodInfo> drMethods,
            MethodInfo indexOf,
            LocalBuilder fieldIndex,
            LocalBuilder isLoaded,
            LocalBuilder entity,
            LocalBuilder objArray,
            Dictionary<string, string> customMapping,
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

                    LoadFieldName(il, property.Name, indexOf, fieldIndex, customMapping, nextProperty);

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
                else if (propertyType != rootEntityType && isList)
                {
                    listProperties.Add(property);
                }
                else if (propertyType != rootEntityType && property.PropertyType.IsClass && !IsBulitinType(property.PropertyType))
                {
                    classProperties.Add(property);
                }
            }

            return new Tuple<List<PropertyInfo>, List<PropertyInfo>>(classProperties, listProperties);
        }

        private void LoadGroupResults(ILGenerator il, LocalBuilder row, List<string> filterFields, List<string> groupFields,
            MethodInfo indexOf, Type recordSetType, Type objectArrayType, Type intArrayType, Type intType)
        {
            MethodInfo groupResults = typeof(MultiLevelILConverter).GetMethod("GroupResults",
                BindingFlags.NonPublic | BindingFlags.Static);

            var filterIndices = il.DeclareLocal(intArrayType);
            var filterValueArray = il.DeclareLocal(objectArrayType);
            var groupIndices = il.DeclareLocal(intArrayType);
            var fieldIndex = il.DeclareLocal(intType);

            il.Emit(OpCodes.Ldc_I4, filterFields.Count);
            il.Emit(OpCodes.Newarr, intType);
            il.Emit(OpCodes.Stloc, filterIndices);

            il.Emit(OpCodes.Ldc_I4, filterFields.Count);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc, filterValueArray);

            il.Emit(OpCodes.Ldc_I4, groupFields.Count);
            il.Emit(OpCodes.Newarr, intType);
            il.Emit(OpCodes.Stloc, groupIndices);

            var queryFieldIndex = il.DeclareLocal(intType);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, queryFieldIndex);

            for (int i = 0; filterFields.Count > i; ++i)
            {
                Label nextValue = il.DefineLabel();

                il.Emit(OpCodes.Ldloc, filterIndices);
                il.Emit(OpCodes.Ldloc, queryFieldIndex);
                il.Emit(OpCodes.Ldc_I4, -1);
                il.Emit(OpCodes.Stelem_I4);

                LoadFieldName(il, filterFields[i], indexOf, fieldIndex, null, nextValue);

                il.Emit(OpCodes.Ldloc, filterIndices);
                il.Emit(OpCodes.Ldloc, queryFieldIndex);
                il.Emit(OpCodes.Ldloc, fieldIndex);
                il.Emit(OpCodes.Stelem_I4);

                il.Emit(OpCodes.Ldloc, filterValueArray);
                il.Emit(OpCodes.Ldloc, queryFieldIndex);
                il.Emit(OpCodes.Ldloc, row);
                il.Emit(OpCodes.Ldloc, fieldIndex);
                il.Emit(OpCodes.Ldelem_Ref);
                il.Emit(OpCodes.Stelem_Ref);

                il.Emit(OpCodes.Ldloc, queryFieldIndex);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Stloc, queryFieldIndex);

                il.MarkLabel(nextValue);
            }

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, queryFieldIndex);

            for (int i = 0; groupFields.Count > i; ++i)
            {
                Label nextValue = il.DefineLabel();

                LoadFieldName(il, groupFields[i], indexOf, fieldIndex, null, nextValue);

                il.Emit(OpCodes.Ldloc, groupIndices);
                il.Emit(OpCodes.Ldloc, queryFieldIndex);
                il.Emit(OpCodes.Ldloc, fieldIndex);
                il.Emit(OpCodes.Stelem_I4);

                il.Emit(OpCodes.Ldloc, queryFieldIndex);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Stloc, queryFieldIndex);

                il.MarkLabel(nextValue);
            }

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, filterIndices);
            il.Emit(OpCodes.Ldloc, filterValueArray);
            il.Emit(OpCodes.Ldloc, groupIndices);

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
            Int32 position,
            List<PropertyItem> entities,
            Type rootEntityType,
            LocalBuilder fieldIndex,
            LocalBuilder objArray,
            Type boolType,
            Type intType,
            Type recordSetType,
            Type objectArrayType,
            Type stringArrayType,
            Type intArrayType,
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
                Dictionary<string, string> customMapping;
                bool hasChain;

                if (HasSingleProperty(position, info, out hasChain, out customMapping))
                {
                    Label nextObject = il.DefineLabel();
                    loaded = true;
                    var isLoaded = il.DeclareLocal(boolType);
                    LocalBuilder rootEntity = LoadEntityTree(il, entities);

                    il.Emit(OpCodes.Newobj, entityType.GetConstructor(Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc, entity);

                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Stloc, isLoaded);

                    subProperties = ScanEntityProperties(il, rootEntityType, entityType, drMethods, indexOf, fieldIndex, isLoaded, entity,
                        objArray, customMapping, true);

                    il.Emit(OpCodes.Ldloc, isLoaded);
                    il.Emit(OpCodes.Brfalse, nextObject);

                    il.Emit(OpCodes.Ldloc, rootEntity);
                    il.Emit(OpCodes.Ldloc, entity);
                    il.Emit(OpCodes.Callvirt, info.GetSetMethod());

                    il.MarkLabel(nextObject);
                }
                else
                {
                    subProperties = ScanEntityProperties(il, rootEntityType, entityType, drMethods, indexOf, fieldIndex, null, null,
                        objArray, null, false);
                }

                if (hasChain)
                {
                    var latest = newEntities[newEntities.Count - 1];
                    latest.CurrentEntity = entity;
                    latest.Info = info;
                    latest.IsLoaded = loaded;

                    newEntities.Add(new PropertyItem() { RootEntity = entity });

                    RecursiveLoadProperties(il, position + 1, newEntities, entityType, fieldIndex, objArray, boolType, intType, recordSetType,
                        objectArrayType, stringArrayType, intArrayType, drMethods, indexOf, getCount, getItem, subProperties);
                }
            }

            foreach (PropertyInfo info in properties.Item2)
            {
                bool hasChain;
                List<string> filterFields;
                List<string> groupFields;
                Dictionary<string, string> customMapping;

                if (HasListProperty(position, info, out hasChain, out filterFields, out groupFields, out customMapping))
                {
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
                            intArrayType, intType);
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

                    subProperties = ScanEntityProperties(il, rootEntityType, genericType, drMethods, indexOf, fieldIndex, isLoaded, listItem,
                        objSubArray, customMapping, true);

                    il.Emit(OpCodes.Ldloc, isLoaded);
                    il.Emit(OpCodes.Brfalse, nextObject);

                    if (hasChain)
                    {
                        newEntities.Add(new PropertyItem() { RootEntity = listItem });
                        RecursiveLoadProperties(il, position + 1, newEntities, genericType, fieldIndex, objSubArray, boolType, intType, recordSetType,
                            objectArrayType, stringArrayType, intArrayType, drMethods, indexOf, getCount, getItem, subProperties);
                    }

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
            Type intArrayType = typeof(int[]);
            Type recordType = typeof(IDataRecord);
            Type boolType = typeof(bool);

            Dictionary<string, MethodInfo> drMethods = GetDrMethod(recordType);

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

            var properties = ScanEntityProperties(il, null, entityType, drMethods, indexOf, fieldIndex, isLoaded, entity, objArray, null, true);

            il.Emit(OpCodes.Ldloc, isLoaded);
            il.Emit(OpCodes.Brfalse, loopStart);

            il.Emit(OpCodes.Ldloc, entities);
            il.Emit(OpCodes.Ldloc, entity);
            il.Emit(OpCodes.Callvirt, addMethod);

            List<PropertyItem> entityList = new List<PropertyItem>();
            entityList.Add(new PropertyItem() { RootEntity = entity });

            RecursiveLoadProperties(il, 0, entityList, entityType, fieldIndex, objArray, boolType, intType, recordSetType,
                objectArrayType, stringArrayType, intArrayType, drMethods, indexOf, getCount, getItem, properties);

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
            this.propertiesList = parser.PropertiesList;
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
