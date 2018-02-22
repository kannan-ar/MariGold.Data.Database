namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal interface IMultiLevelParser
    {
        Dictionary<Type, List<Tuple<PropertyInfo, Dictionary<string, string>>>> SingleProperties { get; }
        Dictionary<Type, Tuple<List<Tuple<PropertyInfo, Dictionary<string, string>>>, List<string>, List<string>>> ListProperties { get; }
        List<PropertyInfo> RootGroupFields { get; }
    }
}
