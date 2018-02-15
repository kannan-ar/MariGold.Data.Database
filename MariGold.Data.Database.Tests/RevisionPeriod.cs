namespace MariGold.Data.Database.Tests
{
    using System;

    public class RevisionPeriod
    {
        public virtual DateTime RevisionDate { get; set; }
        public virtual DateTime NextRevisionDate { get; set; }
    }
}
