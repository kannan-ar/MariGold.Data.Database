namespace MariGold.Data.Database.Tests
{
    public class Employee: IEmployee<User>
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public User User { get; set; }
    }
}
