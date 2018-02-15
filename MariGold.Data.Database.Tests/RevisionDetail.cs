namespace MariGold.Data.Database.Tests
{
    public class RevisionDetail
    {
        public virtual int RevisionId { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual RevisionDefinition Definition { get; set; }
    }
}
