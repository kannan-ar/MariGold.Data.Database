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
				return "Server=localhost;Database=Tests;Uid=testusr;Pwd=pass@word1;";
			}
		}
	}
}
