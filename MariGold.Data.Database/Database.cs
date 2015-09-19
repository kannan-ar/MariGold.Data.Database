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
        private readonly IDbConnection _conn;
        private IDbTransaction _tran;

        /// <summary>
        /// Returns the status of internal IDbConnection.
        /// </summary>
        public ConnectionState State
        {
            get
            {
                if (_conn == null)
                {
                    return ConnectionState.Closed;
                }
                else
                {
                    return _conn.State;
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
                return _conn;
            }
        }

        /// <summary>
        /// Returns the IDbTransaction object. Transaction can be created using BeginTransaction function.
        /// </summary>
        public IDbTransaction Transaction
        {
            get
            {
                return _tran;
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
        	
            _conn = conn;
        }
        
        /// <summary>
        /// Opens the internal IDbConnection.
        /// </summary>
        /// <returns></returns>
        public IDbConnection Open()
        {
            _conn.Open();

            return _conn;
        }

        /// <summary>
        /// Creates are returns a new IDbTransaction using the internal IDbConnection.
        /// Throws exception if there is an active transaction exists. This transaction will be used in subsequent database operations. 
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            if (_tran != null)
            {
                throw new ApplicationException("There is a pending transaction exists");
            }

            _tran = _conn.BeginTransaction();

            return _tran;
        }

        /// <summary>
        /// Commit the transaction if created. Throws exception if the transaction is null.
        /// </summary>
        public void Commit()
        {
            if (_tran == null)
            {
                throw new ApplicationException("There is no active transaction");
            }

            try
            {
                _tran.Commit();
            }
            finally
            {
                _tran = null;
            }
        }

        /// <summary>
        /// Rollback the transaction if created. Throws exception if the transaction is null.
        /// </summary>
        public void Rollback()
        {
            if (_tran == null)
            {
                throw new ApplicationException("There is no active transaction");
            }

            try
            {
                _tran.Rollback();
            }
            finally
            {
                _tran = null;
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
            CommandType commandType = CommandType.Text,
            IDictionary<string, object> parameters = null)
        {
            return GetDataReader(new Query(sql, commandType, parameters));
        }

        /// <summary>
        /// Creates a data reader with the given sql string from underlying IDbConnection.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IDataReader GetDataReader(Query query)
        {
            using (IDbCommand cmd = CreateCommand(query))
            {
                return cmd.ExecuteReader();
            }
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
            CommandType commandType = CommandType.Text, 
            IDictionary<string, object> parameters = null)
        {
            return Execute(new Query(sql, commandType, parameters));
        }

        /// <summary>
        /// Executes the given sql string using the underlying IDbConnection.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int Execute(Query query)
        {
            using (IDbCommand cmd = CreateCommand(query))
            {
                return cmd.ExecuteNonQuery();
            }
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
            CommandType commandType = CommandType.Text,
            IDictionary<string, object> parameters = null)
        {
            return GetScalar(new Query(sql, commandType, parameters));
        }

        /// <summary>
        /// Get a scalar value with the given sql string and underlying IDbConnection.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object GetScalar(Query query)
        {
            using (IDbCommand cmd = CreateCommand(query))
            {
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Creates an IDbCommand using the properties of Query class. Throws exception if the connection is null or not opened.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IDbCommand CreateCommand(Query query)
        {
            if (_conn == null)
            {
                throw new ApplicationException("Database connection is not established yet");
            }

            if (_conn.State != ConnectionState.Open)
            {
                throw new ApplicationException("Database connection is not established yet");
            }

            IDbCommand cmd = query.GetCommand(_conn);

            if (_tran != null)
            {
                cmd.Transaction = _tran;
            }

            return cmd;
        }

        /// <summary>
        /// Close the underlying IDbConnection.
        /// </summary>
        public void Close()
        {
            if (_conn == null)
            {
                return;
            }

            if (_conn.State == ConnectionState.Closed)
            {
                return;
            }

            _conn.Close();

            _conn.Dispose();
        }

        /// <summary>
        /// Returns the connection string of underlying IDbConnection if it not null.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_conn == null)
            {
                return this.ToString();
            }

            return _conn.ConnectionString;
        }
    }
}
