namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IPropertyTree<T>
    {
        IPropertyTree<K> Of<K>(Expression<Func<T, K>> exp);
        IPropertyTree<K> Of<K>(Expression<Func<T, IList<K>>> exp);
        IPropertyTree<K> Of<K>(Expression<Func<T, List<K>>> exp);
    }
}
