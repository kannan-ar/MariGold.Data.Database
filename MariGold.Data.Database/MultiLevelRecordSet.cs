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

        private Dictionary<Type, List<PropertyInfo>> singleProperties;
        private Dictionary<Type, Tuple<List<PropertyInfo>, List<string>, List<string>>> listProperties;
        private List<PropertyInfo> rootGroupFields;
        private Type rootType;

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

        private void SetSingleProperty(Type type, LambdaExpression exp, string[] groupFields = null)
        {
            List<PropertyInfo> propertyList;
            PropertyInfo info = GetPropertyInfo(exp);

            if (singleProperties.TryGetValue(type, out propertyList))
            {
                if (!propertyList.Contains(info))
                {
                    propertyList.Add(info);
                }
            }
            else
            {
                List<PropertyInfo> infoList = new List<PropertyInfo>();
                infoList.Add(info);
                propertyList = new List<PropertyInfo>(infoList);
            }

            singleProperties[type] = propertyList;
        }

        private void SetListProperty(Type type, LambdaExpression exp, List<string> filterFields = null,
            List<string> groupFields = null)
        {
            Tuple<List<PropertyInfo>, List<string>, List<string>> propertyList;
            PropertyInfo info = GetPropertyInfo(exp);

            if (listProperties.TryGetValue(type, out propertyList))
            {
                if (!propertyList.Item1.Contains(info))
                {
                    propertyList.Item1.Add(info);
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
                List<PropertyInfo> infoList = new List<PropertyInfo>();
                List<string> filterList = new List<string>();
                List<string> groupList = new List<string>();

                infoList.Add(info);

                if (filterFields != null)
                {
                    filterList.AddRange(filterFields);
                }

                if (groupFields != null)
                {
                    groupList.AddRange(groupFields);
                }

                propertyList = new Tuple<List<PropertyInfo>, List<string>, List<string>>(infoList, filterList, groupList);
            }

            listProperties[type] = propertyList;
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

        internal void SetConverter(DataConverter converter)
        {
            this.converter = converter;
        }

        public Dictionary<Type, List<PropertyInfo>> SingleProperties
        {
            get
            {
                return singleProperties;
            }
        }

        public Dictionary<Type, Tuple<List<PropertyInfo>, List<string>, List<string>>> ListProperties
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

            singleProperties = new Dictionary<Type, List<PropertyInfo>>();
            listProperties = new Dictionary<Type, Tuple<List<PropertyInfo>, List<string>, List<string>>>();

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

        public IQuery<Root> Single<Child>(Expression<Func<Root, Child>> item)
        {
            SetSingleProperty(rootType, item);
            return this;
        }

        public IQuery<Root> Single<Parent, Child>(Expression<Func<Parent, Child>> item)
        {
            SetSingleProperty(typeof(Parent), item);
            return this;
        }

        public IQuery<Root> Many<Child>(Expression<Func<Root, IList<Child>>> list,
            Action<QueryAction<Child>> filterFields = null, Action<QueryAction<Child>> groupFields = null)
        {
            List<string> filterList = null;
            List<string> groupList = null;

            if (filterFields != null)
            {
                filterFields(fields => { filterList = GetPropertyNames<Child>(fields); });
            }

            if (groupFields != null)
            {
                groupFields(fields => { groupList = GetPropertyNames<Child>(fields); });
            }

            SetListProperty(rootType, list, filterList, groupList);

            return this;
        }

        public IQuery<Root> Many<Parent, Child>(Expression<Func<Parent, IList<Child>>> list,
            Action<QueryAction<Child>> filterFields = null)
        {
            List<string> filterList = null;

            if (filterFields != null)
            {
                filterFields(fields => { filterList = GetPropertyNames<Child>(fields); });
            }

            SetListProperty(typeof(Parent), list, filterList);

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
