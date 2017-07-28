namespace MariGold.Data
{
    using System.Collections.Generic;

    public interface IRecordSet
    {
        T Get<T>() where T : class, new();
        IList<T> GetList<T>() where T : class, new();
        IEnumerable<T> GetEnumerable<T>() where T : class, new();

        dynamic Get();
        IList<dynamic> GetList();
        IEnumerable<dynamic> GetEnumerable();

        object GetScalar();
    }
}
