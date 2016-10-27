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
                return @"Server=.\sqlexpress;Database=Tests;User Id=testusr;Password=pass@word1;";
			}
		}
	}
}
