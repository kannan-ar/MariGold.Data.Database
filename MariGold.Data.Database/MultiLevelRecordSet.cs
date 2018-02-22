namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Data;

    public sealed class MultiLevelRecordSet<Root> : IQuery<Root>, IMultiLevelParser
        where Root : class, new()
    {
        private readonly IDataReader dr;

        private DataConverter converter;

        private Dictionary<Type, List<Tuple<PropertyInfo, Dictionary<string, string>>>> singleProperties;
        private Dictionary<Type, Tuple<List<Tuple<PropertyInfo, Dictionary<string, string>>>, List<string>, List<string>>> listProperties;
        private List<PropertyInfo> rootGroupFields;
        private Type rootType;

        private Dictionary<string, string> ExtractCustomMapping(Dictionary<string, LambdaExpression> properties)
        {
            if (properties == null)
            {
                return null;
            }

            Dictionary<string, string> customMaps = new Dictionary<string, string>();

            foreach (var property in properties)
            {
                customMaps.Add(GetPropertyInfo(property.Value).Name, property.Key);
            }

            return customMaps;
        }

        private PropertyInfo GetPropertyInfo(LambdaExpression exp)
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

            return ex.Member as PropertyInfo;
        }

        private List<string> GetPropertyNames<T>(Expression<Func<T, object>>[] fields)
        {
            List<string> names = new List<string>();

            foreach (var field in fields)
            {
                PropertyInfo info = GetPropertyInfo(field);

                if (info != null)
                {
                    names.Add(info.Name);
                }
            }

            return names;
        }

        private void SetSingleProperty(Type type, LambdaExpression exp, Dictionary<string, string> customMaps = null)
        {
            List<Tuple<PropertyInfo, Dictionary<string, string>>> propertyList;
            PropertyInfo info = GetPropertyInfo(exp);

            if (singleProperties.TryGetValue(type, out propertyList))
            {
                bool found = false;

                foreach (var property in propertyList)
                {
                    if (property.Item1 == info)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    propertyList.Add(new Tuple<PropertyInfo, Dictionary<string, string>>(info, customMaps));
                }
            }
            else
            {
                propertyList = new List<Tuple<PropertyInfo, Dictionary<string, string>>>();
                propertyList.Add(new Tuple<PropertyInfo, Dictionary<string, string>>(info, customMaps));
            }

            singleProperties[type] = propertyList;
        }

        private void SetListProperty(Type type, LambdaExpression exp, List<string> filterFields = null,
            List<string> groupFields = null, Dictionary<string, string> customMaps = null)
        {
            Tuple<List<Tuple<PropertyInfo, Dictionary<string, string>>>, List<string>, List<string>> propertyList;
            PropertyInfo info = GetPropertyInfo(exp);

            if (listProperties.TryGetValue(type, out propertyList))
            {
                bool found = false;

                foreach (var property in propertyList.Item1)
                {
                    if (property.Item1 == info)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    propertyList.Item1.Add(new Tuple<PropertyInfo, Dictionary<string, string>>(info, customMaps));
                }

                if (filterFields != null && filterFields.Count > 0)
                {
                    foreach (string filterField in filterFields)
                    {
                        if (propertyList.Item2.Contains(filterField))
                        {
                            propertyList.Item2.Add(filterField);
                        }
                    }
                }

                if (groupFields != null && groupFields.Count > 0)
                {
                    foreach (string groupField in groupFields)
                    {
                        if (propertyList.Item3.Contains(groupField))
                        {
                            propertyList.Item3.Add(groupField);
                        }
                    }
                }
            }
            else
            {
                List<Tuple<PropertyInfo, Dictionary<string, string>>> infoList = 
                    new List<Tuple<PropertyInfo, Dictionary<string, string>>>();
                List<string> filterList = new List<string>();
                List<string> groupList = new List<string>();

                infoList.Add(new Tuple<PropertyInfo, Dictionary<string, string>>(info, customMaps));

                if (filterFields != null)
                {
                    filterList.AddRange(filterFields);
                }

                if (groupFields != null)
                {
                    groupList.AddRange(groupFields);
                }

                propertyList = new Tuple<List<Tuple<PropertyInfo, Dictionary<string, string>>>, 
                    List<string>, List<string>>(infoList, filterList, groupList);
            }

            listProperties[type] = propertyList;
        }

        internal void SetConverter(DataConverter converter)
        {
            this.converter = converter;
        }

        public Dictionary<Type, List<Tuple<PropertyInfo, Dictionary<string, string>>>> SingleProperties
        {
            get
            {
                return singleProperties;
            }
        }

        public Dictionary<Type, Tuple<List<Tuple<PropertyInfo, Dictionary<string, string>>>, List<string>, List<string>>> ListProperties
        {
            get
            {
                return listProperties;
            }
        }

        public List<PropertyInfo> RootGroupFields
        {
            get
            {
                return rootGroupFields;
            }
        }

        public MultiLevelRecordSet(IDataReader dr)
        {
            this.dr = dr;

            rootType = typeof(Root);

            singleProperties = new Dictionary<Type, List<Tuple<PropertyInfo, Dictionary<string, string>>>>();
            listProperties = new Dictionary<Type, Tuple<List<Tuple<PropertyInfo, Dictionary<string, string>>>, List<string>, List<string>>>();

            rootGroupFields = new List<PropertyInfo>();
        }

        public IQuery<Root> Group(params Expression<Func<Root, object>>[] groupFields)
        {
            foreach (LambdaExpression exp in groupFields)
            {
                rootGroupFields.Add(GetPropertyInfo(exp));
            }

            return this;
        }

        public IQuery<Root> Single<Child>(Expression<Func<Root, Child>> item, Action<CustomFieldMapper<Child>> columns = null)
        {
            Dictionary<string, string> customMaps = null;

            if (columns != null)
            {
                CustomFieldMapper<Child> map = new CustomFieldMapper<Child>();
                columns(map);

                customMaps = ExtractCustomMapping(map.ExtractMap());
            }

            SetSingleProperty(rootType, item, customMaps);

            return this;
        }

        public IQuery<Root> Single<Parent, Child>(Expression<Func<Parent, Child>> item, Action<CustomFieldMapper<Child>> columns = null)
        {
            Dictionary<string, string> customMaps = null;

            if (columns != null)
            {
                CustomFieldMapper<Child> map = new CustomFieldMapper<Child>();
                columns(map);

                customMaps = ExtractCustomMapping(map.ExtractMap());
            }

            SetSingleProperty(typeof(Parent), item, customMaps);

            return this;
        }

        public IQuery<Root> Many<Child>(Expression<Func<Root, IList<Child>>> list,
            Action<QueryAction<Child>> filterFields = null, Action<QueryAction<Child>> groupFields = null,
            Action<CustomFieldMapper<Child>> columns = null)
        {
            List<string> filterList = null;
            List<string> groupList = null;
            Dictionary<string, string> customMaps = null;

            if (filterFields != null)
            {
                filterFields(fields => { filterList = GetPropertyNames<Child>(fields); });
            }

            if (groupFields != null)
            {
                groupFields(fields => { groupList = GetPropertyNames<Child>(fields); });
            }

            if (columns != null)
            {
                CustomFieldMapper<Child> map = new CustomFieldMapper<Child>();
                columns(map);

                customMaps = ExtractCustomMapping(map.ExtractMap());
            }

            SetListProperty(rootType, list, filterList, groupList, customMaps);

            return this;
        }

        public IQuery<Root> Many<Parent, Child>(Expression<Func<Parent, IList<Child>>> list,
            Action<QueryAction<Child>> filterFields = null, Action<CustomFieldMapper<Child>> columns = null)
        {
            List<string> filterList = null;
            Dictionary<string, string> customMaps = null;

            if (filterFields != null)
            {
                filterFields(fields => { filterList = GetPropertyNames<Child>(fields); });
            }

            if (columns != null)
            {
                CustomFieldMapper<Child> map = new CustomFieldMapper<Child>();
                columns(map);

                customMaps = ExtractCustomMapping(map.ExtractMap());
            }

            SetListProperty(typeof(Parent), list, filterList, null, customMaps);

            return this;
        }

        public Root Get()
        {
            return converter.Get<Root>(dr);
        }

        public IList<Root> GetList()
        {
            return converter.GetList<Root>(dr);
        }

        public IEnumerable<Root> GetEnumerable()
        {
            return converter.GetEnumerable<Root>(dr);
        }
    }
}
