namespace MariGold.Data
{
    using System;
    using System.Dynamic;
    using System.Collections.Generic;

    public sealed class DbObject : DynamicObject
    {
        private readonly Dictionary<string, object> fields;

        internal void Add(string field, object value)
        {
            fields.Add(field, value);
        }

        public DbObject()
        {
            fields = new Dictionary<string, object>();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return fields.TryGetValue(binder.Name, out result);
        }
    }
}
