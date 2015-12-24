namespace MariGold.Data
{
	using System;
	using System.Data;
	using System.Collections.Generic;
	
	public class Db
	{
		private static object dbLock = new object();
		private static object converterLock = new object();
		private static object dynamicLock = new object();
		
		private static IDatabase db;
		private static IDynamicDataConverter dynamicConvertor;
		private static IDataConverter dataConvertor;
		
		private IDbConnection conn;
		
		internal static IDatabase GetConnection(IDbConnection conn)
		{
			lock (dbLock) 
			{
				if (db == null) 
				{
					db = new Database(conn);
				}
			}
			
			return db;
		}
		
		internal static IDataConverter GetConverter()
		{
			lock (converterLock) 
			{
				if (dataConvertor == null) 
				{
					dataConvertor = new ILDataConverter();
				}
			}
			
			return dataConvertor;
		}
		
		internal static IDynamicDataConverter GetDynamicConvertor()
		{
			lock (dynamicLock) 
			{
				if (dynamicConvertor == null) 
				{
					dynamicConvertor = new DynamicDataConverter();
				}
			}
			
			return dynamicConvertor;
		}
		
		public static void SetDatabase(IDatabase database)
		{
			lock (dbLock) 
			{
				db = database;
			}
		}
		
		public static void SetConverter(IDataConverter converter)
		{
			lock (converterLock) {
				dataConvertor = converter;
			}
		}
		
		public static void SetDynamicConverter(IDynamicDataConverter converter)
		{
			lock (dynamicLock) 
			{
				dynamicConvertor = converter;
			}
		}
		
		
		
		public Db(IDbConnection conn)
		{
			this.conn = conn;
		}
		
		public T Get<T>(string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.Get<T>(db.GetDataReader(sql, commandType, parameters));
		}
		
		public T Get<T>(string sql,
			CommandType commandType)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.Get<T>(db.GetDataReader(sql, commandType));
		}
		
		public T Get<T>(string sql,
			IDictionary<string, object> parameters)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.Get<T>(db.GetDataReader(sql, parameters));
		}
		
		public T Get<T>(string sql)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.Get<T>(db.GetDataReader(sql));
		}
		
		public T Get<T>(Query query)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.Get<T>(db.GetDataReader(query));
		}
		
		public IList<T> GetList<T>(string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.GetList<T>(db.GetDataReader(sql, commandType, parameters));
		}
		
		public IList<T> GetList<T>(string sql,
			CommandType commandType)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.GetList<T>(db.GetDataReader(sql, commandType));
		}
		
		public IList<T> GetList<T>(string sql,
			IDictionary<string, object> parameters)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.GetList<T>(db.GetDataReader(sql, parameters));
		}
		
		public IList<T> GetList<T>(string sql)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.GetList<T>(db.GetDataReader(sql));
		}
		
		public IList<T> GetList<T>(Query query)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.GetList<T>(db.GetDataReader(query));
		}
		
		public IEnumerable<T> GetEnumerable<T>(string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.GetEnumerable<T>(db.GetDataReader(sql, commandType, parameters));
		}
		
		public IEnumerable<T> GetEnumerable<T>(string sql,
			CommandType commandType)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.GetEnumerable<T>(db.GetDataReader(sql, commandType));
		}
		
		public IEnumerable<T> GetEnumerable<T>(string sql,
			IDictionary<string, object> parameters)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.GetEnumerable<T>(db.GetDataReader(sql, parameters));
		}
		
		public IEnumerable<T> GetEnumerable<T>(string sql)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.GetEnumerable<T>(db.GetDataReader(sql));
		}
		
		public IEnumerable<T> GetEnumerable<T>(Query query)
		{
			var db = GetConnection(conn);
			var converter = GetConverter();
			
			return converter.GetEnumerable<T>(db.GetDataReader(query));
		}
		
		public dynamic Get(string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.Get(db.GetDataReader(sql, commandType, parameters));
		}
		
		public dynamic Get(string sql,
			CommandType commandType)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.Get(db.GetDataReader(sql, commandType));
		}
		
		public dynamic Get(string sql,
			IDictionary<string, object> parameters)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.Get(db.GetDataReader(sql, parameters));
		}
		
		public dynamic Get(string sql)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.Get(db.GetDataReader(sql));
		}
		
		public dynamic Get(Query query)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.Get(db.GetDataReader(query));
		}
		
		public IList<dynamic> GetList(string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.GetList(db.GetDataReader(sql, commandType, parameters));
		}
		
		public IList<dynamic> GetList(string sql,
			CommandType commandType)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.GetList(db.GetDataReader(sql, commandType));
		}
		
		public IList<dynamic> GetList(string sql,
			IDictionary<string, object> parameters)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.GetList(db.GetDataReader(sql, parameters));
		}
		
		public IList<dynamic> GetList(string sql)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.GetList(db.GetDataReader(sql));
		}
		
		public IList<dynamic> GetList(Query query)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.GetList(db.GetDataReader(query));
		}
		
		public IEnumerable<dynamic> GetEnumerable(string sql,
			CommandType commandType,
			IDictionary<string, object> parameters)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.GetEnumerable(db.GetDataReader(sql, commandType, parameters));
		}
		
		public IEnumerable<dynamic> GetEnumerable(string sql,
			CommandType commandType)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.GetEnumerable(db.GetDataReader(sql, commandType));
		}
		
		public IEnumerable<dynamic> GetEnumerable(string sql,
			IDictionary<string, object> parameters)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.GetEnumerable(db.GetDataReader(sql, parameters));
		}
		
		public IEnumerable<dynamic> GetEnumerable(string sql)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.GetEnumerable(db.GetDataReader(sql));
		}
		
		public IEnumerable<dynamic> GetEnumerable(Query query)
		{
			var db = GetConnection(conn);
			var converter = GetDynamicConvertor();
			
			return converter.GetEnumerable(db.GetDataReader(query));
		}
	}
}
