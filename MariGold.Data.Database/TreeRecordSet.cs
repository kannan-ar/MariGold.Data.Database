namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Data;

    public sealed class TreeRecordSet<T> : ITypedRecordSet<T>
        where T : class, new()
    {
        private readonly IDataReader dr;
        private readonly DataConverter converter;
        private readonly Expression<Func<T, object>>[] properties;

        public TreeRecordSet(IDataReader dr, DataConverter converter, Expression<Func<T, object>>[] properties)
        {
            this.dr = dr;
            this.converter = converter;
            this.properties = properties;
        }

        public T Get()
        {
           return converter.Get<T>(dr, properties);
        }

        public IList<T> GetList()
        {
            return converter.GetList<T>(dr, properties);
        }

        public IEnumerable<T> GetEnumerable()
        {
            return converter.GetList<T>(dr, properties);
        }
    }
}
