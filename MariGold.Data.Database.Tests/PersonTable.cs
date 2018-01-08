namespace MariGold.Data.Database.Tests
{
	using System;
	using NSubstitute;
	using System.Collections.Generic;
	
	public class PersonTable
	{
		private IPerson GetPerson(Int32 id, String name, DateTime dateOfBirth, 
			Int64 ssn, Decimal bankAccount, Int16 noofCars, Boolean isPremium)
		{
			var person = Substitute.For<IPerson>();
			
			person.Id.Returns(id);
			person.Name.Returns(name);
			person.DateOfBirth.Returns(dateOfBirth);
			person.SSN.Returns(ssn);
			person.BankAccount.Returns(bankAccount);
			person.NoOfCars.Returns(noofCars);
			person.IsPremium.Returns(isPremium);
			
			return person;
		}
		
		public List<IPerson> GetTable()
		{
			var table = Substitute.For<List<IPerson>>();
			
			table.Add(GetPerson(1, "James", new DateTime(1973, 11, 1), 1000001, 75000, 2, false));
			table.Add(GetPerson(2, "Tomy", new DateTime(1984, 6, 17), 1000002, 115000, 5, true));
			table.Add(GetPerson(3, "Max", new DateTime(1995, 1, 19), 1000007, 500, 0, false));
			table.Add(GetPerson(4, "Matthew", new DateTime(1965, 12, 12), 1000010, 50000, 0, true));
			table.Add(GetPerson(5, "Thomas", new DateTime(2001, 2, 10), 1000011, 150000, 1, true));
			
			return table;
		}
	}
}
