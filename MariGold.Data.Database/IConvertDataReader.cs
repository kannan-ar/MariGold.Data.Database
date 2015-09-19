namespace MariGold.Data
{
    using System;
    using System.Data;
    using System.Collections.Generic;

    public interface IConvertDataReader
    {
        T Get<T>(IDataReader dr);
        List<T> GetList<T>(IDataReader dr);
        IEnumerable<T> GetEnumerable<T>(IDataReader dr);
    }
}
