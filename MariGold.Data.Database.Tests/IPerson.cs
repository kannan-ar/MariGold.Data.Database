namespace MariGold.Data.Database.Tests
{
	using System;

	public interface IPerson
	{
		Int32 Id{ get; set; }
		String Name{ get; set; }
		DateTime DateOfBirth{ get; set; }
		Int64 SSN{ get; set; }
		Decimal BankAccount{ get; set; }
		Int16 NoofCars{ get; set; }
		Boolean IsPremium{ get; set; }
	}
}
