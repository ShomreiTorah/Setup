IF schema_id('Data') IS NULL
	EXECUTE('create schema Data');
IF schema_id('Billing') IS NULL
	EXECUTE('create schema Billing');
IF schema_id('Seating') IS NULL
	EXECUTE('create schema Seating');
GO
CREATE FUNCTION Data.FormatPhone(@phone VARCHAR(20)) 
	RETURNS VARCHAR(16)
AS
BEGIN
	DECLARE @strippedPhone VARCHAR(10)
	SET @strippedPhone = REPLACE(REPLACE(REPLACE(REPLACE(@phone, '-', ''), ' ', ''), '(', ''), ')', '');
	
	IF(LEN(@strippedPhone) = 7) RETURN SUBSTRING(@strippedPhone, 1, 3) + ' - ' + SUBSTRING(@strippedPhone, 4, 4);
	IF(LEN(@strippedPhone) <> 10) RETURN @phone;
	RETURN '(' + SUBSTRING(@strippedPhone, 1, 3) + ') ' + SUBSTRING(@strippedPhone, 4, 3) + ' - ' + SUBSTRING(@strippedPhone, 7, 4);
END
GO

--This is the YK directory
CREATE TABLE Data.MasterDirectory (
	Id				UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	YKID			INTEGER				NULL,
	LastName		NVARCHAR(100)		NOT NULL,
	HisName			NVARCHAR(100)		NULL,
	HerName			NVARCHAR(100)		NULL,
	FullName		NVARCHAR(100)		NOT NULL,
	Salutation		NVARCHAR(100)		NOT NULL,
	Address			NVARCHAR(100)		NULL,
	City			NVARCHAR(100)		NULL,
	State			VARCHAR(2)			NULL,
	Zip				VARCHAR(5)			NULL,
	Phone			VARCHAR(20)			NOT NULL,
	[Source]		NVARCHAR(64)		NOT NULL	DEFAULT('Manually Added'),
	[RowVersion]	RowVersion
);
	

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
	CheckNumber		VARCHAR(32)			NULL,
	Account			VARCHAR(32)			NOT NULL	DEFAULT 'Operating Fund',
	Amount			MONEY				NOT NULL,
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
	PledgeId		UNIQUEIDENTIFIER	NOT NULL	UNIQUE		REFERENCES Billing.Pledges(PledgeId) ON DELETE CASCADE,
	PaymentId		UNIQUEIDENTIFIER	NOT NULL	UNIQUE		REFERENCES Billing.Payments(PaymentsId) ON DELETE CASCADE,
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
	Notes			NVARCHAR(512)		NOT NULL,
	[RowVersion]	RowVersion
);


CREATE TABLE Data.Relatives (
	RowId			UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	MemberId		UNIQUEIDENTIFIER	NOT NULL	REFERENCES Data.MasterDirectory(Id),
	RelativeId		UNIQUEIDENTIFIER	NOT NULL	REFERENCES Data.MasterDirectory(Id),
	Relation		NVARCHAR(64)		NOT NULL,
	[RowVersion]	RowVersion
);