namespace MariGold.Data
{
    using System;
    using System.Data;
    using System.Collections.Generic;

    /// <summary>
    /// Performs all database operations using an instance of IDbConnection.
    /// </summary>
    public sealed class Database : IDatabase
    {
        private readonly IDbConnection conn;
        private IDbTransaction tran;

        private IDataReader GetDataReader(IDbCommand cmd)
        {
            using (cmd)
            {
                if (tran != null)
                {
                    cmd.Transaction = tran;
                }

                return cmd.ExecuteReader();
            }
        }

        private int Execute(IDbCommand cmd)
        {
            using (cmd)
            {
                if (tran != null)
                {
                    cmd.Transaction = tran;
                }

                return cmd.ExecuteNonQuery();
            }
        }

        private object GetScalar(IDbCommand cmd)
        {
            using (cmd)
            {
                if (tran != null)
                {
                    cmd.Transaction = tran;
                }

                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Returns the status of internal IDbConnection.
        /// </summary>
        public ConnectionState State
        {
            get
            {
                if (conn == null)
                {
                    return ConnectionState.Closed;
                }
                else
                {
                    return conn.State;
                }
            }
        }

        /// <summary>
        /// Returns the internal IDbConnection.
        /// </summary>
        public IDbConnection Connection
        {
            get
            {
                return conn;
            }
        }

        /// <summary>
        /// Returns the IDbTransaction object. Transaction can be created using BeginTransaction function.
        /// </summary>
        public IDbTransaction Transaction
        {
            get
            {
                return tran;
            }
        }

        /// <summary>
        /// Initializes a new instance of MariGold.Data.Database class
        /// </summary>
        /// <param name="conn">An implemention of IDbConnection. Throws ArgumentNullException if null.</param>
        public Database(IDbConnection conn)
        {
            if (conn == null)
            {
                throw new ArgumentNullException("conn");
            }

            this.conn = conn;
        }

        /// <summary>
        /// Opens the internal IDbConnection.
        /// </summary>
        /// <returns></returns>
        public IDbConnection Open()
        {
            conn.Open();

            return conn;
        }

        /// <summary>
        /// Creates are returns a new IDbTransaction using the internal IDbConnection.
        /// Throws exception if there is an active transaction exists. This transaction will be used in subsequent database operations. 
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            if (tran != null)
            {
                throw new InvalidOperationException("There is a pending transaction exists");
            }

            tran = conn.BeginTransaction();

            return tran;
        }

        /// <summary>
        /// Commit the transaction if created. Throws exception if the transaction is null.
        /// </summary>
        public void Commit()
        {
            if (tran == null)
            {
                throw new InvalidOperationException("There is no active transaction");
            }

            try
            {
                tran.Commit();
            }
            finally
            {
                tran = null;
            }
        }

        /// <summary>
        /// Rollback the transaction if created. Throws exception if the transaction is null.
        /// </summary>
        public void Rollback()
        {
            if (tran == null)
            {
                throw new InvalidOperationException("There is no active transaction");
            }

            try
            {
                tran.Rollback();
            }
            finally
            {
                tran = null;
            }
        }

        /// <summary>
        /// Creates a data reader with the given sql string from underlying IDbConnection.
        /// </summary>
        /// <param name="sql">A valid sql string to be executed</param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IDataReader GetDataReader(
            string sql,
            CommandType commandType,
            IDictionary<string, object> parameters)
        {
            return GetDataReader(Query.GetCommand(conn, sql, commandType, parameters));
        }

        public IDataReader GetDataReader(
           string sql,
           CommandType commandType)
        {
            return GetDataReader(Query.GetCommand(conn, sql, commandType));
        }

        public IDataReader GetDataReader(
            string sql,
            IDictionary<string, object> parameters)
        {
            return GetDataReader(Query.GetCommand(conn, sql, parameters));
        }

        public IDataReader GetDataReader(string sql)
        {
            return GetDataReader(Query.GetCommand(conn, sql));
        }

        /// <summary>
        /// Creates a data reader with the given sql string from underlying IDbConnection.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IDataReader GetDataReader(Query query)
        {
            return GetDataReader(query.GetCommand(conn));
        }

        /// <summary>
        /// Executes the given sql string using the underlying IDbConnection.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int Execute(
            string sql,
            CommandType commandType,
            IDictionary<string, object> parameters)
        {
            return Execute(Query.GetCommand(conn, sql, commandType, parameters));
        }

        public int Execute(
            string sql,
            CommandType commandType)
        {
            return Execute(Query.GetCommand(conn, sql, commandType));
        }

        public int Execute(
            string sql,
            IDictionary<string, object> parameters)
        {
            return Execute(Query.GetCommand(conn, sql, parameters));
        }

        public int Execute(string sql)
        {
            return Execute(Query.GetCommand(conn, sql));
        }

        /// <summary>
        /// Executes the given sql string using the underlying IDbConnection.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int Execute(Query query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            return Execute(query.GetCommand(conn));
        }

        /// <summary>
        /// Get a scalar value with the given sql string and underlying IDbConnection.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object GetScalar(
            string sql,
            CommandType commandType,
            IDictionary<string, object> parameters)
        {
            return GetScalar(Query.GetCommand(conn, sql, commandType, parameters));
        }

        public object GetScalar(
            string sql,
            CommandType commandType)
        {
            return GetScalar(Query.GetCommand(conn, sql, commandType));
        }

        public object GetScalar(
            string sql,
            IDictionary<string, object> parameters)
        {
            return GetScalar(Query.GetCommand(conn, sql, parameters));
        }

        public object GetScalar(string sql)
        {
            return GetScalar(Query.GetCommand(conn, sql));
        }

        /// <summary>
        /// Get a scalar value with the given sql string and underlying IDbConnection.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object GetScalar(Query query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            return GetScalar(query.GetCommand(conn));
        }

        /// <summary>
        /// Close the underlying IDbConnection.
        /// </summary>
        public void Close()
        {
            if (conn == null)
            {
                return;
            }

            if (conn.State == ConnectionState.Closed)
            {
                return;
            }

            conn.Close();

            conn.Dispose();
        }

        /// <summary>
        /// Returns the connection string of underlying IDbConnection if it not null.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (conn == null)
            {
                return this.ToString();
            }

            return conn.ConnectionString;
        }
    }
}
