namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public delegate void QueryAction<T>(params Expression<Func<T, object>>[] fields);

    public interface IQuery<Root>
        where Root : class, new()
    {
        IQuery<Root> Group(params Expression<Func<Root, object>>[] groupFields);
        IQuery<Root> Single<Child>(Expression<Func<Root, Child>> item);
        IQuery<Root> Single<Parent, Child>(Expression<Func<Parent, Child>> item);
        IQuery<Root> Many<Child>(Expression<Func<Root, IList<Child>>> list,
            Action<QueryAction<Child>> filterFields = null, Action<QueryAction<Child>> groupFields = null);
        IQuery<Root> Many<Parent, Child>(Expression<Func<Parent, IList<Child>>> list,
            Action<QueryAction<Child>> filterFields = null);

        Root Get();
        IList<Root> GetList();
        IEnumerable<Root> GetEnumerable();
    }
}
