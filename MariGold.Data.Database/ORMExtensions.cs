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
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			Db db = new Db(conn);
			
			return db.Get(sql, commandType, parameters);
		}

		public static T Get<T>(this IDbConnection conn,
			string sql,
			CommandType commandType)
		{
			Db db = new Db(conn);

			return db.Get(sql, commandType);
		}

		public static T Get<T>(this IDbConnection conn,
			string sql,
			IDictionary<string, object> parameters)
		{
			Db db = new Db(conn);

			return db.Get(sql, parameters);
		}

		public static T Get<T>(this IDbConnection conn, string sql)
		{
			Db db = new Db(conn);
			
			return db.Get(sql);
		}

		/// <summary>
		/// Creates type T instance from the Query parameter using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public static T Get<T>(this IDbConnection conn, Query query)
		{
			Db db = new Db(conn);

			return db.Get(query);
		}

		/// <summary>
		/// Creates a list of type T objects from the sql query using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="sql"></param>
		/// <param name="commandType"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static IList<T> GetList<T>(this IDbConnection conn,
			string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			Db db = new Db(conn);

			return db.GetList<T>(sql, commandType, parameters);
		}

		public static IList<T> GetList<T>(this IDbConnection conn,
			string sql,
			CommandType commandType)
		{
			Db db = new Db(conn);

			return db.GetList<T>(sql, commandType);
		}

		public static IList<T> GetList<T>(this IDbConnection conn,
			string sql,
			IDictionary<string, object> parameters)
		{
			Db db = new Db(conn);

			return db.GetList<T>(sql, parameters);
		}

		public static IList<T> GetList<T>(this IDbConnection conn, string sql)
		{
			Db db = new Db(conn);
			
			return db.GetList<T>(sql);
		}

		/// <summary>
		/// Creates a list of type T objects from the Query parameter using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IList<T> GetList<T>(this IDbConnection conn, Query query)
		{
			Db db = new Db(conn);
			
			return db.GetList<T>(query);
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
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			Db db = new Db(conn);
			
			return db.GetEnumerable<T>(sql, commandType, parameters);
		}

		public static IEnumerable<T> GetEnumerable<T>(this IDbConnection conn,
			string sql,
			CommandType commandType)
		{
			Db db = new Db(conn);

			return db.GetEnumerable<T>(sql, commandType);
		}

		public static IEnumerable<T> GetEnumerable<T>(this IDbConnection conn,
			string sql,
			IDictionary<string, object> parameters)
		{
			Db db = new Db(conn);
			
			return db.GetEnumerable<T>(sql, parameters);
		}

		public static IEnumerable<T> GetEnumerable<T>(this IDbConnection conn, string sql)
		{
			Db db = new Db(conn);
			
			return db.GetEnumerable<T>(sql);
		}

		/// <summary>
		/// Creates an Enumerable of type T from the Query parameter using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetEnumerable<T>(this IDbConnection conn, Query query)
		{
			Db db = new Db(conn);
			
			return db.GetEnumerable<T>(query);
		}
        
        
		public static dynamic Get(this IDbConnection conn,
			string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			Db db = new Db(conn);
			
			return db.Get(sql, commandType, parameters);
		}
        
		public static dynamic Get(this IDbConnection conn,
			string sql,
			CommandType commandType)
		{
			Db db = new Db(conn);
			
			return db.Get(sql, commandType);
		}
        
		public static dynamic Get(this IDbConnection conn,
			string sql,
			IDictionary<string, object> parameters)
		{
			Db db = new Db(conn);
			
			return db.Get(sql, parameters);
		}
        
		public static dynamic Get(this IDbConnection conn,
			string sql)
		{
			Db db = new Db(conn);
			
			return db.Get(sql);
		}
        
		public static dynamic Get(this IDbConnection conn, Query query)
		{
			Db db = new Db(conn);
			
			return db.Get(query);
		}
		
		public static IList<dynamic> GetList(this IDbConnection conn,
			string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			Db db = new Db(conn);

			return db.GetList(sql, commandType, parameters);
		}
		
		public static IList<dynamic> GetList(this IDbConnection conn,
			string sql,
			CommandType commandType)
		{
			Db db = new Db(conn);

			return db.GetList(sql, commandType);
		}
		
		public static IList<dynamic> GetList(this IDbConnection conn,
			string sql,
			IDictionary<string, object> parameters)
		{
			Db db = new Db(conn);

			return db.GetList(sql, parameters);
		}
		
		public static IList<dynamic> GetList(this IDbConnection conn, string sql)
		{
			Db db = new Db(conn);

			return db.GetList(sql);
		}
		
		public static IList<dynamic> GetList(this IDbConnection conn, Query query)
		{
			Db db = new Db(conn);

			return db.GetList(query);
		}
		
		public static IEnumerable<dynamic> GetEnumerable(this IDbConnection conn,
			string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			Db db = new Db(conn);
			
			return db.GetEnumerable(sql, commandType, parameters);
		}
		
		public static IEnumerable<dynamic> GetEnumerable(this IDbConnection conn,
			string sql,
			CommandType commandType)
		{
			Db db = new Db(conn);
			
			return db.GetEnumerable(sql, commandType);
		}
		
		public static IEnumerable<dynamic> GetEnumerable(this IDbConnection conn,
			string sql,
			IDictionary<string, object> parameters)
		{
			Db db = new Db(conn);
			
			return db.GetEnumerable(sql, parameters);
		}
		
		public static IEnumerable<dynamic> GetEnumerable(this IDbConnection conn, string sql)
		{
			Db db = new Db(conn);
			
			return db.GetEnumerable(sql);
		}
		
		public static IEnumerable<dynamic> GetEnumerable(this IDbConnection conn, Query query)
		{
			Db db = new Db(conn);
			
			return db.GetEnumerable(query);
		}
	}
}
