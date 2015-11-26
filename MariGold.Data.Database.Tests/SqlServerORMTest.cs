namespace MariGold.Data.Database.Tests
{
	using System;
	using NUnit.Framework;
	using MariGold.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Collections.Generic;

	[TestFixture]
	public class SqlServerORMTest
	{
		private const string connectionString = @"Server=.\sqlexpress;Database=Tests;Trusted_Connection=True;";
		private readonly PersonTable table;

		public SqlServerORMTest()
		{
			table = new PersonTable();
		}
		
		[Test]
		public void TestPersonWithIdIsOne()
		{
			IPerson mockPerson = table.GetTable().First(p => p.Id == 1);
        	
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();

				var person = conn.Get<Person>("select Id,Name from person where Id = @Id",
					                         new Dictionary<string,object>()
					{
						{ "@Id",1 }
					});

				Assert.IsNotNull(person);

				Assert.AreEqual(mockPerson.Id, person.Id);
				Assert.AreEqual(mockPerson.Name, person.Name);
			}
		}
	}
}
