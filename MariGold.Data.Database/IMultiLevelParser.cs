namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal interface IMultiLevelParser
    {
        Dictionary<Type, List<PropertyInfo>> SingleProperties { get; }
        Dictionary<Type, Tuple<List<PropertyInfo>, List<string>, List<string>>> ListProperties { get; }
        List<PropertyInfo> RootGroupFields { get; }
    }
}
