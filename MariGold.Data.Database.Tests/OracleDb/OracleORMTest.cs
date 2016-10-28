﻿namespace MariGold.Data.Database.Tests.OracleDb
{
    using System;
    using NUnit.Framework;
    using MariGold.Data;
    using Oracle.ManagedDataAccess.Client;
    using System.Linq;
    using System.Collections.Generic;

    [TestFixture]
    public class OracleORMTest
    {
        private readonly PersonTable table;

        public OracleORMTest()
        {
            table = new PersonTable();
        }

        [Test]
        public void TestPersonWithIdIsOne()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get<Person>("select Id,Name from person where Id = :Id",
                    new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.Id);
                Assert.AreEqual(mockPerson.Name, person.Name);
            }
        }

        [Test]
        public void GetAllWithId5()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 5);

            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get<Person>("select Id,Name,DateOfBirth,SSN,BankAccount,NoofCars from person where Id = :Id",
                    new { Id = 5 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.Id);
                Assert.AreEqual(mockPerson.Name, person.Name);
                Assert.AreEqual(mockPerson.DateOfBirth, person.DateOfBirth);
                Assert.AreEqual(mockPerson.SSN, person.SSN);
                Assert.AreEqual(mockPerson.BankAccount, person.BankAccount);
                Assert.AreEqual(mockPerson.NoofCars, person.NoofCars);
                //Assert.AreEqual(mockPerson.IsPremium, person.IsPremium);
            }
        }

        [Test]
        public void CheckPersonWithIdGreaterThan2AndLessThan4()
        {
            List<IPerson> mockPersons = table.GetTable().Where(p => p.Id > 2 && p.Id < 4).ToList();

            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
            {
                conn.Open();

                IList<Person> persons = conn.GetList<Person>("select Id,Name from person where Id > :from_id and Id < :to_id",
                    new { from_id = 2, to_id = 4 });
            }
        }

        [Test]
        public void CheckPersonWithNameLikeM()
        {
            List<IPerson> mockPersons = table.GetTable().Where(p => p.Name.StartsWith("M")).ToList();

            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
            {
                conn.Open();

                var persons = conn.GetList<Person>("select Id,Name from person where Name like 'M%'");

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

            using (OracleConnection conn = new OracleConnection(OracleUtility.ConnectionString))
            {
                conn.Open();

                var people = conn.GetEnumerable<Person>("select Id,Name,DateOfBirth,SSN,BankAccount,NoofCars from person");

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
                    //Assert.AreEqual(mockPersons[i].IsPremium, person.IsPremium);

                    i++;
                }
            }

        }
    }
}
