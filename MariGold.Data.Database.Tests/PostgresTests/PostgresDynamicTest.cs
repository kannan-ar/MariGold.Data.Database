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
                
                var person = conn.Get("select id,name from person where id = @Id",
                    new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.id);
                Assert.AreEqual(mockPerson.Name, person.name);
            }
        }

        [Test]
        public void GetAllWithId1()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get("select * from public.person where id = @Id",
                    new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.id);
                Assert.AreEqual(mockPerson.Name, person.name);
                Assert.AreEqual(mockPerson.DateOfBirth, person.date_of_birth);
                Assert.AreEqual(mockPerson.SSN, person.ssn);
                Assert.AreEqual(mockPerson.BankAccount, person.bank_account);
                Assert.AreEqual(mockPerson.NoOfCars, person.no_of_cars);
                Assert.AreEqual(mockPerson.IsPremium, person.is_premium);
            }
        }
    }
}
