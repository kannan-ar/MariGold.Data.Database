namespace MariGold.Data.Database.Tests
{
    using System.Collections.Generic;

    public class Employee
    {
        public virtual int EmployeeId { get; set; }
        public virtual string EmployeeName { get; set; }
        public virtual User User { get; set; }
        public virtual List<Revision> Revisions { get; set; }
    }
}
