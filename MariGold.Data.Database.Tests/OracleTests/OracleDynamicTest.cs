namespace MariGold.Data.Database.Tests.OracleTests
{
    using System;
    using NUnit.Framework;
    using MariGold.Data;
    using Oracle.ManagedDataAccess.Client;
    using System.Linq;

    [TestFixture]
    public class OracleDynamicTest
    {
        private readonly PersonTable table;

        public OracleDynamicTest()
        {
            table = new PersonTable();
        }

        [Test]
        public void TestIdWithOne()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get("select \"Id\",\"Name\" from person where \"Id\" = :Id",
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

            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get("select * from person where \"Id\" = :Id",
                    new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.Id);
                Assert.AreEqual(mockPerson.Name, person.Name);
                Assert.AreEqual(mockPerson.DateOfBirth, person.DateOfBirth);
                Assert.AreEqual(mockPerson.SSN, person.SSN);
                Assert.AreEqual(mockPerson.BankAccount, person.BankAccount);
                Assert.AreEqual(mockPerson.NoofCars, person.NoofCars);
                Assert.AreEqual(mockPerson.IsPremium ? 1 : 0, person.IsPremium);
            }
        }
    }
}
