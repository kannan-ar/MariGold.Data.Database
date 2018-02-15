namespace MariGold.Data.Database.Tests.MySqlTests
{
    using System;
    using NUnit.Framework;
    using MariGold.Data;
    using MySql.Data.MySqlClient;
    using System.Linq;
    using System.Collections.Generic;

    [TestFixture]
    public class MySqlMultiLevelTest
    {
        [Test]
        public void EmployeeOnlyListTest()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                var employees = conn.Query<Employee>(@"Select * From Employee e Left Outer Join User u On e.UserId = u.UserId
                    Left Outer Join Revision r On e.EmployeeId = r.EmployeeId
                    Left Outer Join RevisionDetails rd On r.RevisionId = rd.RevisionId
                    Left Outer Join Definition d On rd.DefinitionId = d.DefinitionId").GetList();

                Assert.IsNotNull(employees);
                Assert.AreEqual(8, employees.Count);
            }
        }

        [Test]
        public void EmployeeOnlyListGroupTest()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                var employees = conn.Query<Employee>(@"Select * From Employee e Left Outer Join User u On e.UserId = u.UserId
                    Left Outer Join Revision r On e.EmployeeId = r.EmployeeId
                    Left Outer Join RevisionDetails rd On r.RevisionId = rd.RevisionId
                    Left Outer Join Definition d On rd.DefinitionId = d.DefinitionId").Group(e => e.EmployeeId).GetList();

                Assert.IsNotNull(employees);
                Assert.AreEqual(2, employees.Count);

                Assert.AreEqual(1, employees[0].EmployeeId);
                Assert.AreEqual("Employee1", employees[0].EmployeeName);
                Assert.IsNull(employees[0].User);
                Assert.IsNull(employees[0].Revisions);

                Assert.AreEqual(2, employees[1].EmployeeId);
                Assert.AreEqual("Employee2", employees[1].EmployeeName);
                Assert.IsNull(employees[1].User);
                Assert.IsNull(employees[1].Revisions);
            }
        }

        [Test]
        public void EmployeeUserListGroupTest()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                var employees = conn.Query<Employee>(@"Select * From Employee e Left Outer Join User u On e.UserId = u.UserId
                    Left Outer Join Revision r On e.EmployeeId = r.EmployeeId
                    Left Outer Join RevisionDetails rd On r.RevisionId = rd.RevisionId
                    Left Outer Join Definition d On rd.DefinitionId = d.DefinitionId")
                    .Group(e => e.EmployeeId)
                    .Single<User>(e => e.User)
                    .GetList();

                Assert.IsNotNull(employees);
                Assert.AreEqual(2, employees.Count);

                Assert.AreEqual(1, employees[0].EmployeeId);
                Assert.AreEqual("Employee1", employees[0].EmployeeName);
                Assert.IsNotNull(employees[0].User);
                Assert.IsNull(employees[0].Revisions);

                Assert.AreEqual(1, employees[0].User.UserId);
                Assert.AreEqual("User1", employees[0].User.UserName);

                Assert.AreEqual(2, employees[1].EmployeeId);
                Assert.AreEqual("Employee2", employees[1].EmployeeName);
                Assert.IsNotNull(employees[1].User);
                Assert.IsNull(employees[1].Revisions);

                Assert.AreEqual(2, employees[1].User.UserId);
                Assert.AreEqual("User2", employees[1].User.UserName);
            }
        }

        [Test]
        public void EmployeeUserRevisionListGroupTest()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                var employees = conn.Query<Employee>(@"Select * From Employee e Left Outer Join User u On e.UserId = u.UserId
                    Left Outer Join Revision r On e.EmployeeId = r.EmployeeId
                    Left Outer Join RevisionDetails rd On r.RevisionId = rd.RevisionId
                    Left Outer Join Definition d On rd.DefinitionId = d.DefinitionId")
                    .Group(e => e.EmployeeId)
                    .Single<User>(e => e.User)
                    .Many<Revision>(e => e.Revisions, filter => filter(r => r.EmployeeId), group => group(r => r.RevisionId))
                    .GetList();

                Assert.IsNotNull(employees);
                Assert.AreEqual(2, employees.Count);

                Assert.AreEqual(1, employees[0].EmployeeId);
                Assert.AreEqual("Employee1", employees[0].EmployeeName);
                Assert.IsNotNull(employees[0].User);
                Assert.IsNotNull(employees[0].Revisions);

                Assert.AreEqual(1, employees[0].User.UserId);
                Assert.AreEqual("User1", employees[0].User.UserName);

                Assert.AreEqual(2, employees[0].Revisions.Count);

                Assert.AreEqual(1, employees[0].Revisions[0].RevisionId);
                Assert.AreEqual("Revision1", employees[0].Revisions[0].RevisionName);
                Assert.IsNull(employees[0].Revisions[0].RevisionPeriod);

                Assert.AreEqual(2, employees[0].Revisions[1].RevisionId);
                Assert.AreEqual("Revision2", employees[0].Revisions[1].RevisionName);
                Assert.IsNull(employees[0].Revisions[1].RevisionPeriod);

                Assert.AreEqual(2, employees[1].EmployeeId);
                Assert.AreEqual("Employee2", employees[1].EmployeeName);
                Assert.IsNotNull(employees[1].User);
                Assert.IsNotNull(employees[1].Revisions);

                Assert.AreEqual(2, employees[1].User.UserId);
                Assert.AreEqual("User2", employees[1].User.UserName);

                Assert.AreEqual(2, employees[1].Revisions.Count);

                Assert.AreEqual(3, employees[1].Revisions[0].RevisionId);
                Assert.AreEqual("Revision3", employees[1].Revisions[0].RevisionName);
                Assert.IsNull(employees[1].Revisions[0].RevisionPeriod);

                Assert.AreEqual(4, employees[1].Revisions[1].RevisionId);
                Assert.AreEqual("Revision4", employees[1].Revisions[1].RevisionName);
                Assert.IsNull(employees[1].Revisions[1].RevisionPeriod);
            }
        }

        [Test]
        public void EmployeeUserRevisionPropertiesListGroupTest()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                var employees = conn.Query<Employee>(@"Select * From Employee e Left Outer Join User u On e.UserId = u.UserId
                    Left Outer Join Revision r On e.EmployeeId = r.EmployeeId
                    Left Outer Join RevisionDetails rd On r.RevisionId = rd.RevisionId
                    Left Outer Join Definition d On rd.DefinitionId = d.DefinitionId")
                    .Group(e => e.EmployeeId)
                    .Single<User>(e => e.User)
                    .Many<Revision>(e => e.Revisions, filter => filter(r => r.EmployeeId), group => group(r => r.RevisionId))
                    .Single<Revision, RevisionPeriod>(r => r.RevisionPeriod)
                    .GetList();

                Assert.IsNotNull(employees);
                Assert.AreEqual(2, employees.Count);

                Assert.AreEqual(1, employees[0].EmployeeId);
                Assert.AreEqual("Employee1", employees[0].EmployeeName);
                Assert.IsNotNull(employees[0].User);
                Assert.IsNotNull(employees[0].Revisions);

                Assert.AreEqual(1, employees[0].User.UserId);
                Assert.AreEqual("User1", employees[0].User.UserName);

                Assert.AreEqual(2, employees[0].Revisions.Count);

                Assert.AreEqual(1, employees[0].Revisions[0].RevisionId);
                Assert.AreEqual("Revision1", employees[0].Revisions[0].RevisionName);

                Assert.IsNotNull(employees[0].Revisions[0].RevisionPeriod);
                Assert.AreEqual(new DateTime(2018, 1, 1), employees[0].Revisions[0].RevisionPeriod.RevisionDate);
                Assert.AreEqual(new DateTime(2019, 1, 1), employees[0].Revisions[0].RevisionPeriod.NextRevisionDate);

                Assert.IsNull(employees[0].Revisions[0].Details);

                Assert.AreEqual(2, employees[0].Revisions[1].RevisionId);
                Assert.AreEqual("Revision2", employees[0].Revisions[1].RevisionName);

                Assert.IsNotNull(employees[0].Revisions[1].RevisionPeriod);
                Assert.AreEqual(new DateTime(2017, 7, 10), employees[0].Revisions[1].RevisionPeriod.RevisionDate);
                Assert.AreEqual(new DateTime(2018, 7, 10), employees[0].Revisions[1].RevisionPeriod.NextRevisionDate);

                Assert.IsNull(employees[0].Revisions[1].Details);

                Assert.AreEqual(2, employees[1].EmployeeId);
                Assert.AreEqual("Employee2", employees[1].EmployeeName);
                Assert.IsNotNull(employees[1].User);
                Assert.IsNotNull(employees[1].Revisions);

                Assert.AreEqual(2, employees[1].User.UserId);
                Assert.AreEqual("User2", employees[1].User.UserName);

                Assert.AreEqual(2, employees[1].Revisions.Count);

                Assert.AreEqual(3, employees[1].Revisions[0].RevisionId);
                Assert.AreEqual("Revision3", employees[1].Revisions[0].RevisionName);

                Assert.IsNotNull(employees[1].Revisions[0].RevisionPeriod);
                Assert.AreEqual(new DateTime(2016, 5, 3), employees[1].Revisions[0].RevisionPeriod.RevisionDate);
                Assert.AreEqual(new DateTime(2017, 5, 3), employees[1].Revisions[0].RevisionPeriod.NextRevisionDate);

                Assert.IsNull(employees[1].Revisions[0].Details);

                Assert.AreEqual(4, employees[1].Revisions[1].RevisionId);
                Assert.AreEqual("Revision4", employees[1].Revisions[1].RevisionName);

                Assert.IsNotNull(employees[1].Revisions[1].RevisionPeriod);
                Assert.AreEqual(new DateTime(2015, 10, 8), employees[1].Revisions[1].RevisionPeriod.RevisionDate);
                Assert.AreEqual(new DateTime(2016, 10, 8), employees[1].Revisions[1].RevisionPeriod.NextRevisionDate);

                Assert.IsNull(employees[1].Revisions[1].Details);
            }
        }

        [Test]
        public void EmployeeUserRevisionPropertiesDetailListGroupTest()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                var employees = conn.Query<Employee>(@"Select * From Employee e Left Outer Join User u On e.UserId = u.UserId
                    Left Outer Join Revision r On e.EmployeeId = r.EmployeeId
                    Left Outer Join RevisionDetails rd On r.RevisionId = rd.RevisionId
                    Left Outer Join Definition d On rd.DefinitionId = d.DefinitionId")
                    .Group(e => e.EmployeeId)
                    .Single<User>(e => e.User)
                    .Many<Revision>(e => e.Revisions, filter => filter(r => r.EmployeeId), group => group(r => r.RevisionId))
                    .Single<Revision, RevisionPeriod>(r => r.RevisionPeriod)
                    .Many<Revision, RevisionDetail>(r => r.Details, filter => filter(r => r.RevisionId))
                    .GetList();

                Assert.IsNotNull(employees);
                Assert.AreEqual(2, employees.Count);

                Assert.AreEqual(1, employees[0].EmployeeId);
                Assert.AreEqual("Employee1", employees[0].EmployeeName);
                Assert.IsNotNull(employees[0].User);
                Assert.IsNotNull(employees[0].Revisions);

                Assert.AreEqual(1, employees[0].User.UserId);
                Assert.AreEqual("User1", employees[0].User.UserName);

                Assert.AreEqual(2, employees[0].Revisions.Count);

                Assert.AreEqual(1, employees[0].Revisions[0].RevisionId);
                Assert.AreEqual("Revision1", employees[0].Revisions[0].RevisionName);

                Assert.IsNotNull(employees[0].Revisions[0].RevisionPeriod);
                Assert.AreEqual(new DateTime(2018, 1, 1), employees[0].Revisions[0].RevisionPeriod.RevisionDate);
                Assert.AreEqual(new DateTime(2019, 1, 1), employees[0].Revisions[0].RevisionPeriod.NextRevisionDate);

                Assert.IsNotNull(employees[0].Revisions[0].Details);
                Assert.AreEqual(2, employees[0].Revisions[0].Details.Count);

                Assert.AreEqual(10, employees[0].Revisions[0].Details[0].Amount);
                Assert.IsNull(employees[0].Revisions[0].Details[0].Definition);

                Assert.AreEqual(20, employees[0].Revisions[0].Details[1].Amount);
                Assert.IsNull(employees[0].Revisions[0].Details[1].Definition);

                Assert.AreEqual(2, employees[0].Revisions[1].RevisionId);
                Assert.AreEqual("Revision2", employees[0].Revisions[1].RevisionName);

                Assert.IsNotNull(employees[0].Revisions[1].RevisionPeriod);
                Assert.AreEqual(new DateTime(2017, 7, 10), employees[0].Revisions[1].RevisionPeriod.RevisionDate);
                Assert.AreEqual(new DateTime(2018, 7, 10), employees[0].Revisions[1].RevisionPeriod.NextRevisionDate);

                Assert.IsNotNull(employees[0].Revisions[1].Details);
                Assert.AreEqual(2, employees[0].Revisions[1].Details.Count);

                Assert.AreEqual(30, employees[0].Revisions[1].Details[0].Amount);
                Assert.IsNull(employees[0].Revisions[1].Details[0].Definition);

                Assert.AreEqual(40, employees[0].Revisions[1].Details[1].Amount);
                Assert.IsNull(employees[0].Revisions[1].Details[1].Definition);

                Assert.AreEqual(2, employees[1].EmployeeId);
                Assert.AreEqual("Employee2", employees[1].EmployeeName);
                Assert.IsNotNull(employees[1].User);
                Assert.IsNotNull(employees[1].Revisions);

                Assert.AreEqual(2, employees[1].User.UserId);
                Assert.AreEqual("User2", employees[1].User.UserName);

                Assert.AreEqual(2, employees[1].Revisions.Count);

                Assert.AreEqual(3, employees[1].Revisions[0].RevisionId);
                Assert.AreEqual("Revision3", employees[1].Revisions[0].RevisionName);

                Assert.IsNotNull(employees[1].Revisions[0].RevisionPeriod);
                Assert.AreEqual(new DateTime(2016, 5, 3), employees[1].Revisions[0].RevisionPeriod.RevisionDate);
                Assert.AreEqual(new DateTime(2017, 5, 3), employees[1].Revisions[0].RevisionPeriod.NextRevisionDate);

                Assert.IsNotNull(employees[1].Revisions[0].Details);
                Assert.AreEqual(2, employees[1].Revisions[0].Details.Count);

                Assert.AreEqual(50, employees[1].Revisions[0].Details[0].Amount);
                Assert.IsNull(employees[1].Revisions[0].Details[0].Definition);

                Assert.AreEqual(60, employees[1].Revisions[0].Details[1].Amount);
                Assert.IsNull(employees[1].Revisions[0].Details[1].Definition);

                Assert.AreEqual(4, employees[1].Revisions[1].RevisionId);
                Assert.AreEqual("Revision4", employees[1].Revisions[1].RevisionName);

                Assert.IsNotNull(employees[1].Revisions[1].RevisionPeriod);
                Assert.AreEqual(new DateTime(2015, 10, 8), employees[1].Revisions[1].RevisionPeriod.RevisionDate);
                Assert.AreEqual(new DateTime(2016, 10, 8), employees[1].Revisions[1].RevisionPeriod.NextRevisionDate);

                Assert.IsNotNull(employees[1].Revisions[1].Details);
                Assert.AreEqual(2, employees[1].Revisions[1].Details.Count);

                Assert.AreEqual(70, employees[1].Revisions[1].Details[0].Amount);
                Assert.IsNull(employees[1].Revisions[1].Details[0].Definition);

                Assert.AreEqual(80, employees[1].Revisions[1].Details[1].Amount);
                Assert.IsNull(employees[1].Revisions[1].Details[1].Definition);
            }
        }

        [Test]
        public void EmployeeUserRevisionPropertiesDetailDefinitionListGroupTest()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                var employees = conn.Query<Employee>(@"Select * From Employee e Left Outer Join User u On e.UserId = u.UserId
                    Left Outer Join Revision r On e.EmployeeId = r.EmployeeId
                    Left Outer Join RevisionDetails rd On r.RevisionId = rd.RevisionId
                    Left Outer Join Definition d On rd.DefinitionId = d.DefinitionId")
                    .Group(e => e.EmployeeId)
                    .Single<User>(e => e.User)
                    .Many<Revision>(e => e.Revisions, filter => filter(r => r.EmployeeId), group => group(r => r.RevisionId))
                    .Single<Revision, RevisionPeriod>(r => r.RevisionPeriod)
                    .Many<Revision, RevisionDetail>(r => r.Details, filter => filter(r => r.RevisionId))
                    .Single<RevisionDetail, RevisionDefinition>(rd => rd.Definition)
                    .GetList();

                Assert.IsNotNull(employees);
                Assert.AreEqual(2, employees.Count);

                Assert.AreEqual(1, employees[0].EmployeeId);
                Assert.AreEqual("Employee1", employees[0].EmployeeName);
                Assert.IsNotNull(employees[0].User);
                Assert.IsNotNull(employees[0].Revisions);

                Assert.AreEqual(1, employees[0].User.UserId);
                Assert.AreEqual("User1", employees[0].User.UserName);

                Assert.AreEqual(2, employees[0].Revisions.Count);

                Assert.AreEqual(1, employees[0].Revisions[0].RevisionId);
                Assert.AreEqual("Revision1", employees[0].Revisions[0].RevisionName);

                Assert.IsNotNull(employees[0].Revisions[0].RevisionPeriod);
                Assert.AreEqual(new DateTime(2018, 1, 1), employees[0].Revisions[0].RevisionPeriod.RevisionDate);
                Assert.AreEqual(new DateTime(2019, 1, 1), employees[0].Revisions[0].RevisionPeriod.NextRevisionDate);

                Assert.IsNotNull(employees[0].Revisions[0].Details);
                Assert.AreEqual(2, employees[0].Revisions[0].Details.Count);

                Assert.AreEqual(10, employees[0].Revisions[0].Details[0].Amount);
                Assert.IsNotNull(employees[0].Revisions[0].Details[0].Definition);
                Assert.AreEqual(1, employees[0].Revisions[0].Details[0].Definition.DefinitionId);
                Assert.AreEqual("Definition1", employees[0].Revisions[0].Details[0].Definition.DefinitionName);

                Assert.AreEqual(20, employees[0].Revisions[0].Details[1].Amount);
                Assert.IsNotNull(employees[0].Revisions[0].Details[1].Definition);
                Assert.AreEqual(2, employees[0].Revisions[0].Details[1].Definition.DefinitionId);
                Assert.AreEqual("Definition2", employees[0].Revisions[0].Details[1].Definition.DefinitionName);

                Assert.AreEqual(2, employees[0].Revisions[1].RevisionId);
                Assert.AreEqual("Revision2", employees[0].Revisions[1].RevisionName);

                Assert.IsNotNull(employees[0].Revisions[1].RevisionPeriod);
                Assert.AreEqual(new DateTime(2017, 7, 10), employees[0].Revisions[1].RevisionPeriod.RevisionDate);
                Assert.AreEqual(new DateTime(2018, 7, 10), employees[0].Revisions[1].RevisionPeriod.NextRevisionDate);

                Assert.IsNotNull(employees[0].Revisions[1].Details);
                Assert.AreEqual(2, employees[0].Revisions[1].Details.Count);

                Assert.AreEqual(30, employees[0].Revisions[1].Details[0].Amount);
                Assert.IsNotNull(employees[0].Revisions[1].Details[0].Definition);
                Assert.AreEqual(1, employees[0].Revisions[1].Details[0].Definition.DefinitionId);
                Assert.AreEqual("Definition1", employees[0].Revisions[1].Details[0].Definition.DefinitionName);

                Assert.AreEqual(40, employees[0].Revisions[1].Details[1].Amount);
                Assert.IsNotNull(employees[0].Revisions[1].Details[1].Definition);
                Assert.AreEqual(2, employees[0].Revisions[1].Details[1].Definition.DefinitionId);
                Assert.AreEqual("Definition2", employees[0].Revisions[1].Details[1].Definition.DefinitionName);

                Assert.AreEqual(2, employees[1].EmployeeId);
                Assert.AreEqual("Employee2", employees[1].EmployeeName);
                Assert.IsNotNull(employees[1].User);
                Assert.IsNotNull(employees[1].Revisions);

                Assert.AreEqual(2, employees[1].User.UserId);
                Assert.AreEqual("User2", employees[1].User.UserName);

                Assert.AreEqual(2, employees[1].Revisions.Count);

                Assert.AreEqual(3, employees[1].Revisions[0].RevisionId);
                Assert.AreEqual("Revision3", employees[1].Revisions[0].RevisionName);

                Assert.IsNotNull(employees[1].Revisions[0].RevisionPeriod);
                Assert.AreEqual(new DateTime(2016, 5, 3), employees[1].Revisions[0].RevisionPeriod.RevisionDate);
                Assert.AreEqual(new DateTime(2017, 5, 3), employees[1].Revisions[0].RevisionPeriod.NextRevisionDate);

                Assert.IsNotNull(employees[1].Revisions[0].Details);
                Assert.AreEqual(2, employees[1].Revisions[0].Details.Count);

                Assert.AreEqual(50, employees[1].Revisions[0].Details[0].Amount);
                Assert.IsNotNull(employees[1].Revisions[0].Details[0].Definition);
                Assert.AreEqual(1, employees[1].Revisions[0].Details[0].Definition.DefinitionId);
                Assert.AreEqual("Definition1", employees[1].Revisions[0].Details[0].Definition.DefinitionName);

                Assert.AreEqual(60, employees[1].Revisions[0].Details[1].Amount);
                Assert.IsNotNull(employees[1].Revisions[0].Details[1].Definition);
                Assert.AreEqual(2, employees[1].Revisions[0].Details[1].Definition.DefinitionId);
                Assert.AreEqual("Definition2", employees[1].Revisions[0].Details[1].Definition.DefinitionName);

                Assert.AreEqual(4, employees[1].Revisions[1].RevisionId);
                Assert.AreEqual("Revision4", employees[1].Revisions[1].RevisionName);

                Assert.IsNotNull(employees[1].Revisions[1].RevisionPeriod);
                Assert.AreEqual(new DateTime(2015, 10, 8), employees[1].Revisions[1].RevisionPeriod.RevisionDate);
                Assert.AreEqual(new DateTime(2016, 10, 8), employees[1].Revisions[1].RevisionPeriod.NextRevisionDate);

                Assert.IsNotNull(employees[1].Revisions[1].Details);
                Assert.AreEqual(2, employees[1].Revisions[1].Details.Count);

                Assert.AreEqual(70, employees[1].Revisions[1].Details[0].Amount);
                Assert.IsNotNull(employees[1].Revisions[1].Details[0].Definition);
                Assert.AreEqual(1, employees[1].Revisions[1].Details[0].Definition.DefinitionId);
                Assert.AreEqual("Definition1", employees[1].Revisions[1].Details[0].Definition.DefinitionName);

                Assert.AreEqual(80, employees[1].Revisions[1].Details[1].Amount);
                Assert.IsNotNull(employees[1].Revisions[1].Details[1].Definition);
                Assert.AreEqual(2, employees[1].Revisions[1].Details[1].Definition.DefinitionId);
                Assert.AreEqual("Definition2", employees[1].Revisions[1].Details[1].Definition.DefinitionName);
            }
        }
    }
}
