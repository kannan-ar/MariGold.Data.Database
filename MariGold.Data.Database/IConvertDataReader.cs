namespace MariGold.Data
{
    using System;
    using System.Data;
    using System.Collections.Generic;

    public interface IConvertDataReader<T>
    {
        T Get(IDataReader dr);
        IList<T> GetList(IDataReader dr);
        IEnumerable<T> GetEnumerable(IDataReader dr);
    }
}
