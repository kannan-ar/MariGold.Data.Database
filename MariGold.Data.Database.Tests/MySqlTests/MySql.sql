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

INSERT INTO user(UserId, UserName) VALUES(1,'User1');
INSERT INTO user(UserId, UserName) VALUES(2,'User2');

CREATE TABLE `employee` (
  `EmployeeId` int(11),
  `EmployeeName` varchar(50),
  `UserId` int(11)
)

INSERT INTO EMPLOYEE(EmployeeId, EmployeeName, UserId) VALUES(1, 'Employee1', 1);
INSERT INTO EMPLOYEE(EmployeeId, EmployeeName, UserId) VALUES(2, 'Employee2', 2);

ALTER TABLE user ADD SessionId int(11);
UPDATE user SET SessionId = 1;

CREATE TABLE `definition` (
	`DefinitionId` int(11),
	`DefinitionName` varchar(50),
	PRIMARY KEY (`DefinitionId`)
)

INSERT INTO definition(DefinitionId, DefinitionName) VALUES(1, 'Definition1');
INSERT INTO definition(DefinitionId, DefinitionName) VALUES(2, 'Definition2');

CREATE TABLE `Revision` (
	`RevisionId` int(11),
	`EmployeeId` int(11),
	`RevisionName` varchar(50),
	`RevisionDate` datetime,
	`NextRevisionDate` datetime,
	PRIMARY KEY (`RevisionId`)
);

INSERT INTO Revision(RevisionId,EmployeeId,RevisionName,RevisionDate,NextRevisionDate) VALUES(1,1,'Revision1','20180101','20190101');
INSERT INTO Revision(RevisionId,EmployeeId,RevisionName,RevisionDate,NextRevisionDate) VALUES(2,1,'Revision2','20170710','20180710');
INSERT INTO Revision(RevisionId,EmployeeId,RevisionName,RevisionDate,NextRevisionDate) VALUES(3,2,'Revision3','20160503','20170503');
INSERT INTO Revision(RevisionId,EmployeeId,RevisionName,RevisionDate,NextRevisionDate) VALUES(4,2,'Revision4','20151008','20161008');

CREATE TABLE `RevisionDetails` (
	`RevisionId` int(11),
	`DefinitionId` int(11),
	`Amount` decimal(18, 0)
);

INSERT INTO RevisionDetails(RevisionId, DefinitionId, Amount) VALUES(1,1,10);
INSERT INTO RevisionDetails(RevisionId, DefinitionId, Amount) VALUES(1,2,20);
INSERT INTO RevisionDetails(RevisionId, DefinitionId, Amount) VALUES(2,1,30);
INSERT INTO RevisionDetails(RevisionId, DefinitionId, Amount) VALUES(2,2,40);
INSERT INTO RevisionDetails(RevisionId, DefinitionId, Amount) VALUES(3,1,50);
INSERT INTO RevisionDetails(RevisionId, DefinitionId, Amount) VALUES(3,2,60);
INSERT INTO RevisionDetails(RevisionId, DefinitionId, Amount) VALUES(4,1,70);
INSERT INTO RevisionDetails(RevisionId, DefinitionId, Amount) VALUES(4,2,80);
