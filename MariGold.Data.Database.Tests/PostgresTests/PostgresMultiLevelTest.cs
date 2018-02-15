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
    }
}
