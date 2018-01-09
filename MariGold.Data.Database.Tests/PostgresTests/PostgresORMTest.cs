namespace MariGold.Data.Database.Tests.PostgresTests
{
    using System;
    using NUnit.Framework;
    using MariGold.Data;
    using System.Data;
    using System.Collections.Generic;
    using Npgsql;
    using System.Linq;

    [TestFixture]
    public class PostgresORMTest
    {
        private readonly PersonTable table;
        private readonly EmployeeTable empTable;

        public PostgresORMTest()
        {
            table = new PersonTable();
            empTable = new EmployeeTable();
        }

        [Test]
        public void TestPersonWithIdIsOne()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get<Person>("select id, Name from public.person where id = @Id", new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.Id);
                Assert.AreEqual(mockPerson.Name, person.Name);
            }
        }

        [Test]
        public void GetAllWithId5()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 5);

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var person = conn.Get<Person>("select * from public.person where id = @Id", new { Id = 5 });

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

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                IList<Person> persons = conn.GetList<Person>("select id,name from public.person where id > @from_id and id < @to_id",
                    new { from_id = 2, to_id = 4 });
            }
        }

        [Test]
        public void CheckPersonWithNameLikeM()
        {
            List<IPerson> mockPersons = table.GetTable().Where(p => p.Name.StartsWith("M")).ToList();

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                var persons = conn.GetList<Person>("select id, name from public.person where name like 'M%'");

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

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                var people = conn.GetEnumerable<Person>("select * from public.person");

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
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                var person = conn.Get<Person>("Select NULL As name");

                Assert.IsNotNull(person);
                Assert.AreEqual(null, person.Name);

            }
        }

        [Test]
        public void SelectNullableDOB()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                Assert.DoesNotThrow(() =>
                {
                    conn.GetList<Person>("Select date_of_birth From public.person");
                });

            }
        }

        [Test]
        public void NullDOB()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Assert.DoesNotThrow(() =>
                {
                    conn.GetList<Person>("Select NULL as date_of_birth From public.person");
                });

            }
        }

        [Test]
        public void TestPersonWithMappingFieldAndIdIsOne()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                EntityManager<Person>.Map(p => p.Id, "ID").Map(p => p.Name, "PName").DisposeAfterUse();

                var person = conn.Get<Person>("select id as ID, name as PName from public.person where id = @Id", new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.Id, person.Id);
                Assert.AreEqual(mockPerson.Name, person.Name);
            }
        }

        [Test]
        public void TestDynamicNameList()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                var people = conn.GetList("select name from person");

                Assert.IsNotNull(people);
                Assert.AreEqual(5, people.Count);
            }
        }

        [Test]
        public void TestCamelCaseToUnderscore()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                var person = conn.Get<Person>("select date_of_birth, bank_account, no_of_cars, is_premium from public.person where id = @Id", new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.DateOfBirth, person.DateOfBirth);
                Assert.AreEqual(mockPerson.BankAccount, person.BankAccount);
                Assert.AreEqual(mockPerson.NoOfCars, person.NoOfCars);
                Assert.AreEqual(mockPerson.IsPremium, person.IsPremium);
            }
        }

        [Test]
        public void TestSwitchCamelCaseToUnderscore()
        {
            IPerson mockPerson = table.GetTable().First(p => p.Id == 1);

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = false;

                var person = conn.Get<Person>("select date_of_birth as DateOfBirth from public.person where id = @Id", new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.DateOfBirth, person.DateOfBirth);

                Config.UnderscoreToPascalCase = true;

                person = conn.Get<Person>("select date_of_birth from public.person where id = @Id", new { Id = 1 });

                Assert.IsNotNull(person);

                Assert.AreEqual(mockPerson.DateOfBirth, person.DateOfBirth);
            }
        }

        [Test]
        public void TestMultipleMaxCountScalar()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
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

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

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

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                Employee emp = conn.Query<Employee>("select * from public.employee where employee_id = 1").Get();

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

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                Employee emp = conn.Query<Employee>("select * from public.employee e inner join public.user u on e.user_id = u.user_id where employee_id = 1", e => e.User).Get();

                Assert.NotNull(emp);

                Assert.AreEqual(mockEmployee.EmployeeId, emp.EmployeeId);
                Assert.AreEqual(mockEmployee.EmployeeName, emp.EmployeeName);
                Assert.AreEqual(mockEmployee.User.UserId, emp.User.UserId);
                Assert.AreEqual(mockEmployee.User.UserName, emp.User.UserName);
            }
        }

        [Test]
        public void PascalToCamelWithEntityMapping()
        {
            var mockEmployee = empTable.GetTable().Where(e => e.EmployeeId == 1).FirstOrDefault();

            Assert.NotNull(mockEmployee);

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;
                EntityManager<Employee>.Map(e => e.EmployeeName, "EmpName").DisposeAfterUse();

                Employee emp = conn.Query<Employee>("select employee_id, employee_name from public.employee where employee_id = 1").Get();

                Assert.NotNull(emp);

                Assert.AreEqual(mockEmployee.EmployeeId, emp.EmployeeId);
                Assert.AreEqual(mockEmployee.EmployeeName, emp.EmployeeName);
                Assert.AreEqual(mockEmployee.User, emp.User);
            }
        }

        [Test]
        public void GetEmployeeWithNullUserSection()
        {
            var mockEmployee = empTable.GetFullTable().Where(e => e.EmployeeId == 1).FirstOrDefault();

            Assert.NotNull(mockEmployee);

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                Employee emp = conn.Query<Employee>("select employee_id, employee_name, u.user_id, user_name from public.employee e inner join public.user u on e.user_id = u.user_id where employee_id = 1", e => e.User).Get();

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

            using (NpgsqlConnection conn = new NpgsqlConnection(PostgresUtility.ConnectionString))
            {
                conn.Open();

                Config.UnderscoreToPascalCase = true;

                Employee emp = conn.Query<Employee>("select employee_id, employee_name, u.user_id, user_name, session_id from public.employee e inner join public.user u on e.user_id = u.user_id where employee_id = 1", e => e.User).Get();

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
