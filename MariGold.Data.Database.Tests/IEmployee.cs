namespace MariGold.Data.Database.Tests
{
    using System;

    public interface IEmployee<T>
        where T : IUser
    {
        int EmployeeId { get; set; }
        string EmployeeName { get; set; }
        T User { get; set; }
    }
}
