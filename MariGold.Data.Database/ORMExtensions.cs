namespace MariGold.Data
{
    using System;
    using System.Data;
    using System.Collections.Generic;

    public static class ORMExtensions
    {
    	/// <summary>
    	/// Creates type T instance from the sql query using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
    	/// </summary>
    	/// <param name="conn"></param>
    	/// <param name="sql"></param>
    	/// <param name="commandType"></param>
    	/// <param name="parameters"></param>
    	/// <returns></returns>
        public static T Get<T>(this IDbConnection conn,
            string sql,
            CommandType commandType = CommandType.Text,
            IDictionary<string, object> parameters = null)
        {
            return Get<T>(conn, new Query(sql, commandType, parameters));
        }

    	/// <summary>
    	/// Creates type T instance from the Query parameter using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
    	/// </summary>
    	/// <param name="conn"></param>
    	/// <param name="query"></param>
    	/// <returns></returns>
        public static T Get<T>(this IDbConnection conn, Query query)
        {
            ORM orm = new ORM(new DbBuilder(conn));

            return orm.Get<T>(query);
        }

        /// <summary>
        /// Creates a list of type T objects from the sql query using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(this IDbConnection conn,
           string sql,
           CommandType commandType = CommandType.Text,
           IDictionary<string, object> parameters = null)
        {
            return GetList<T>(conn, new Query(sql, commandType, parameters));
        }

        /// <summary>
        /// Creates a list of type T objects from the Query parameter using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(this IDbConnection conn,Query query)
        {
            ORM orm = new ORM(new DbBuilder(conn));

            return orm.GetList<T>(query);
        }

        /// <summary>
        /// Creates an Enumerable of type T from the sql query using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumerable<T>(this IDbConnection conn,
           string sql,
           CommandType commandType = CommandType.Text,
           IDictionary<string, object> parameters = null)
        {
            return GetEnumerable<T>(conn, new Query(sql, commandType, parameters));
        }

        /// <summary>
        /// Creates an Enumerable of type T from the Query parameter using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumerable<T>(this IDbConnection conn,Query query)
        {
            ORM orm = new ORM(new DbBuilder(conn));

            return orm.GetEnumerable<T>(query);
        }
    }
}
