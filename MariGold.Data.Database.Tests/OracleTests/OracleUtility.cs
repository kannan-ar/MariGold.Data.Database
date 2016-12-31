namespace MariGold.Data.Database.Tests.OracleTests
{
	using System;
	using System.Configuration;
	
	public static class OracleUtility
	{
		public static string ConnectionString 
		{
			get 
			{
				return "DATA SOURCE=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521)))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = FS)));PASSWORD=password2;PERSIST SECURITY INFO=True;USER ID=testusr";
			}
		}
	}
}
