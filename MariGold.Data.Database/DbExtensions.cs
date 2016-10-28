namespace MariGold.Data
{
    using System;
    using System.Data;
    using System.Collections.Generic;

    public static class DbExtensions
    {
        /// <summary>
        /// Creates an IDataReader from the given IDbConnection using the sql and other parameters.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IDataReader GetDataReader(this IDbConnection conn,
            string sql,
            CommandType commandType,
            object parameters)
        {
			IDatabase db = Db.GetConnection(conn);

            return db.GetDataReader(sql, commandType, parameters);
        }

        public static IDataReader GetDataReader(this IDbConnection conn,
            string sql,
            CommandType commandType)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.GetDataReader(sql, commandType);
        }

        public static IDataReader GetDataReader(this IDbConnection conn,
            string sql,
            object parameters)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.GetDataReader(sql, parameters);
        }

        public static IDataReader GetDataReader(this IDbConnection conn, string sql)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.GetDataReader(sql);
        }

        /// <summary>
        /// Creates an IDataReader from the given IDbConnection using the properties of Query object.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IDataReader GetDataReader(this IDbConnection conn, Query query)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.GetDataReader(query);
        }

        /// <summary>
        /// Executes the sql using the IDbConnection parameter.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int Execute(
            this IDbConnection conn,
            string sql,
            CommandType commandType,
            object parameters)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.Execute(sql, commandType, parameters);
        }

        public static int Execute(
            this IDbConnection conn,
            string sql,
            CommandType commandType)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.Execute(sql, commandType);
        }

        public static int Execute(
            this IDbConnection conn,
            string sql,
            object parameters)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.Execute(sql, parameters);
        }

        public static int Execute(this IDbConnection conn, string sql)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.Execute(sql);
        }

        /// <summary>
        /// Executes the Query object with the IDbConnection parameter.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static int Execute(this IDbConnection conn, Query query)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.Execute(query);
        }

        /// <summary>
        /// Get a scalar value using the IDbConnection and other sql parameters.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object GetScalar(
            this IDbConnection conn,
            string sql,
            CommandType commandType,
            object parameters)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.GetScalar(sql, commandType, parameters);
        }

        public static object GetScalar(
            this IDbConnection conn,
            string sql,
            CommandType commandType)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.GetScalar(sql, commandType);
        }

        public static object GetScalar(
            this IDbConnection conn,
            string sql,
            object parameters)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.GetScalar(sql, parameters);
        }

        public static object GetScalar(this IDbConnection conn, string sql)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.GetScalar(sql);
        }

        /// <summary>
        /// Get a scalar value using the IDbConnection and the Query parameters.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static object GetScalar(this IDbConnection conn, Query query)
        {
            IDatabase db = Db.GetConnection(conn);

            return db.GetScalar(query);
        }
    }
}
