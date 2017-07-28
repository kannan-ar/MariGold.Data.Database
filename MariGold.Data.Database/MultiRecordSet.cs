namespace MariGold.Data
{
    using System.Collections.Generic;
    using System.Data;

    public sealed class MultiRecordSet : IRecordSet
    {
        private readonly IDataReader reader;

        public MultiRecordSet(IDataReader reader)
        {
            this.reader = reader;
        }

        public T Get<T>()
            where T : class, new()
        {
            var converter = Db.GetConverter();

            T item = converter.Get<T>(reader);

            reader.NextResult();

            return item;
        }

        public IList<T> GetList<T>()
            where T : class, new()
        {
            var converter = Db.GetConverter();

            IList<T> list = converter.GetList<T>(reader);

            reader.NextResult();

            return list;
        }

        public IEnumerable<T> GetEnumerable<T>()
            where T : class, new()
        {
            var converter = Db.GetConverter();

            IEnumerable<T> list = converter.GetEnumerable<T>(reader);

            reader.NextResult();

            return converter.GetEnumerable<T>(reader);
        }

        public dynamic Get()
        {
            var converter = Db.GetDynamicConvertor();

            dynamic item = converter.Get(reader);

            reader.NextResult();

            return item;
        }

        public IList<dynamic> GetList()
        {
            var converter = Db.GetDynamicConvertor();

            IList<dynamic> list = converter.GetList(reader);

            reader.NextResult();

            return list;
        }

        public IEnumerable<dynamic> GetEnumerable()
        {
            var converter = Db.GetDynamicConvertor();

            IEnumerable<dynamic> list = converter.GetEnumerable(reader);

            reader.NextResult();

            return list;
        }

        public object GetScalar()
        {
            if(reader.Read())
            {
                object obj = reader[0];

                reader.NextResult();

                return obj;
            }
            else
            {
                return null;
            }
        }
    }
}
