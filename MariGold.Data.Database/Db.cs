namespace MariGold.Data
{
	using System;
	using System.Dynamic;
	
	public abstract class Db
	{
		public abstract IDatabase GetConnection();
		public abstract ConvertDataReader<T> GetConverter<T>();
		public abstract ConvertDataReader<dynamic> GetConverter();
	}
}
