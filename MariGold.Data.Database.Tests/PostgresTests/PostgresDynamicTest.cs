namespace MariGold.Data.Database.Tests.PostgresTests
{
    using System;
    using NUnit.Framework;
    using MariGold.Data;
    using System.Data;
    using System.Collections.Generic;
    using Npgsql;
    using System.Linq;

    [TestFixture]
    public class PostgresDynamicTest
    {
        private readonly PersonTable table;

        public PostgresDynamicTest()
        {
            table = new PersonTable();
        }

        [Test]
        public void TestIdWithOne()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get("select \"Id\",\"Name\" from public.\"Person\" where \"Id\" = @Id",
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

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get("select * from public.\"Person\" where \"Id\" = @Id",
                    new { Id = 1 });

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
    }
}
