namespace MariGold.Data
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	
	public interface IDataConverter
	{
		T Get<T>(IDataReader dr) where T : class, new();
		IList<T> GetList<T>(IDataReader dr) where T : class, new();
		IEnumerable<T> GetEnumerable<T>(IDataReader dr) where T : class, new();
	}
}
