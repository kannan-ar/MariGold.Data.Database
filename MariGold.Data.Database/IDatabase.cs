namespace MariGold.Data
{
    using System;
    using System.Data;
    using System.Collections.Generic;

    public interface IDatabase
    {
        ConnectionState State { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; set; }

        IDbConnection Open();

        IDbTransaction BeginTransaction();

        void Commit();

        void Rollback();

        IDataReader GetDataReader(string sql, CommandType commandType, object parameters);

        IDataReader GetDataReader(string sql, CommandType commandType);

        IDataReader GetDataReader(string sql, object parameters);

        IDataReader GetDataReader(string sql);

        IDataReader GetDataReader(Query query);

        int Execute(string sql, CommandType commandType, object parameters);

        int Execute(string sql, CommandType commandType);

        int Execute(string sql, object parameters);

        int Execute(string sql);

        int Execute(Query query);

        object GetScalar(string sql, CommandType commandType, object parameters);

        object GetScalar(string sql, CommandType commandType);

        object GetScalar(string sql, object parameters);

        object GetScalar(string sql);

        object GetScalar(Query query);

        void Close();
    }
}
