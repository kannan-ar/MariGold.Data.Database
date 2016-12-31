namespace MariGold.Data.Database.Tests.SqlServerTests
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
        private readonly PersonTable table;

        public SqlServerORMTest()
        {
            table = new PersonTable();
        }

        [Test]
        public void TestPersonWithIdIsOne()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get<Person>("Select Id,Name From PERSON Where Id = @Id", new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.Id);
                Assert.AreEqual(mockPerson.Name, person.Name);
            }
        }

        [Test]
        public void GetAllWithId5()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 5);

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get<Person>("Select * From PERSON Where Id = @Id", new { Id = 5 });

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

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                IList<Person> persons = conn.GetList<Person>("Select Id,Name From PERSON Where Id > @from and Id < @to",
                    new { from = 2, to = 4 });
            }
        }

        [Test]
        public void CheckPersonWithNameLikeM()
        {
            List<IPerson> mockPersons = table.GetTable().Where(p => p.Name.StartsWith("M")).ToList();

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                var persons = conn.GetList<Person>("Select Id,Name From PERSON Where Name like 'M%'");

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

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                var people = conn.GetEnumerable<Person>("Select * From PERSON");

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

        [Test]
        public void NullName()
        {
            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get<Person>("Select NULL As Name");

                Assert.IsNotNull(person);
                Assert.AreEqual(null, person.Name);

            }
        }

        [Test]
        public void SelectNullableDOB()
        {
            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                Assert.DoesNotThrow(() =>
                {
                    conn.GetList<Person>("Select DateOfBirth From PERSON");
                });

            }
        }

        [Test]
        public void NullDOB()
        {
            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                Assert.DoesNotThrow(() =>
                {
                    conn.GetList<Person>("Select NULL as DateOfBirth From PERSON");
                });

            }
        }

        [Test]
        public void TestPersonWithMappingFieldAndIdIsOne()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                EntityManager<Person>.Map(p => p.Id, "ID").Map(p => p.Name, "PName").DisposeAfterUse();

                var person = conn.Get<Person>("Select Id as ID, Name as PName From PERSON Where Id = @Id", new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.Id);
                Assert.AreEqual(mockPerson.Name, person.Name);
            }
        }
    }
}
