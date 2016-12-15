namespace MariGold.Data.Database.Tests
{
	using System;
	
	public class Person : IPerson
	{
		public Int32 Id{ get; set; }
		public String Name{ get; set; }
		public DateTime? DateOfBirth{ get; set; }
		public Int64 SSN{ get; set; }
		public Decimal BankAccount{ get; set; }
		public Int16 NoofCars{ get; set; }
		public Boolean IsPremium{ get; set; }
	}
}
