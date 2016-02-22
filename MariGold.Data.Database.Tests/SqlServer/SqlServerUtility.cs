namespace MariGold.Data.Database.Tests.SqlServer
{
	using System;
	using System.Configuration;

	public static class SqlServerUtility
	{
		public static string ConnectionString 
		{
			get 
			{
				return ConfigurationManager.AppSettings["sqlServerConnection"];
			}
		}
	}
}
