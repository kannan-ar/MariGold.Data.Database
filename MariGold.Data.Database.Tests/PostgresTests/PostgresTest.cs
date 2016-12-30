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
                
                int count = Convert.ToInt32(conn.GetScalar("Select COUNT(*) From public.\"Person\""));

                int i = table.GetTable().Count;

                Assert.AreEqual(count, i);
            }
        }

        [Test]
        public void CheckPersonWithIdOne()
        {
            IPerson person = table.GetTable().First(p => p.Id == 1);

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("Select \"Id\", \"Name\" From public.\"Person\" Where \"Id\" = @Id", new { Id = 1 }))
                {
                    if (dr.Read())
                    {
                        Assert.AreEqual(dr.ConvertToInt32("Id"), person.Id);
                        Assert.AreEqual(dr.ConvertToString("Name"), person.Name);
                    }
                }
            }
        }

        [Test]
        public void CheckPersonWithIdGreaterThan2AndLessThan4()
        {
            List<IPerson> persons = table.GetTable().Where(p => p.Id > 2 && p.Id < 4).ToList();

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("Select \"Id\",\"Name\" From public.\"Person\" Where \"Id\" > @from and \"Id\" < @to",
                    new { from = 2, to = 4 }))
                {

                    int i = 0;

                    while (dr.Read())
                    {
                        Assert.AreEqual(dr.ConvertToInt32("Id"), persons[i].Id);
                        Assert.AreEqual(dr.ConvertToString("Name"), persons[i].Name);

                        ++i;
                    }

                    Assert.AreEqual(persons.Count, i);

                }
            }
        }

        [Test]
        public void CheckPersonWithNameLikeM()
        {
            List<IPerson> persons = table.GetTable().Where(p => p.Name.StartsWith("M", StringComparison.InvariantCultureIgnoreCase)).ToList();

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("Select \"Id\",\"Name\" From public.\"Person\" Where \"Name\" like 'M%'"))
                {

                    int i = 0;

                    while (dr.Read())
                    {
                        Assert.AreEqual(dr.ConvertToInt32("Id"), persons[i].Id);
                        Assert.AreEqual(dr.ConvertToString("Name"), persons[i].Name);

                        ++i;
                    }

                    Assert.AreEqual(persons.Count, i);

                }
            }
        }
    }
}
