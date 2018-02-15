namespace MariGold.Data.Database.Tests
{
	using System;
	
	public class Person
	{
		public virtual Int32 Id{ get; set; }
		public virtual String Name{ get; set; }
		public virtual DateTime? DateOfBirth{ get; set; }
		public virtual Int64 SSN{ get; set; }
		public virtual Decimal BankAccount{ get; set; }
		public virtual Int16 NoOfCars{ get; set; }
		public virtual Boolean IsPremium{ get; set; }
	}
}
