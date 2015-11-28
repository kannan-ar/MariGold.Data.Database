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
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();
			
			return converter.Get(db.GetDataReader(sql, commandType, parameters));
		}

		public static T Get<T>(this IDbConnection conn,
			string sql,
			CommandType commandType)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();

			return converter.Get(db.GetDataReader(sql, commandType));
		}

		public static T Get<T>(this IDbConnection conn,
			string sql,
			IDictionary<string, object> parameters)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();

			return converter.Get(db.GetDataReader(sql, parameters));
		}

		public static T Get<T>(this IDbConnection conn, string sql)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();
			
			return converter.Get(db.GetDataReader(sql));
		}

		/// <summary>
		/// Creates type T instance from the Query parameter using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public static T Get<T>(this IDbConnection conn, Query query)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();

			return converter.Get(db.GetDataReader(query));
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
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();

			return converter.GetList(db.GetDataReader(sql, commandType, parameters));
		}

		public static IList<T> GetList<T>(this IDbConnection conn,
			string sql,
			CommandType commandType)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();

			return converter.GetList(db.GetDataReader(sql, commandType));
		}

		public static IList<T> GetList<T>(this IDbConnection conn,
			string sql,
			IDictionary<string, object> parameters)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();

			return converter.GetList(db.GetDataReader(sql, parameters));
		}

		public static IList<T> GetList<T>(this IDbConnection conn, string sql)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();
			
			return converter.GetList(db.GetDataReader(sql));
		}

		/// <summary>
		/// Creates a list of type T objects from the Query parameter using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IList<T> GetList<T>(this IDbConnection conn, Query query)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();
			
			return converter.GetList(db.GetDataReader(query));
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
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();
			
			return converter.GetEnumerable(db.GetDataReader(sql, commandType, parameters));
		}

		public static IEnumerable<T> GetEnumerable<T>(this IDbConnection conn,
			string sql,
			CommandType commandType)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();

			return converter.GetEnumerable(db.GetDataReader(sql, commandType));
		}

		public static IEnumerable<T> GetEnumerable<T>(this IDbConnection conn,
			string sql,
			IDictionary<string, object> parameters)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();
			
			return converter.GetEnumerable(db.GetDataReader(sql, parameters));
		}

		public static IEnumerable<T> GetEnumerable<T>(this IDbConnection conn, string sql)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();
			
			return converter.GetEnumerable(db.GetDataReader(sql));
		}

		/// <summary>
		/// Creates an Enumerable of type T from the Query parameter using the default implementations of IDatabase and IConvertDataReader provided through DbBuilder class.
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetEnumerable<T>(this IDbConnection conn, Query query)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter<T>();
			
			return converter.GetEnumerable(db.GetDataReader(query));
		}
        
        
		public static dynamic Get(this IDbConnection conn,
			string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();
			
			return converter.Get(db.GetDataReader(sql, commandType, parameters));
		}
        
		public static dynamic Get(this IDbConnection conn,
			string sql,
			CommandType commandType)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();
			
			return converter.Get(db.GetDataReader(sql, commandType));
		}
        
		public static dynamic Get(this IDbConnection conn,
			string sql,
			IDictionary<string, object> parameters)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();
			
			return converter.Get(db.GetDataReader(sql, parameters));
		}
        
		public static dynamic Get(this IDbConnection conn,
			string sql)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();
			
			return converter.Get(db.GetDataReader(sql));
		}
        
		public static dynamic Get(this IDbConnection conn, Query query)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();
			
			return converter.Get(db.GetDataReader(query));
		}
		
		public static IList<dynamic> GetList(this IDbConnection conn,
			string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();

			return converter.GetList(db.GetDataReader(sql, commandType, parameters));
		}
		
		public static IList<dynamic> GetList(this IDbConnection conn,
			string sql,
			CommandType commandType)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();

			return converter.GetList(db.GetDataReader(sql, commandType));
		}
		
		public static IList<dynamic> GetList(this IDbConnection conn,
			string sql,
			IDictionary<string, object> parameters)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();

			return converter.GetList(db.GetDataReader(sql, parameters));
		}
		
		public static IList<dynamic> GetList(this IDbConnection conn, string sql)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();

			return converter.GetList(db.GetDataReader(sql));
		}
		
		public static IList<dynamic> GetList(this IDbConnection conn, Query query)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();

			return converter.GetList(db.GetDataReader(query));
		}
		
		public static IEnumerable<dynamic> GetEnumerable(this IDbConnection conn,
			string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();
			
			return converter.GetEnumerable(db.GetDataReader(sql, commandType, parameters));
		}
		
		public static IEnumerable<dynamic> GetEnumerable(this IDbConnection conn,
			string sql,
			CommandType commandType)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();
			
			return converter.GetEnumerable(db.GetDataReader(sql, commandType));
		}
		
		public static IEnumerable<dynamic> GetEnumerable(this IDbConnection conn,
			string sql,
			IDictionary<string, object> parameters)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();
			
			return converter.GetEnumerable(db.GetDataReader(sql, parameters));
		}
		
		public static IEnumerable<dynamic> GetEnumerable(this IDbConnection conn, string sql)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();
			
			return converter.GetEnumerable(db.GetDataReader(sql));
		}
		
		public static IEnumerable<dynamic> GetEnumerable(this IDbConnection conn, Query query)
		{
			Db builder = DefaultDb.Create(conn);
			var db = builder.GetConnection();
			var converter = builder.GetConverter();
			
			return converter.GetEnumerable(db.GetDataReader(query));
		}
	}
}
