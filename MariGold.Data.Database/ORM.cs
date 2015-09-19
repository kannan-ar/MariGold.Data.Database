namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// A wrapper class for all ORM operations. Requires the implementation of IDatabase and IConvertDataReader as constructor parameters.
    /// Provides methods for converting sql select statments as CLR objects. 
    /// </summary>
    public sealed class ORM
    {
        private readonly IDatabase db;
        private readonly IConvertDataReader converter;

        /// <summary>
        /// Constructs an instance of ORM using the IDatabase and IConvertDataReader parameters.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="converter"></param>
        public ORM(IDatabase db, IConvertDataReader converter)
        {
            ValidateORM(db, converter);

            this.db = db;
            this.converter = converter;
        }

        /// <summary>
        /// Constructs an instance of ORM using an IDbBuilder parameter. The instance of IDbBuilder provides the implementation of IDatabase and IConvertDataReader.
        /// </summary>
        /// <param name="builder"></param>
        public ORM(IDbBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            IDatabase db = builder.GetConnection();
            IConvertDataReader converter = builder.GetConverter();

            ValidateORM(db, converter);

            this.db = db;
            this.converter = converter;
        }

        private void ValidateORM(IDatabase db, IConvertDataReader converter)
        {
            if (db == null)
            {
                throw new ApplicationException("Connection is null");
            }

            if (db.State != ConnectionState.Open)
            {
                throw new ApplicationException("Database connection is not opened");
            }

            if (converter == null)
            {
                throw new ApplicationException("Converter is null");
            }
        }

        /// <summary>
        /// Creates a type T object from the sql query using the IDatabase and IConvertDataReader instances provided through the constructor methods.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T Get<T>(
            string sql,
            CommandType commandType = CommandType.Text,
            IDictionary<string, object> parameters = null)
        {
            return Get<T>(new Query(sql, commandType, parameters));
        }

        /// <summary>
        /// Creates a type T object from the Query parameter using the IDatabase and IConvertDataReader instances provided through the constructor methods.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public T Get<T>(Query query)
        {
            return converter.Get<T>(db.GetDataReader(query));
        }

        /// <summary>
        /// Creates a list of type T objects from the sql query using the IDatabase and IConvertDataReader instances provided through the constructor methods.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> GetList<T>(
            string sql,
            CommandType commandType = CommandType.Text,
            IDictionary<string, object> parameters = null)
        {
            return GetList<T>(new Query(sql, commandType, parameters));
        }

        /// <summary>
        /// Creates a list of type T objects from the Query parameter using the IDatabase and IConvertDataReader instances provided through the constructor methods.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<T> GetList<T>(Query query)
        {
            return converter.GetList<T>(db.GetDataReader(query));
        }

        /// <summary>
        /// Creates an Enumberable of type T objects from the sql query using the IDatabase and IConvertDataReader instances provided through the constructor methods.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> GetEnumerable<T>(
            string sql,
            CommandType commandType = CommandType.Text,
            IDictionary<string, object> parameters = null)
        {
            return GetEnumerable<T>(new Query(sql, commandType, parameters));
        }

        /// <summary>
        /// Creates an Enumberable of type T objects from the Query parameter using the IDatabase and IConvertDataReader instances provided through the constructor methods.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<T> GetEnumerable<T>(Query query)
        {
            return converter.GetEnumerable<T>(db.GetDataReader(query));
        }
    }
}
