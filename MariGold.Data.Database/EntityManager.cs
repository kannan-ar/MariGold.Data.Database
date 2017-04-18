namespace MariGold.Data
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Collections.Generic;

    public static class EntityManager<T>
    {
        private static ConcurrentDictionary<Type, EntityMapper> maps;

        static EntityManager()
        {
            maps = new ConcurrentDictionary<Type, EntityMapper>();
        }

        public class EntityMapper
        {
            private bool disposeAfterUse;

            private Dictionary<string, string> columns;

            internal Dictionary<string, string> Columns
            {
                get
                {
                    return columns;
                }
            }

            internal bool ShouldDisposeAfterUse
            {
                get
                {
                    return disposeAfterUse;
                }
            }

            internal void Init()
            {
                columns = new Dictionary<string, string>();
            }

            public EntityMapper Map(Expression<Func<T, object>> item, string columnName)
            {
                string propertyName = string.Empty;

                if (item.Body is UnaryExpression)
                    propertyName = ((MemberExpression)(item.Body as UnaryExpression).Operand).Member.Name;
                else if (item.Body is MemberExpression)
                    propertyName = ((MemberExpression)item.Body).Member.Name;

                columns.Add(propertyName, columnName);

                return this;
            }

            public EntityMapper DisposeAfterUse()
            {
                this.disposeAfterUse = true;

                return this;
            }
        }

        internal static Dictionary<string, string> Get(out bool shouldDispose)
        {
            Type type = typeof(T);
            EntityMapper mapper;
            shouldDispose = false;

            if (maps.TryGetValue(type, out mapper))
            {
                if (mapper.ShouldDisposeAfterUse)
                {
                    shouldDispose = true;
                    maps.TryRemove(type, out mapper);
                }

                return mapper.Columns;
            }
            else
            {
                return null;
            }
        }

        public static EntityMapper Map(Expression<Func<T, object>> item, string columnName)
        {
            EntityMapper mapper;
            Type type = typeof(T);

            Db.GetConverter().ClearType<T>();

            if (!maps.TryGetValue(type, out mapper))
            {
                mapper = new EntityMapper();
                maps.TryAdd(type, mapper);
            }

            mapper.Init();
            mapper.Map(item, columnName);

            return mapper;
        }
    }
}
