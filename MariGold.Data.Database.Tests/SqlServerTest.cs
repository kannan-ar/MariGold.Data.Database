namespace MariGold.Data.Database.Tests
{
	using System;
	using NUnit.Framework;
	using MariGold.Data;
	using System.Data;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Linq;

	[TestFixture]
	public class SqlServerTest
	{
		private const string connectionString = @"Server=.\sqlexpress;Database=Tests;Trusted_Connection=True;";
		private readonly PersonTable table;
		
		public SqlServerTest()
		{
			table = new PersonTable();
		}
		
		[Test]
		public void PersonCountTest()
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();

				int count = Convert.ToInt32(conn.GetScalar("select count(*) from person"));
                
				int i = table.GetTable().Count;

				Assert.AreEqual(count, i);
			}
		}

		[Test]
		public void CheckPersonWithIdOne()
		{
			IPerson person = table.GetTable().First(p => p.Id == 1);
			
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();

				using (IDataReader dr = conn.GetDataReader("select Id,Name from person where Id = @Id",
					                        new Dictionary<string,object>()
					{
						{ "@Id",1 }
					}))
				{
					if (dr.Read())
					{
						Assert.AreEqual(dr.ConvertToInt32("Id"), person.Id);
						Assert.AreEqual(dr.ConvertToString("Name"), person.Name);
					}
				}
			}
		}
	}
}
