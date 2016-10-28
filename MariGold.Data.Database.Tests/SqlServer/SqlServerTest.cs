namespace MariGold.Data.Database.Tests.SqlServer
{
    using System;
    using NUnit.Framework;
    using MariGold.Data;
    using System.Data;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    [TestFixture]
    public class SqlServerTest
    {
        private readonly PersonTable table;

        public SqlServerTest()
        {
            table = new PersonTable();
        }

        [Test]
        public void PersonCountTest()
        {
            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                int count = Convert.ToInt32(conn.GetScalar("select count(*) from person"));

                int i = table.GetTable().Count;

                Assert.AreEqual(count, i);
            }
        }

        [Test]
        public void CheckPersonWithIdOne()
        {
            IPerson person = table.GetTable().First(p => p.Id == 1);

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("select Id,Name from person where Id = @Id", new { Id = 1 }))
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

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("select Id,Name from person where Id > @from and Id < @to",
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

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("select Id,Name from person where Name like 'M%'"))
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
