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
    public class PostgresTest
    {
        private readonly PersonTable table;

        public PostgresTest()
        {
            table = new PersonTable();
        }

        [Test]
        public void PersonCountTest()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();
                
                int count = Convert.ToInt32(conn.GetScalar("Select COUNT(*) From public.person"));

                int i = table.GetTable().Count;

                Assert.AreEqual(count, i);
            }
        }

        [Test]
        public void CheckPersonWithIdOne()
        {
            Person person = table.GetTable().First(p => p.Id == 1);

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("Select id, name From public.person Where id = @Id", new { Id = 1 }))
                {
                    if (dr.Read())
                    {
                        Assert.AreEqual(dr.ConvertToInt32("id"), person.Id);
                        Assert.AreEqual(dr.ConvertToString("name"), person.Name);
                    }
                }
            }
        }

        [Test]
        public void CheckPersonWithIdGreaterThan2AndLessThan4()
        {
            List<Person> persons = table.GetTable().Where(p => p.Id > 2 && p.Id < 4).ToList();

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("Select id, name From public.person Where id > @from and id < @to",
                    new { from = 2, to = 4 }))
                {

                    int i = 0;

                    while (dr.Read())
                    {
                        Assert.AreEqual(dr.ConvertToInt32("id"), persons[i].Id);
                        Assert.AreEqual(dr.ConvertToString("name"), persons[i].Name);

                        ++i;
                    }

                    Assert.AreEqual(persons.Count, i);

                }
            }
        }

        [Test]
        public void CheckPersonWithNameLikeM()
        {
            List<Person> persons = table.GetTable().Where(p => p.Name.StartsWith("M", StringComparison.InvariantCultureIgnoreCase)).ToList();

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("Select id, name From public.person Where name like 'M%'"))
                {

                    int i = 0;

                    while (dr.Read())
                    {
                        Assert.AreEqual(dr.ConvertToInt32("id"), persons[i].Id);
                        Assert.AreEqual(dr.ConvertToString("name"), persons[i].Name);

                        ++i;
                    }

                    Assert.AreEqual(persons.Count, i);

                }
            }
        }
    }
}
