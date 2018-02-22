namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class CustomFieldMapper<T>
    {
        private Dictionary<string, LambdaExpression> properties;

        public CustomFieldMapper()
        {
            properties = new Dictionary<string, LambdaExpression>();
        }

        public CustomFieldMapper<T> Map(string field, Expression<Func<T, object>> property)
        {
            properties.Add(field, property);

            return this;
        }

        internal Dictionary<string, LambdaExpression> ExtractMap()
        {
            return properties;
        }
    }
}
