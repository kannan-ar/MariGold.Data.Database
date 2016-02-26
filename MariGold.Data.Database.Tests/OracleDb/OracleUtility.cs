namespace MariGold.Data.Database.Tests.OracleDb
{
	using System;
	using System.Configuration;
	
	public static class OracleUtility
	{
		public static string ConnectionString 
		{
			get 
			{
				return ConfigurationManager.AppSettings["oracleConnection"];
			}
		}
	}
}
