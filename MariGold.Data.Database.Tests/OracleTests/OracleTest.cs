namespace MariGold.Data.Database.Tests.OracleTests
{
    using System;
    using NUnit.Framework;
    using MariGold.Data;
    using System.Data;
    using System.Collections.Generic;
    using Oracle.ManagedDataAccess.Client;
    using System.Linq;

    [TestFixture]
    public class OracleTest
    {
        private readonly PersonTable table;

        public OracleTest()
        {
            table = new PersonTable();
        }

        [Test]
        public void PersonCountTest()
        {
            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
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
            Person person = table.GetTable().First(p => p.Id == 1);

            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("select id, name from person where id = :Id",
                    new { Id = 1 }))
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

            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("select id, name from person where id > :from_id and id < :to_id",
                    new { from_id = 2, to_id = 4 }))
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

            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("select id, name from person where name like 'M%'"))
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
