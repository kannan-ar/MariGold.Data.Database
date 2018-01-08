namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;

    public abstract class DataConverter
	{
        protected string ConvertCamelStringToUnderscore(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            char[] charArray = new char[text.Length * 2];
            int index = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0 && char.IsUpper(text[i]))
                {
                    charArray[index] = '_';
                    ++index;
                }

                charArray[index] = char.ToLower(text[i]);
                ++index;
            }

            return new String(charArray, 0, index);
        }

        public abstract void ClearType<T>();
        public abstract void ClearAllTypes();
        public abstract T Get<T>(IDataReader dr, Expression<Func<T, object>>[] properties = null) where T : class, new();
        public abstract IList<T> GetList<T>(IDataReader dr, Expression<Func<T, object>>[] properties = null) where T : class, new();
        public abstract IEnumerable<T> GetEnumerable<T>(IDataReader dr, Expression<Func<T, object>>[] properties = null) where T : class, new();
    }
}
