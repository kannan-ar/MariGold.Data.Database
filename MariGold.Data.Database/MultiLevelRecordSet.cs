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
        private Dictionary<List<PropertyInfo>, Tuple<Dictionary<string, string>, List<string>, List<string>>> propertiesList;
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
                customMaps.Add(property.Value.GetPropertyInfo().Name, property.Key);
            }

            return customMaps;
        }
        
        private List<string> GetPropertyNames<T>(Expression<Func<T, object>>[] fields)
        {
            List<string> names = new List<string>();

            foreach (var field in fields)
            {
                PropertyInfo info = field.GetPropertyInfo();

                if (info != null)
                {
                    names.Add(info.Name);
                }
            }

            return names;
        }
        
        private void SetPropertiesList<Child>(Action<IPropertyTree<Root>> item,
            Action<CustomFieldMapper<Child>> columns = null, Action <QueryAction<Child>> filterFields = null, 
            Action<QueryAction<Child>> groupFields = null)
        {
            List<string> filterList = new List<string>();
            List<string> groupList = new List<string>();
            Dictionary<string, string> customMaps = null;

            filterFields?.Invoke(fields => { filterList = GetPropertyNames<Child>(fields); });

            groupFields?.Invoke(fields => { groupList = GetPropertyNames<Child>(fields); });

            if (columns != null)
            {
                CustomFieldMapper<Child> map = new CustomFieldMapper<Child>();
                columns(map);

                customMaps = ExtractCustomMapping(map.ExtractMap());
            }

            var prop = new PropertyTreeParser<Root>();
            item(prop);

            propertiesList.Add(prop.PropertyTree, new Tuple<Dictionary<string, string>, List<string>, List<string>>(customMaps, filterList, groupList));

        }

        internal void SetConverter(DataConverter converter)
        {
            this.converter = converter;
        }
        
        public Dictionary<List<PropertyInfo>, Tuple<Dictionary<string, string>, List<string>, List<string>>> PropertiesList
        {
            get
            {
                return propertiesList;
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
            propertiesList = new Dictionary<List<PropertyInfo>, Tuple<Dictionary<string, string>, List<string>, List<string>>>();
            rootGroupFields = new List<PropertyInfo>();
        }

        public IQuery<Root> Group(params Expression<Func<Root, object>>[] groupFields)
        {
            foreach (LambdaExpression exp in groupFields)
            {
                rootGroupFields.Add(exp.GetPropertyInfo());
            }

            return this;
        }
        
        public IQuery<Root> Property<Child>(Action<IPropertyTree<Root>> item)
        {
            SetPropertiesList<Child>(item, null, null, null);
            return this;
        }

        public IQuery<Root> Property<Child>(Action<IPropertyTree<Root>> item,
            Action<CustomFieldMapper<Child>> columns)
        {
            SetPropertiesList<Child>(item, columns, null, null);
            return this;
        }

        public IQuery<Root> Property<Child>(Action<IPropertyTree<Root>> item,
            Action<CustomFieldMapper<Child>> columns, Action<QueryAction<Child>> filterFields)
        {
            SetPropertiesList<Child>(item, columns, filterFields, null);
            return this;
        }

        public IQuery<Root> Property<Child>(Action<IPropertyTree<Root>> item,
            Action<CustomFieldMapper<Child>> columns, Action<QueryAction<Child>> filterFields,
            Action<QueryAction<Child>> groupFields)
        {
            SetPropertiesList<Child>(item, columns, filterFields, groupFields);
            return this;
        }

        public IQuery<Root> Property<Child>(Action<IPropertyTree<Root>> item,
           Action<QueryAction<Child>> filterFields, Action<QueryAction<Child>> groupFields)
        {
            SetPropertiesList<Child>(item, null, filterFields, groupFields);
            return this;
        }

        public IQuery<Root> Property<Child>(Action<IPropertyTree<Root>> item,
           Action<QueryAction<Child>> filterFields)
        {
            SetPropertiesList<Child>(item, null, filterFields, null);
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
