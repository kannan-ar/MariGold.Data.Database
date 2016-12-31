namespace MariGold.Data.Database.Tests.SqlServerTests
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
