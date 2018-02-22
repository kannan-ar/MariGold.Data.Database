namespace MariGold.Data.Database.Tests.PostgresTests
{
    using System;
    using NUnit.Framework;
    using Data;
    using Npgsql;

    [TestFixture]
    public class PostgresMultiLevelTest
    {
        [Test]
        public void EmployeeOnlyListTest()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var employees = conn.Query<Employee>("Select * From employee e Left Outer Join \"user\" u On e.user_id = u.user_id Left Outer Join revision r On e.employee_id = r.employee_id Left Outer Join revision_details rd On r.revision_id = rd.revision_id Left Outer Join definition d On rd.definition_id = d.definition_id").GetList();

                Assert.IsNotNull(employees);
                Assert.AreEqual(8, employees.Count);
            }
        }

        [Test]
        public void EmployeeOnlyListGroupTest()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var employees = conn.Query<Employee>("Select * From employee e Left Outer Join \"user\" u On e.user_id = u.user_id Left Outer Join revision r On e.employee_id = r.employee_id Left Outer Join revision_details rd On r.revision_id = rd.revision_id Left Outer Join definition d On rd.definition_id = d.definition_id").Group(e => e.EmployeeId).GetList();

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
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var employees = conn.Query<Employee>("Select * From employee e Left Outer Join \"user\" u On e.user_id = u.user_id Left Outer Join revision r On e.employee_id = r.employee_id Left Outer Join revision_details rd On r.revision_id = rd.revision_id Left Outer Join definition d On rd.definition_id = d.definition_id")
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
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var employees = conn.Query<Employee>("Select * From employee e Left Outer Join \"user\" u On e.user_id = u.user_id Left Outer Join revision r On e.employee_id = r.employee_id Left Outer Join revision_details rd On r.revision_id = rd.revision_id Left Outer Join definition d On rd.definition_id = d.definition_id")
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
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var employees = conn.Query<Employee>("Select * From employee e Left Outer Join \"user\" u On e.user_id = u.user_id Left Outer Join revision r On e.employee_id = r.employee_id Left Outer Join revision_details rd On r.revision_id = rd.revision_id Left Outer Join definition d On rd.definition_id = d.definition_id")
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
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var employees = conn.Query<Employee>("Select * From employee e Left Outer Join \"user\" u On e.user_id = u.user_id Left Outer Join revision r On e.employee_id = r.employee_id Left Outer Join revision_details rd On r.revision_id = rd.revision_id Left Outer Join definition d On rd.definition_id = d.definition_id")
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
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var employees = conn.Query<Employee>("Select * From employee e Left Outer Join \"user\" u On e.user_id = u.user_id Left Outer Join revision r On e.employee_id = r.employee_id Left Outer Join revision_details rd On r.revision_id = rd.revision_id Left Outer Join definition d On rd.definition_id = d.definition_id")
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
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var employees = conn.Query<Employee>("Select * From employee e Left Outer Join \"user\" u On e.user_id = u.user_id Left Outer Join revision r On e.employee_id = r.employee_id Left Outer Join revision_details rd On r.revision_id = rd.revision_id Left Outer Join definition d On rd.definition_id = d.definition_id")
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
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var employees = conn.Query<Employee>("Select * From employee e Left Outer Join \"user\" u On e.user_id = u.user_id Left Outer Join revision r On e.employee_id = r.employee_id Left Outer Join revision_details rd On r.revision_id = rd.revision_id Left Outer Join definition d On rd.definition_id = d.definition_id")
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
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var revisions = conn.Query<Revision>(@"Select r.revision_id, revision_name, revision_date, next_revision_date, e.employee_id, e.employee_name, d.definition_id, definition_name, amount, rv.employee_id reviser_id, rv.employee_name reviser_name From  revision r
                    Left Outer Join employee e On r.employee_id = e.employee_id
                    Left Outer Join employee rv On r.revised_by = rv.employee_id
                    Left Outer Join revision_details rd On r.revision_id = rd.revision_id
                    Left Outer Join definition d On rd.definition_id = d.definition_id")
                    .Group(r => r.RevisionId)
                    .Single<Employee>(e => e.Employee)
                    .Single<Employee>(r => r.RevisedBy, (m) => m.Map("reviser_id", r => r.EmployeeId).Map("reviser_name", r => r.EmployeeName))
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
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var employees = conn.Query<Employee>("Select e.employee_id, employee_name, revision_id as rev_id, revision_name as rev_name From employee e Inner Join revision r On e.employee_id = r.employee_id Order By employee_id, rev_id")
                    .Group(e => e.EmployeeId)
                    .Many<Revision>(e => e.Revisions, filter => filter(e => e.EmployeeId), null, (m) => m.Map("rev_id", r => r.RevisionId).Map("rev_name", r => r.RevisionName))
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
