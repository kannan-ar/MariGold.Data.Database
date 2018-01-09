namespace MariGold.Data.Database.Tests
{
    using System;
    using NSubstitute;
    using System.Collections.Generic;

    public class EmployeeTable
    {
        private IEmployee<IUser> GetEmployee(int employeeId, string employeeName)
        {
            var employee = Substitute.For<IEmployee<IUser>>();

            employee.EmployeeId.Returns(employeeId);
            employee.EmployeeName.Returns(employeeName);
            employee.User.Returns(default(IUser));

            return employee;
        }

        private IEmployee<IUser> GetEmployee(int employeeId, string employeeName, int userId, string userName, int? sessionId)
        {
            var employee = GetEmployee(employeeId, employeeName);
            var user = Substitute.For<IUser>();

            user.UserId.Returns(userId);
            user.UserName.Returns(userName);
            user.SessionId.Returns(sessionId);
            employee.User.Returns<IUser>(user);

            return employee;
        }

        public List<IEmployee<IUser>> GetTable()
        {
            var table = Substitute.For<List<IEmployee<IUser>>>();

            table.Add(GetEmployee(1, "Employee1"));

            return table;
        }

        public List<IEmployee<IUser>> GetFullTable()
        {
            var table = Substitute.For<List<IEmployee<IUser>>>();

            table.Add(GetEmployee(1, "Employee1", 1, "User1", 1));

            return table;
        }
    }
}
