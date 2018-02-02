namespace MariGold.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

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

        protected Dictionary<string, MethodInfo> GetDrMethod(Type recordType)
        {
            Dictionary<string, MethodInfo> list = new Dictionary<string, MethodInfo>();

            list.Add("Int32", recordType.GetMethod("GetInt32"));
            list.Add("String", recordType.GetMethod("GetString"));
            list.Add("Boolean", recordType.GetMethod("GetBoolean"));
            list.Add("Char", recordType.GetMethod("GetChar"));
            list.Add("DateTime", recordType.GetMethod("GetDateTime"));
            list.Add("Decimal", recordType.GetMethod("GetDecimal"));
            list.Add("Double", recordType.GetMethod("GetDouble"));
            list.Add("Single", recordType.GetMethod("GetFloat"));
            list.Add("Int16", recordType.GetMethod("GetInt16"));
            list.Add("Int64", recordType.GetMethod("GetInt64"));

            return list;
        }

        public abstract void ClearType<T>();
        public abstract void ClearAllTypes();
        public abstract T Get<T>(IDataReader dr) where T : class, new();
        public abstract IList<T> GetList<T>(IDataReader dr) where T : class, new();
        public abstract IEnumerable<T> GetEnumerable<T>(IDataReader dr) where T : class, new();
    }
}
