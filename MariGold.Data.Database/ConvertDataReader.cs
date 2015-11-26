namespace MariGold.Data
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	
	public abstract class ConvertDataReader<T> : IConvertDataReader<T>
	{
		public abstract T Get(IDataReader dr);
		public abstract IList<T> GetList(IDataReader dr);
		public abstract IEnumerable<T> GetEnumerable(IDataReader dr);
	}
}
