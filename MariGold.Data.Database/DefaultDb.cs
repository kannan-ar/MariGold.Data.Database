namespace MariGold.Data
{
    using System;
    using System.Data;
	
    /// <summary>
    /// Default implementation of IDbBuilder. Provides default implementation for database connection and data reader converter.
    /// </summary>
    public sealed class DefaultDb : Db
    {
        private IDbConnection connection;

        public static Db Create(IDbConnection connection)
        {
			return new DefaultDb(connection);
        }
        
         /// <summary>
        /// Initialize the DbBuilder object with an instance of IDbConnection. Throws ArgumentNullException if the IDbConnection is null.
        /// </summary>
        /// <param name="connection"></param>
        public DefaultDb(IDbConnection connection)
        {
        	 if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            this.connection = connection;
        }

        /// <summary>
        /// Get the default implementation of IDatabase
        /// </summary>
        /// <returns></returns>
        public override IDatabase GetConnection()
        {
            return new Database(connection);
        }

        /// <summary>
        /// Get the default implementation IConvertDataReader
        /// </summary>
        /// <returns></returns>
        public override ConvertDataReader<T> GetConverter<T>()
        {
            return new ConvertILDataReader<T>();
        }
        
        public override ConvertDataReader<dynamic> GetConverter()
        {
			return new ConvertDynamicDataReader();
        }
    }
}
