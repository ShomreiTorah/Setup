--This file creates the core tables used by most Shomrei Torah applications.  
--This includes the master directory and the email list.

IF schema_id('Data') IS NULL
	EXECUTE('create schema Data');
GO
CREATE FUNCTION Data.StripPhone(@phone VARCHAR(20)) 
	RETURNS VARCHAR(10)
AS
BEGIN
	RETURN REPLACE(REPLACE(REPLACE(REPLACE(@phone, '-', ''), ' ', ''), '(', ''), ')', '');
END
GO
CREATE FUNCTION Data.FormatPhone(@phone VARCHAR(20)) 
	RETURNS VARCHAR(16)
AS
BEGIN
	DECLARE @strippedPhone VARCHAR(10)
	SET @strippedPhone = StripPhone(@phone)
	
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
	
CREATE TABLE Data.Relatives (
	RowId			UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	MemberId		UNIQUEIDENTIFIER	NOT NULL	REFERENCES Data.MasterDirectory(Id),
	RelativeId		UNIQUEIDENTIFIER	NOT NULL	REFERENCES Data.MasterDirectory(Id),
	Relation		NVARCHAR(64)		NOT NULL,
	[RowVersion]	RowVersion
);

--This table maintains backwards compatibility with our very old WebWiz system
CREATE TABLE dbo.tblMLMembers(
	Mail_ID			INT					NOT NULL	IDENTITY(1,1) PRIMARY KEY,
	[Name]			NVARCHAR(100)		COLLATE SQL_Latin1_General_CP1_CI_AS	NULL,
	Email			NVARCHAR(100)		COLLATE SQL_Latin1_General_CP1_CI_AS	NOT NULL,
	ID_Code			NVARCHAR(20)		COLLATE SQL_Latin1_General_CP1_CI_AS	NULL,
	Password		NVARCHAR(50)		COLLATE SQL_Latin1_General_CP1_CI_AS	NULL,
	Salt			NVARCHAR(30)		COLLATE SQL_Latin1_General_CP1_CI_AS	NULL,
	Active			BIT					NOT NULL	DEFAULT (0),
	Join_Date		DATETIME			NOT NULL	DEFAULT (getdate()),
	HTMLformat		BIT					NOT	NULL	DEFAULT (0),
	PersonId		UNIQUEIDENTIFIER	NULL		REFERENCES Data.MasterDirectory(Id),
	[RowId]			UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL UNIQUE DEFAULT(newid()),
	[RowVersion]	RowVersion
);