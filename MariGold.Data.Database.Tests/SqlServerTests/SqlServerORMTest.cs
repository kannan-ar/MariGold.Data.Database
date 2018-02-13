namespace MariGold.Data.Database.Tests.SqlServerTests
{
    using System;
    using NUnit.Framework;
    using MariGold.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Collections.Generic;

    [TestFixture]
    public class SqlServerORMTest
    {
        private readonly PersonTable table;
        private readonly EmployeeTable empTable;

        public SqlServerORMTest()
        {
            table = new PersonTable();
            empTable = new EmployeeTable();
        }

        [Test]
        public void TestPersonWithIdIsOne()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get<Person>("Select Id,Name From PERSON Where Id = @Id", new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.Id);
                Assert.AreEqual(mockPerson.Name, person.Name);
            }
        }

        [Test]
        public void GetAllWithId5()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 5);

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get<Person>("Select * From PERSON Where Id = @Id", new { Id = 5 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.Id);
                Assert.AreEqual(mockPerson.Name, person.Name);
                Assert.AreEqual(mockPerson.DateOfBirth, person.DateOfBirth);
                Assert.AreEqual(mockPerson.SSN, person.SSN);
                Assert.AreEqual(mockPerson.BankAccount, person.BankAccount);
                Assert.AreEqual(mockPerson.NoOfCars, person.NoOfCars);
                Assert.AreEqual(mockPerson.IsPremium, person.IsPremium);
            }
        }

        [Test]
        public void CheckPersonWithIdGreaterThan2AndLessThan4()
        {
            List<IPerson> mockPersons = table.GetTable().Where(p => p.Id > 2 && p.Id < 4).ToList();

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                IList<Person> persons = conn.GetList<Person>("Select Id,Name From PERSON Where Id > @from and Id < @to",
                    new { from = 2, to = 4 });
            }
        }

        [Test]
        public void CheckPersonWithNameLikeM()
        {
            List<IPerson> mockPersons = table.GetTable().Where(p => p.Name.StartsWith("M")).ToList();

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                var persons = conn.GetList<Person>("Select Id,Name From PERSON Where Name like 'M%'");

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

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                var people = conn.GetEnumerable<Person>("Select * From PERSON");

                Assert.AreEqual(mockPersons.Count, people.Count());

                int i = 0;

                foreach (IPerson person in people)
                {
                    Assert.AreEqual(mockPersons[i].Id, person.Id);
                    Assert.AreEqual(mockPersons[i].Name, person.Name);
                    Assert.AreEqual(mockPersons[i].DateOfBirth, person.DateOfBirth);
                    Assert.AreEqual(mockPersons[i].SSN, person.SSN);
                    Assert.AreEqual(mockPersons[i].BankAccount, person.BankAccount);
                    Assert.AreEqual(mockPersons[i].NoOfCars, person.NoOfCars);
                    Assert.AreEqual(mockPersons[i].IsPremium, person.IsPremium);

                    i++;
                }
            }
        }

        [Test]
        public void NullName()
        {
            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get<Person>("Select NULL As Name");

                Assert.IsNotNull(person);
                Assert.AreEqual(null, person.Name);
            }
        }

        [Test]
        public void SelectNullableDOB()
        {
            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                Assert.DoesNotThrow(() =>
                {
                    conn.GetList<Person>("Select DateOfBirth From PERSON");
                });
            }
        }

        [Test]
        public void NullDOB()
        {
            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                Assert.DoesNotThrow(() =>
                {
                    conn.GetList<Person>("Select NULL as DateOfBirth From PERSON");
                });
            }
        }

        [Test]
        public void TestPersonWithMappingFieldAndIdIsOne()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                EntityManager<Person>.Map(p => p.Id, "ID").Map(p => p.Name, "PName").DisposeAfterUse();

                var person = conn.Get<Person>("Select Id as ID, Name as PName From PERSON Where Id = @Id", new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.Id);
                Assert.AreEqual(mockPerson.Name, person.Name);
            }
        }

        [Test]
        public void TestDynamicNameList()
        {
            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                var people = conn.GetList("select Name from Person");

                Assert.IsNotNull(people);
                Assert.AreEqual(5, people.Count);
            }
        }

        [Test]
        public void TestMultipleMaxCountScalar()
        {
            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                IRecordSet record = conn.QueryMultiple(@"Select Max(Id)+10 From person;Select Count(Id) From person");

                Assert.AreEqual(15, record.GetScalar());
                Assert.AreEqual(5, record.GetScalar());
            }
        }

        [Test]
        public void GetMultipleAllAndCountList()
        {
            var mockPersons = table.GetTable();

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                IRecordSet record = conn.QueryMultiple(@"select * from person;select count(*) from person");

                var people = record.GetList<Person>();

                Assert.AreEqual(mockPersons.Count, people.Count());

                int i = 0;

                foreach (IPerson person in people)
                {
                    Assert.AreEqual(mockPersons[i].Id, person.Id);
                    Assert.AreEqual(mockPersons[i].Name, person.Name);
                    Assert.AreEqual(mockPersons[i].DateOfBirth, person.DateOfBirth);
                    Assert.AreEqual(mockPersons[i].SSN, person.SSN);
                    Assert.AreEqual(mockPersons[i].BankAccount, person.BankAccount);
                    Assert.AreEqual(mockPersons[i].NoOfCars, person.NoOfCars);
                    Assert.AreEqual(mockPersons[i].IsPremium, person.IsPremium);

                    i++;
                }

                Assert.AreEqual(5, record.GetScalar());
            }
        }

        [Test]
        public void GetEmployeeOnly()
        {
            var mockEmployee = empTable.GetTable().Where(e => e.EmployeeId == 1).FirstOrDefault();

            Assert.NotNull(mockEmployee);

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                Employee emp = conn.Query<Employee>("select * from employee where employeeid = 1").Get();

                Assert.NotNull(emp);

                Assert.AreEqual(mockEmployee.EmployeeId, emp.EmployeeId);
                Assert.AreEqual(mockEmployee.EmployeeName, emp.EmployeeName);
                Assert.AreEqual(mockEmployee.User, emp.User);
            }
        }

        [Test]
        public void GetEmployeeWithUser()
        {
            var mockEmployee = empTable.GetFullTable().Where(e => e.EmployeeId == 1).FirstOrDefault();

            Assert.NotNull(mockEmployee);

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                Employee emp = conn.Query<Employee>("select EmployeeId,EmployeeName,u.UserId,UserName from employee e inner join [user] u on e.userid = u.userid where employeeid = 1")
                    .Single<User>(e => e.User).Get();

                Assert.NotNull(emp);
                Assert.AreEqual(mockEmployee.EmployeeId, emp.EmployeeId);
                Assert.AreEqual(mockEmployee.EmployeeName, emp.EmployeeName);
                Assert.AreEqual(mockEmployee.User.UserId, emp.User.UserId);
                Assert.AreEqual(mockEmployee.User.UserName, emp.User.UserName);
            }
        }

        [Test]
        public void GetEmployeeWithNullUserSection()
        {
            var mockEmployee = empTable.GetFullTable().Where(e => e.EmployeeId == 1).FirstOrDefault();

            Assert.NotNull(mockEmployee);

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                Employee emp = conn.Query<Employee>("select EmployeeId, EmployeeName, u.UserId, UserName from employee e inner join [user] u on e.userid = u.userid where employeeid = 1")
                    .Single<User>(e => e.User).Get();

                Assert.NotNull(emp);
                Assert.AreEqual(mockEmployee.EmployeeId, emp.EmployeeId);
                Assert.AreEqual(mockEmployee.EmployeeName, emp.EmployeeName);
                Assert.AreEqual(mockEmployee.User.UserId, emp.User.UserId);
                Assert.AreEqual(mockEmployee.User.UserName, emp.User.UserName);
                Assert.IsNull(emp.User.SessionId);
            }
        }

        [Test]
        public void GetEmployeeWithUserSection()
        {
            var mockEmployee = empTable.GetFullTable().Where(e => e.EmployeeId == 1).FirstOrDefault();

            Assert.NotNull(mockEmployee);

            using (SqlConnection conn = new SqlConnection(SqlServerUtility.ConnectionString))
            {
                conn.Open();

                Employee emp = conn.Query<Employee>("select EmployeeId, EmployeeName, u.UserId, UserName, SessionId from employee e inner join [user] u on e.userid = u.userid where employeeid = 1")
                    .Single<User>(e => e.User).Get();

                Assert.NotNull(emp);
                Assert.AreEqual(mockEmployee.EmployeeId, emp.EmployeeId);
                Assert.AreEqual(mockEmployee.EmployeeName, emp.EmployeeName);
                Assert.AreEqual(mockEmployee.User.UserId, emp.User.UserId);
                Assert.AreEqual(mockEmployee.User.UserName, emp.User.UserName);
                Assert.AreEqual(mockEmployee.User.SessionId, emp.User.SessionId);
            }
        }
    }
}
