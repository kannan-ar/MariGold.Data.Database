namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public sealed class PropertyTreeParser<T> : IPropertyTree<T>
    {
        private List<PropertyInfo> infoList;

        internal List<PropertyInfo> PropertyTree
        {
            get
            {
                return infoList;
            }
        }

        public PropertyTreeParser()
        {
            infoList = new List<PropertyInfo>();
        }

        public PropertyTreeParser(List<PropertyInfo> infoList)
        {
            this.infoList = infoList;
        }

        public IPropertyTree<K> Of<K>(Expression<Func<T, K>> exp)
        {
            infoList.Add(exp.GetPropertyInfo());
            var obj = new PropertyTreeParser<K>(infoList);
            return obj;
        }

        public IPropertyTree<K> Of<K>(Expression<Func<T, IList<K>>> exp)
        {
            infoList.Add(exp.GetPropertyInfo());
            var obj = new PropertyTreeParser<K>(infoList);
            return obj;
        }

        public IPropertyTree<K> Of<K>(Expression<Func<T, List<K>>> exp)
        {
            infoList.Add(exp.GetPropertyInfo());
            var obj = new PropertyTreeParser<K>(infoList);
            return obj;
        }
    }
}
