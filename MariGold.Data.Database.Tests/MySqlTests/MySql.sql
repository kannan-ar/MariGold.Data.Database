CREATE TABLE `person` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) DEFAULT NULL,
  `DateOfBirth` datetime DEFAULT NULL,
  `SSN` bigint(10) DEFAULT NULL,
  `BankAccount` decimal(18,0) DEFAULT NULL,
  `NoofCars` smallint(8) DEFAULT NULL,
  `IsPremium` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


INSERT Person (Id, Name, DateOfBirth, SSN, BankAccount, NoofCars, IsPremium) VALUES (1, N'James', '19731101', 1000001, 75000.00, 2, 0)
INSERT Person (Id, Name, DateOfBirth, SSN, BankAccount, NoofCars, IsPremium) VALUES (2, N'Tomy', '19840617', 1000002, 115000.00, 5, 1)
INSERT Person (Id, Name, DateOfBirth, SSN, BankAccount, NoofCars, IsPremium) VALUES (3, N'Max', '19950119', 1000007, 500.00, 0, 0)
INSERT Person (Id, Name, DateOfBirth, SSN, BankAccount, NoofCars, IsPremium) VALUES (4, N'Matthew', '19651212', 1000010, 50000.00, 0, 1)
INSERT Person (Id, Name, DateOfBirth, SSN, BankAccount, NoofCars, IsPremium) VALUES (5, N'Thomas', '20010210', 1000011, 150000.00, 1, 1)

CREATE TABLE `user` (
  `UserId` int(11),
  `UserName` varchar(50)
)

INSERT INTO user(UserId, UserName) VALUES(1,'User1')

CREATE TABLE `employee` (
  `EmployeeId` int(11),
  `EmployeeName` varchar(50),
  `UserId` int(11)
)

INSERT INTO EMPLOYEE(EmployeeId, EmployeeName, UserId) VALUES(1, 'Employee1', 1)
