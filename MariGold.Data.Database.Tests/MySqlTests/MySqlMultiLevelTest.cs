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

        [Test]
        public void NonExistsFilterField()
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
                    .Many<Revision>(e => e.Revisions, filter => filter(r => r.EmployeeId, r => r.CreatedBy), group => group(r => r.RevisionId))
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

        [Test]
        public void NonExistsFilterGroupField()
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
                    .Many<Revision>(e => e.Revisions, filter => filter(r => r.EmployeeId, r => r.CreatedBy), group => group(r => r.RevisionId, r => r.ModifiedBy))
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

        [Test]
        public void EmployeeUserRevisionWithCustomMapping()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                var revisions = conn.Query<Revision>(@"Select r.RevisionId, RevisionName, RevisionDate, NextRevisionDate, e.EmployeeId, e.EmployeeName, d.DefinitionId, DefinitionName, Amount, rv.EmployeeId ReviserId, rv.EmployeeName ReviserName From  Revision r
                    Left Outer Join Employee e On r.EmployeeId = e.EmployeeId
                    Left Outer Join Employee rv On r.RevisedBy = rv.EmployeeId
                    Left Outer Join RevisionDetails rd On r.RevisionId = rd.RevisionId
                    Left Outer Join Definition d On rd.DefinitionId = d.DefinitionId")
                    .Group(r => r.RevisionId)
                    .Single<Employee>(e => e.Employee)
                    .Single<Employee>(r => r.RevisedBy, (m) => m.Map("ReviserId", r => r.EmployeeId).Map("ReviserName", r => r.EmployeeName))
                    .GetList();

                Assert.IsNotNull(revisions);
                Assert.AreEqual(4, revisions.Count);

                Assert.AreEqual(1, revisions[0].RevisionId);
                Assert.AreEqual("Revision1", revisions[0].RevisionName);
                Assert.IsNotNull(revisions[0].Employee);
                Assert.AreEqual(1, revisions[0].Employee.EmployeeId);
                Assert.AreEqual("Employee1", revisions[0].Employee.EmployeeName);
                Assert.IsNotNull(revisions[0].RevisedBy);
                Assert.AreEqual(2, revisions[0].RevisedBy.EmployeeId);
                Assert.AreEqual("Employee2", revisions[0].RevisedBy.EmployeeName);

                Assert.AreEqual(2, revisions[1].RevisionId);
                Assert.AreEqual("Revision2", revisions[1].RevisionName);
                Assert.IsNotNull(revisions[1].Employee);
                Assert.AreEqual(1, revisions[1].Employee.EmployeeId);
                Assert.AreEqual("Employee1", revisions[1].Employee.EmployeeName);
                Assert.IsNotNull(revisions[1].RevisedBy);
                Assert.AreEqual(2, revisions[1].RevisedBy.EmployeeId);
                Assert.AreEqual("Employee2", revisions[1].RevisedBy.EmployeeName);

                Assert.AreEqual(3, revisions[2].RevisionId);
                Assert.AreEqual("Revision3", revisions[2].RevisionName);
                Assert.IsNotNull(revisions[2].Employee);
                Assert.AreEqual(2, revisions[2].Employee.EmployeeId);
                Assert.AreEqual("Employee2", revisions[2].Employee.EmployeeName);
                Assert.IsNotNull(revisions[2].RevisedBy);
                Assert.AreEqual(1, revisions[2].RevisedBy.EmployeeId);
                Assert.AreEqual("Employee1", revisions[2].RevisedBy.EmployeeName);

                Assert.AreEqual(4, revisions[3].RevisionId);
                Assert.AreEqual("Revision4", revisions[3].RevisionName);
                Assert.IsNotNull(revisions[3].Employee);
                Assert.AreEqual(2, revisions[3].Employee.EmployeeId);
                Assert.AreEqual("Employee2", revisions[3].Employee.EmployeeName);
                Assert.IsNotNull(revisions[3].RevisedBy);
                Assert.AreEqual(1, revisions[3].RevisedBy.EmployeeId);
                Assert.AreEqual("Employee1", revisions[3].RevisedBy.EmployeeName);
            }
        }

        [Test]
        public void EmployeeWithCustomFieldRevisions()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlUtility.ConnectionString))
            {
                conn.Open();

                var employees = conn.Query<Employee>("Select e.EmployeeId, EmployeeName, RevisionId as RevId, RevisionName as RevName From Employee e Inner Join Revision r On e.EmployeeId = r.EmployeeId")
                    .Group(e => e.EmployeeId)
                    .Many<Revision>(e => e.Revisions, filter => filter(e => e.EmployeeId), null, (m) => m.Map("RevId", r => r.RevisionId).Map("RevName", r => r.RevisionName))
                    .GetList();

                Assert.IsNotNull(employees);
                Assert.AreEqual(2, employees.Count);

                Assert.AreEqual(1, employees[0].EmployeeId);
                Assert.AreEqual("Employee1", employees[0].EmployeeName);
                Assert.IsNotNull(employees[0].Revisions);
                Assert.AreEqual(2, employees[0].Revisions.Count);
                Assert.AreEqual(1, employees[0].Revisions[0].RevisionId);
                Assert.AreEqual("Revision1", employees[0].Revisions[0].RevisionName);
                Assert.AreEqual(2, employees[0].Revisions[1].RevisionId);
                Assert.AreEqual("Revision2", employees[0].Revisions[1].RevisionName);

                Assert.AreEqual(2, employees[1].EmployeeId);
                Assert.AreEqual("Employee2", employees[1].EmployeeName);
                Assert.IsNotNull(employees[1].Revisions);
                Assert.AreEqual(2, employees[1].Revisions.Count);
                Assert.AreEqual(3, employees[1].Revisions[0].RevisionId);
                Assert.AreEqual("Revision3", employees[1].Revisions[0].RevisionName);
                Assert.AreEqual(4, employees[1].Revisions[1].RevisionId);
                Assert.AreEqual("Revision4", employees[1].Revisions[1].RevisionName);
            }
        }
    }
}
