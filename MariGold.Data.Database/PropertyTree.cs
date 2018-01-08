namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class PropertyTree
    {
        public PropertyTree()
        {
            Items = new Dictionary<string, PropertyTree>();
        }

        public MethodInfo SetMethod { get; set; }
        public Type PropertyType { get; set; }
        public Dictionary<string, PropertyTree> Items { get; set; }
    }
}
