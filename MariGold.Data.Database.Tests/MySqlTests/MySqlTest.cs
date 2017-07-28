namespace MariGold.Data.Database.Tests.MySqlTests
{
    using System;
    using NUnit.Framework;
    using MariGold.Data;
    using System.Data;
    using System.Collections.Generic;
    using System.Linq;
    using MySql.Data.MySqlClient;

    [TestFixture]
    public class MySqlTest
    {
        private readonly PersonTable table;

        public MySqlTest()
        {
            table = new PersonTable();
        }

        [Test]
        public void PersonCountTest()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                int count = Convert.ToInt32(conn.GetScalar("Select Count(*) From Person"));

                int i = table.GetTable().Count;

                Assert.AreEqual(count, i);
            }
        }

        [Test]
        public void CheckPersonWithIdOne()
        {
            IPerson person = table.GetTable().First(p => p.Id == 1);

            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
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

            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("select Id,Name from person where Id > @from_id and Id < @to_id",
                    new { from_id = 2, to_id = 4 }))
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

            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
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
