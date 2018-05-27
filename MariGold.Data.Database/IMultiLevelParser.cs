namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal interface IMultiLevelParser
    {
        Dictionary<List<PropertyInfo>, Tuple<Dictionary<string, string>, List<string>, List<string>>> PropertiesList { get; }
        List<PropertyInfo> RootGroupFields { get; }
    }
}
