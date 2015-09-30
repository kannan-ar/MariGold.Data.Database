namespace MariGold.Data.Database.Tests
{
    using System;
    using NUnit.Framework;
    using MariGold.Data;
    using System.Data;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    [TestFixture]
    public class SqlServerTest
    {
        private string connectionString = @"Server=.\sqlexpress;Database=Test;Trusted_Connection=True;";

        [Test]
        public void PersonCountTest()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                int count = Convert.ToInt32(conn.GetScalar("select count(*) from person"));
                
                int i = 0;

                using (IDataReader dr = conn.GetDataReader("select * from person"))
                {
                    
                    while (dr.Read())
                    {
                        i++;
                    }
                }

                Assert.AreEqual(count, i);
            }
        }

        [Test]
        public void PersonDbMatchTest()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var lst1 = Person.GetMokeDb();
                var lst2 = new List<Person>();

                using (IDataReader dr = conn.GetDataReader("select * from person"))
                {
                    while (dr.Read())
                    {
                        lst2.Add(new Person() { Id = dr.ConvertToInt32("Id"), Name = dr.ConvertToString("Name") });
                    }
                }

                Assert.IsTrue(Person.ComparePersons(lst1, lst2));
            }
        }

        [Test]
        public void CheckPersonWithIdOne()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (IDataReader dr = conn.GetDataReader("select Id,Name from person where Id = 1"))
                {
                    if(dr.Read())
                    {
                        Assert.AreEqual(Convert.ToInt32(dr[0]), 1);
                        Assert.AreEqual(Convert.ToString(dr[1]), "One");
                    }
                }
            }
        }
    }
}
