namespace MariGold.Data.Database.Tests.MySqlTests
{
    using System;
    using NUnit.Framework;
    using MariGold.Data;
    using MySql.Data.MySqlClient;
    using System.Linq;
    using System.Collections.Generic;

    public class MySqlDynamicTest
    {
        private readonly PersonTable table;

        public MySqlDynamicTest()
        {
            table = new PersonTable();
        }

        [Test]
        public void TestIdWithOne()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get("select Id,Name from person where Id = @Id",
                    new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.Id);
                Assert.AreEqual(mockPerson.Name, person.Name);
            }
        }

        [Test]
        public void GetAllWithId1()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get("select * from person where Id = @Id",
                    new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.Id);
                Assert.AreEqual(mockPerson.Name, person.Name);
                Assert.AreEqual(mockPerson.DateOfBirth, person.DateOfBirth);
                Assert.AreEqual(mockPerson.SSN, person.SSN);
                Assert.AreEqual(mockPerson.BankAccount, person.BankAccount);
                Assert.AreEqual(mockPerson.NoOfCars, person.NoofCars);
                Assert.AreEqual(mockPerson.IsPremium, person.IsPremium);
            }
        }
    }
}
