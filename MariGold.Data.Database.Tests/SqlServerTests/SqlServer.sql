CREATE DATABASE Tests
GO
CREATE TABLE Person(
	Id Int NOT NULL PRIMARY KEY,
	Name VARCHAR(50) NULL,
	DateOfBirth DATETIME NULL,
	SSN BIGINT NULL,
	BankAccount DECIMAL(18, 2) NULL,
	NoofCars SMALLINT NULL,
	IsPremium BIT NULL)
GO
INSERT Person (Id, Name, DateOfBirth, SSN, BankAccount, NoofCars, IsPremium) VALUES (1, N'James', '19731101', 1000001, 75000.00, 2, 0)
INSERT Person (Id, Name, DateOfBirth, SSN, BankAccount, NoofCars, IsPremium) VALUES (2, N'Tomy', '19840617', 1000002, 115000.00, 5, 1)
INSERT Person (Id, Name, DateOfBirth, SSN, BankAccount, NoofCars, IsPremium) VALUES (3, N'Max', '19950119', 1000007, 500.00, 0, 0)
INSERT Person (Id, Name, DateOfBirth, SSN, BankAccount, NoofCars, IsPremium) VALUES (4, N'Matthew', '19651212', 1000010, 50000.00, 0, 1)
INSERT Person (Id, Name, DateOfBirth, SSN, BankAccount, NoofCars, IsPremium) VALUES (5, N'Thomas', '20010210', 1000011, 150000.00, 1, 1)
