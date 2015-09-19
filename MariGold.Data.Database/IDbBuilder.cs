namespace MariGold.Data
{
    using System;

    public interface IDbBuilder
    {
        IDatabase GetConnection();
        IConvertDataReader GetConverter();
    }
}
