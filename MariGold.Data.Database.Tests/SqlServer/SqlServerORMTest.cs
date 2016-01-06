﻿namespace MariGold.Data.Database.Tests.SqlServer
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
		
		[Test]
		public void GetAllWithId5()
		{
			IPerson mockPerson = table.GetTable().First(p => p.Id == 5);
			
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();

				var person = conn.Get<Person>("select * from person where Id = @Id",
					             new Dictionary<string,object>()
					{
						{ "@Id",5 }
					});

				Assert.IsNotNull(person);

				Assert.AreEqual(mockPerson.Id, person.Id);
				Assert.AreEqual(mockPerson.Name, person.Name);
				Assert.AreEqual(mockPerson.DateOfBirth, person.DateOfBirth);
				Assert.AreEqual(mockPerson.SSN, person.SSN);
				Assert.AreEqual(mockPerson.BankAccount, person.BankAccount);
				Assert.AreEqual(mockPerson.NoofCars, person.NoofCars);
				Assert.AreEqual(mockPerson.IsPremium, person.IsPremium);
			}
		}
			
		[Test]
		public void CheckPersonWithIdGreaterThan2AndLessThan4()
		{
			List<IPerson> mockPersons = table.GetTable().Where(p => p.Id > 2 && p.Id < 4).ToList();
			
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				
				IList<Person> persons = conn.GetList<Person>("select Id,Name from person where Id > @from and Id < @to",
					                        new Dictionary<string,object>()
					{
						{ "@from",2 },
						{ "@to",4 }
					});
			}
		}
		
		[Test]
		public void CheckPersonWithNameLikeM()
		{
			List<IPerson> mockPersons = table.GetTable().Where(p => p.Name.StartsWith("M")).ToList();
			
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				
				var persons = conn.GetList<Person>("select Id,Name from person where Name like 'M%'");
				
				Assert.AreEqual(mockPersons.Count, persons.Count);
				
				for (int i = 0; mockPersons.Count > i; i++)
				{
					Assert.AreEqual(mockPersons[i].Id, persons[i].Id);
					Assert.AreEqual(mockPersons[i].Name, persons[i].Name);
				}
			}
		}
		
		[Test]
		public void GetAllEnumerable()
		{
			var mockPersons = table.GetTable();
			
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				
				var people = conn.GetEnumerable<Person>("select * from person");
				
				Assert.AreEqual(mockPersons.Count, people.Count());
				
				int i = 0;
				
				foreach (IPerson person in people) 
				{
					Assert.AreEqual(mockPersons[i].Id, person.Id);
					Assert.AreEqual(mockPersons[i].Name, person.Name);
					Assert.AreEqual(mockPersons[i].DateOfBirth, person.DateOfBirth);
					Assert.AreEqual(mockPersons[i].SSN, person.SSN);
					Assert.AreEqual(mockPersons[i].BankAccount, person.BankAccount);
					Assert.AreEqual(mockPersons[i].NoofCars, person.NoofCars);
					Assert.AreEqual(mockPersons[i].IsPremium, person.IsPremium);
					
					i++;
				}
			}
			
		}
	}
}
