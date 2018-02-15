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
            Person mockPerson = table.GetTable().First(p => p.Id == 1);

            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get("select id, name from person where id = :Id",
                    new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.ID);
                Assert.AreEqual(mockPerson.Name, person.NAME);
            }
        }

        [Test]
        public void GetAllWithId1()
        {
            Person mockPerson = table.GetTable().First(p => p.Id == 1);

            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var person = conn.Get("select * from person where id = :Id",
                    new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.ID);
                Assert.AreEqual(mockPerson.Name, person.NAME);
                Assert.AreEqual(mockPerson.DateOfBirth, person.DATE_OF_BIRTH);
                Assert.AreEqual(mockPerson.SSN, person.SSN);
                Assert.AreEqual(mockPerson.BankAccount, person.BANK_ACCOUNT);
                Assert.AreEqual(mockPerson.NoOfCars, person.NO_OF_CARS);
                Assert.AreEqual(mockPerson.IsPremium ? 1 : 0, person.IS_PREMIUM);
            }
        }
    }
}
