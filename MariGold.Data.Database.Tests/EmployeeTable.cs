namespace MariGold.Data.Database.Tests
{
    using System;
    using NSubstitute;
    using System.Collections.Generic;

    public class EmployeeTable
    {
        private Employee GetEmployee(int employeeId, string employeeName)
        {
            var employee = Substitute.For<Employee>();

            employee.EmployeeId.Returns(employeeId);
            employee.EmployeeName.Returns(employeeName);
            employee.User.Returns(default(User));

            return employee;
        }

        private Employee GetEmployee(int employeeId, string employeeName, int userId, string userName, int? sessionId)
        {
            var employee = GetEmployee(employeeId, employeeName);
            var user = Substitute.For<User>();

            user.UserId.Returns(userId);
            user.UserName.Returns(userName);
            user.SessionId.Returns(sessionId);
            employee.User.Returns<User>(user);

            return employee;
        }

        public List<Employee> GetTable()
        {
            var table = Substitute.For<List<Employee>>();

            table.Add(GetEmployee(1, "Employee1"));

            return table;
        }

        public List<Employee> GetFullTable()
        {
            var table = Substitute.For<List<Employee>>();

            table.Add(GetEmployee(1, "Employee1", 1, "User1", 1));

            return table;
        }
    }
}
