namespace MariGold.Data
{
	using System;
	using System.Dynamic;
	
	public interface IDbBuilder
	{
		IDatabase GetConnection();
		IConvertDataReader<T> GetConverter<T>();
		ConvertDataReader<dynamic> GetConverter();
	}
}
