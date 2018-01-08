namespace MariGold.Data
{
    using System.Collections.Generic;

    public interface ITypedRecordSet<T>
         where T : class, new()
    {
        T Get();
        IList<T> GetList();
        IEnumerable<T> GetEnumerable();
    }
}
