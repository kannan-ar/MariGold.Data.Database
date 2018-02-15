namespace MariGold.Data.Database.Tests
{
    public class User
    {
        public virtual int UserId { get; set; }
        public virtual string UserName { get; set; }
        public virtual int? SessionId { get; set; }
    }
}
