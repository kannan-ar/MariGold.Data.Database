namespace MariGold.Data.Database.Tests
{
    using System;
    using NUnit.Framework;
    using MariGold.Data;
    using System.Data.SqlClient;
    using System.Linq;

    [TestFixture]
    public class SqlServerORMTest
    {
        private string connectionString = @"Server=.\sqlexpress;Database=Test;Trusted_Connection=True;";

        [Test]
        public void TestPersonWithIdIsOne()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var person = conn.Get<Person>("select Id,Name from Person where Id = 1");

                Assert.IsNotNull(person);

                if (person != null)
                {
                    Assert.AreEqual(person.Id, 1);
                    Assert.AreEqual(person.Name, "One");
                }
            }
        }

        [Test]
        public void TestPersonIdWithIdIsOne()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var person = conn.Get<Person>("select Id from Person where Id = 1");

                Assert.IsNotNull(person);

                if (person != null)
                {
                    Assert.AreEqual(person.Id, 1);
                    Assert.IsNull(person.Name);
                }
            }
        }

        [Test]
        public void TestPersonCount()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var lst1 = conn.GetList<Person>("select * from person");
                var lst2 = Person.GetMokeDb();

                Assert.IsNotNull(lst1);
                Assert.IsNotNull(lst2);

                if (lst1 != null && lst2 != null)
                {
                    Assert.AreEqual(lst1.Count, lst2.Count);
                }
            }
        }

        [Test]
        public void ComparePersonList()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var lst1 = conn.GetList<Person>("select * from person");
                var lst2 = Person.GetMokeDb();

                Assert.IsNotNull(lst1);
                Assert.IsNotNull(lst2);

                if (lst1 != null && lst2 != null)
                {
                    Assert.IsTrue(Person.ComparePersons(lst1, lst2));
                }
            }
        }

        [Test]
        public void ComparePersonEnumerable()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var lst1 = conn.GetEnumerable<Person>("select * from person");
                var lst2 = Person.GetMokeDb();

                Assert.IsNotNull(lst1);
                Assert.IsNotNull(lst2);

                if (lst1 != null && lst2 != null)
                {
                    Assert.IsTrue(Person.ComparePersons(lst1.ToList(), lst2));
                }
            }
        }
    }
}
