namespace MariGold.Data.Database.Tests
{
    using System.Collections.Generic;

    public class Revision
    {
        public virtual int RevisionId { get; set; }
        public virtual int EmployeeId { get; set; }
        public virtual string RevisionName { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual int ModifiedBy { get; set; }
        public virtual RevisionPeriod RevisionPeriod { get; set; }
        public virtual List<RevisionDetail> Details { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Employee RevisedBy { get; set; }
    }
}
