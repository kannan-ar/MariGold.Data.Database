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

        private static Func<IDbConnection, IDatabase> dbFunc;
        private static IDynamicDataConverter dynamicConvertor;
        private static DataConverter dataConvertor;

        private readonly IDbConnection conn;

        internal static IDatabase GetConnection(IDbConnection conn)
        {
            IDatabase db = null;

            lock (dbLock)
            {
                if (dbFunc != null)
                {
                    db = dbFunc(conn);
                }
                else
                {
                    db = new Database(conn);
                }
            }

            return db;
        }

        internal static DataConverter GetConverter()
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

        public static void SetDatabase(Func<IDbConnection, IDatabase> func)
        {
            lock (dbLock)
            {
                dbFunc = func;
            }
        }

        public static void SetConverter(DataConverter converter)
        {
            lock (converterLock)
            {
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
            object parameters)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            using (IDataReader dr = db.GetDataReader(sql, commandType, parameters))
            {
                return converter.Get<T>(dr);
            }
        }

        public T Get<T>(string sql,
            CommandType commandType)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            using (IDataReader dr = db.GetDataReader(sql, commandType))
            {
                return converter.Get<T>(dr);
            }
        }

        public T Get<T>(string sql,
            object parameters)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            using (IDataReader dr = db.GetDataReader(sql, parameters))
            {
                return converter.Get<T>(dr);
            }
        }

        public T Get<T>(string sql)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            using (IDataReader dr = db.GetDataReader(sql))
            {
                return converter.Get<T>(dr);
            }
        }

        public T Get<T>(Query query)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            using (IDataReader dr = db.GetDataReader(query))
            {
                return converter.Get<T>(dr);
            }
        }

        public IList<T> GetList<T>(string sql,
            CommandType commandType,
            object parameters)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            using (IDataReader dr = db.GetDataReader(sql, commandType, parameters))
            {
                return converter.GetList<T>(dr);
            }
        }

        public IList<T> GetList<T>(string sql,
            CommandType commandType)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            using (IDataReader dr = db.GetDataReader(sql, commandType))
            {
                return converter.GetList<T>(dr);
            }
        }

        public IList<T> GetList<T>(string sql,
            object parameters)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            using (IDataReader dr = db.GetDataReader(sql, parameters))
            {
                return converter.GetList<T>(dr);
            }
        }

        public IList<T> GetList<T>(string sql)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            using (IDataReader dr = db.GetDataReader(sql))
            {
                return converter.GetList<T>(dr);
            }
        }

        public IList<T> GetList<T>(Query query)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            using (IDataReader dr = db.GetDataReader(query))
            {
                return converter.GetList<T>(dr);
            }
        }

        public IEnumerable<T> GetEnumerable<T>(string sql,
            CommandType commandType,
            object parameters)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            return converter.GetEnumerable<T>(db.GetDataReader(sql, commandType, parameters));
        }

        public IEnumerable<T> GetEnumerable<T>(string sql,
            CommandType commandType)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            return converter.GetEnumerable<T>(db.GetDataReader(sql, commandType));
        }

        public IEnumerable<T> GetEnumerable<T>(string sql,
            object parameters)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            return converter.GetEnumerable<T>(db.GetDataReader(sql, parameters));
        }

        public IEnumerable<T> GetEnumerable<T>(string sql)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            return converter.GetEnumerable<T>(db.GetDataReader(sql));
        }

        public IEnumerable<T> GetEnumerable<T>(Query query)
             where T : class, new()
        {
            var db = GetConnection(conn);
            var converter = GetConverter();

            return converter.GetEnumerable<T>(db.GetDataReader(query));
        }

        public dynamic Get(string sql,
            CommandType commandType,
            object parameters)
        {
            var db = GetConnection(conn);
            var converter = GetDynamicConvertor();

            using (IDataReader dr = db.GetDataReader(sql, commandType, parameters))
            {
                return converter.Get(dr);
            }
        }

        public dynamic Get(string sql,
            CommandType commandType)
        {
            var db = GetConnection(conn);
            var converter = GetDynamicConvertor();

            using (IDataReader dr = db.GetDataReader(sql, commandType))
            {
                return converter.Get(dr);
            }
        }

        public dynamic Get(string sql,
            object parameters)
        {
            var db = GetConnection(conn);
            var converter = GetDynamicConvertor();

            using (IDataReader dr = db.GetDataReader(sql, parameters))
            {
                return converter.Get(dr);
            }
        }

        public dynamic Get(string sql)
        {
            var db = GetConnection(conn);
            var converter = GetDynamicConvertor();

            using (IDataReader dr = db.GetDataReader(sql))
            {
                return converter.Get(dr);
            }
        }

        public dynamic Get(Query query)
        {
            var db = GetConnection(conn);
            var converter = GetDynamicConvertor();

            using (IDataReader dr = db.GetDataReader(query))
            {
                return converter.Get(dr);
            }
        }

        public IList<dynamic> GetList(string sql,
            CommandType commandType,
            object parameters)
        {
            var db = GetConnection(conn);
            var converter = GetDynamicConvertor();

            using (IDataReader dr = db.GetDataReader(sql, commandType, parameters))
            {
                return converter.GetList(dr);
            }
        }

        public IList<dynamic> GetList(string sql,
            CommandType commandType)
        {
            var db = GetConnection(conn);
            var converter = GetDynamicConvertor();

            using (IDataReader dr = db.GetDataReader(sql, commandType))
            {
                return converter.GetList(dr);
            }
        }

        public IList<dynamic> GetList(string sql,
            object parameters)
        {
            var db = GetConnection(conn);
            var converter = GetDynamicConvertor();

            using (IDataReader dr = db.GetDataReader(sql, parameters))
            {
                return converter.GetList(dr);
            }
        }

        public IList<dynamic> GetList(string sql)
        {
            var db = GetConnection(conn);
            var converter = GetDynamicConvertor();

            using (IDataReader dr = db.GetDataReader(sql))
            {
                return converter.GetList(dr);
            }
        }

        public IList<dynamic> GetList(Query query)
        {
            var db = GetConnection(conn);
            var converter = GetDynamicConvertor();

            using (IDataReader dr = db.GetDataReader(query))
            {
                return converter.GetList(dr);
            }
        }

        public IEnumerable<dynamic> GetEnumerable(string sql,
            CommandType commandType,
            object parameters)
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
            object parameters)
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

        public IRecordSet QueryMultiple(string sql,
            CommandType commandType,
            object parameters)
        {
            var db = GetConnection(conn);
            return new MultiRecordSet(db.GetDataReader(sql, commandType, parameters));
        }

        public IRecordSet QueryMultiple(string sql,
            CommandType commandType)
        {
            var db = GetConnection(conn);
            return new MultiRecordSet(db.GetDataReader(sql, commandType));
        }

        public IRecordSet QueryMultiple(string sql,
            object parameters)
        {
            var db = GetConnection(conn);
            return new MultiRecordSet(db.GetDataReader(sql, parameters));
        }

        public IRecordSet QueryMultiple(string sql)
        {
            var db = GetConnection(conn);
            return new MultiRecordSet(db.GetDataReader(sql));
        }

        public IRecordSet QueryMultiple(Query query)
        {
            var db = GetConnection(conn);
            return new MultiRecordSet(db.GetDataReader(query));
        }
    }
}
