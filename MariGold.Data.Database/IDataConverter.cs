namespace MariGold.Data
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	
	public interface IDataConverter
	{
		T Get<T>(IDataReader dr);
		IList<T> GetList<T>(IDataReader dr);
		IEnumerable<T> GetEnumerable<T>(IDataReader dr);
	}
}
