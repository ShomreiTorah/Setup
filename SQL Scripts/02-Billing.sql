--This file creates the tables used by the billing system 
--and related applications (including the Journal).
--It depends on Core.

IF schema_id('Billing') IS NULL
	EXECUTE('create schema Billing');
IF schema_id('Seating') IS NULL
	EXECUTE('create schema Seating');
GO

--Type can be Aliyah, Ad, Membership, ...
--SubType can be שלישי, Gold Ad, כל הנערים, ...
--Note will appear on the bill and will be used for things like relatives.
--Comments will not appear on the bill.
CREATE TABLE Billing.Pledges (
	PledgeId		UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	PersonId		UNIQUEIDENTIFIER	NOT NULL	REFERENCES Data.MasterDirectory(Id),
	[Date]			DATETIME			NOT NULL	DEFAULT getdate(),
	Type			NVARCHAR(64)		NOT NULL,
	SubType			NVARCHAR(64)		NOT NULL,
	Account			VARCHAR(32)			NOT NULL	DEFAULT 'Operating Fund',
	Amount			MONEY				NOT NULL,
	Note			NVARCHAR(512)		NULL,
	Comments		NVARCHAR(512)		NULL,
	Modified		DATETIME			NOT NULL	DEFAULT getdate(),
	Modifier		NVARCHAR(32)		NOT NULL,
	ExternalSource	NVARCHAR(32)		NULL		DEFAULT NULL,
	ExternalID		INTEGER				NULL		DEFAULT NULL,
	[RowVersion]	RowVersion
);

CREATE TABLE Billing.Deposits (
	DepositId		UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	[Date]			DATETIME			NOT NULL,
	Number			INTEGER				NOT NULL,
	Account			VARCHAR(32)			NOT NULL	DEFAULT 'Operating Fund',
	[RowVersion]	RowVersion,
	CONSTRAINT UniqueFields UNIQUE([Date], Number, Account)
); --Records deposits into the bank
CREATE TABLE Billing.Payments (
	PaymentId		UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	PersonId		UNIQUEIDENTIFIER	NOT NULL	REFERENCES Data.MasterDirectory(Id),
	[Date]			DATETIME			NOT NULL	DEFAULT getdate(),
	Method			NVARCHAR(64)		NOT NULL,
	CheckNumber		NVARCHAR(64)			NULL,
	Account			VARCHAR(32)			NOT NULL	DEFAULT 'Operating Fund',
	Amount			MONEY				NOT NULL,
	Company			NVARCHAR(100)		NULL,
	DepositId		UNIQUEIDENTIFIER	NULL		DEFAULT NULL REFERENCES Billing.Deposits(DepositId) ON DELETE SET NULL,
	Comments		NVARCHAR(512)		NULL,
	Modified		DATETIME			NOT NULL	DEFAULT getdate(),
	Modifier		NVARCHAR(32)		NOT NULL,
	ExternalSource	NVARCHAR(32)		NULL		DEFAULT NULL,
	ExternalID		INTEGER				NULL		DEFAULT NULL,
	[RowVersion]	RowVersion
);

CREATE TABLE Billing.PledgeLinks (
	LinkId			UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	PledgeId		UNIQUEIDENTIFIER	NOT NULL				REFERENCES Billing.Pledges(PledgeId) ON DELETE CASCADE,
	PaymentId		UNIQUEIDENTIFIER	NOT NULL				REFERENCES Billing.Payments(PaymentId) ON DELETE CASCADE,
	Amount			MONEY				NOT NULL,
	[RowVersion]	RowVersion
);
GO
--This view makes manual queries much simpler
CREATE VIEW Billing.Transactions AS
	SELECT PersonId, 
		   [Date], 
		   'Pledge' AS [Type],
		   CASE SubType 
				WHEN '' THEN [Type]
				ELSE [Type] + ' / ' + SubType
		   END AS [Description],
		   Amount,
		   Account,
		   Comments,
		   Modifier,
		   Modified
		FROM Billing.Pledges
	UNION 
	SELECT PersonId, 
		   [Date], 
		   'Payment' AS [Type],
		   CASE 
				WHEN CheckNumber IS NULL OR CheckNumber='' THEN Method
				ELSE Method + ' #' + CheckNumber
		   END AS [Description],
		   -Amount AS Amount,
		   Account,
		   Comments,
		   Modifier,
		   Modified
		FROM Billing.Payments;
GO

CREATE TABLE Billing.StatementLog (
	Id				UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	PersonId		UNIQUEIDENTIFIER	NOT NULL	REFERENCES Data.MasterDirectory(Id),
	[DateGenerated]	DATETIME			NOT NULL	DEFAULT getdate(),
	Media			NVARCHAR(32)		NOT NULL,
	StatementKind	NVARCHAR(32)		NOT NULL,
	StartDate		DATETIME			NOT NULL,
	EndDate			DATETIME			NOT NULL,
	UserName		NVARCHAR(32)		NOT NULL,
	[RowVersion]	RowVersion
); --Logs all generated statements

CREATE TABLE Seating.SeatingReservations (
	Id				UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	PledgeId		UNIQUEIDENTIFIER	NOT NULL	UNIQUE		REFERENCES Billing.Pledges(PledgeId) ON DELETE CASCADE,
	MensSeats		INTEGER				NOT NULL	DEFAULT 0,
	WomensSeats		INTEGER				NOT NULL	DEFAULT 0,
	BoysSeats		INTEGER				NOT NULL	DEFAULT 0,
	GirlsSeats		INTEGER				NOT NULL	DEFAULT 0,
	Notes			NVARCHAR(2048)		NOT NULL,
	[RowVersion]	RowVersion
);

