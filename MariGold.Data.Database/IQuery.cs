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
        
        IQuery<Root> Property<Child>(Action<IPropertyTree<Root>> item);
        IQuery<Root> Property<Child>(Action<IPropertyTree<Root>> item,
            Action<CustomFieldMapper<Child>> columns);
        IQuery<Root> Property<Child>(Action<IPropertyTree<Root>> item,
           Action<QueryAction<Child>> filterFields);
        IQuery<Root> Property<Child>(Action<IPropertyTree<Root>> item,
            Action<CustomFieldMapper<Child>> columns, Action<QueryAction<Child>> filterFields);
        IQuery<Root> Property<Child>(Action<IPropertyTree<Root>> item,
            Action<CustomFieldMapper<Child>> columns, Action<QueryAction<Child>> filterFields,
            Action<QueryAction<Child>> groupFields);
        IQuery<Root> Property<Child>(Action<IPropertyTree<Root>> item,
           Action<QueryAction<Child>> filterFields, Action<QueryAction<Child>> groupFields);

        Root Get();
        IList<Root> GetList();
        IEnumerable<Root> GetEnumerable();
    }
}
