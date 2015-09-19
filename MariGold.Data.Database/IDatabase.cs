namespace MariGold.Data
{
    using System;
    using System.Data;
    using System.Collections.Generic;

    public interface IDatabase
    {
        ConnectionState State { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }

        IDbConnection Open();

        IDbTransaction BeginTransaction();

        void Commit();

        void Rollback();

        IDataReader GetDataReader(string sql, CommandType commandType = CommandType.Text, 
            IDictionary<string, object> parameters = null);

        IDataReader GetDataReader(Query query);

        int Execute(string sql, CommandType commandType = CommandType.Text,
            IDictionary<string, object> parameters = null);

        int Execute(Query query);

        object GetScalar(string sql, CommandType commandType = CommandType.Text,
            IDictionary<string, object> parameters = null);

        object GetScalar(Query query);

        IDbCommand CreateCommand(Query query);

        void Close();
    }
}
