namespace MariGold.Data.Database.Tests
{
	using System;
	using System.Configuration;
	
	public static class MySqlUtility
	{
		public static string ConnectionString 
		{
			get 
			{
				return ConfigurationManager.AppSettings["mysqlConnection"];
			}
		}
	}
}
