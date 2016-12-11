--This file creates tables used for a staged miration to the Billing database.
--This is managed the the Migrator plugin for the billing system.
--It depends on Core and Billing.

IF schema_id('BillingMigration') IS NULL
	EXECUTE('create schema BillingMigration');
GO

CREATE TABLE BillingMigration.StagedPeople (
	StagedPersonId	UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	PersonId		UNIQUEIDENTIFIER		NULL	REFERENCES Data.MasterDirectory(Id),
	HisName			NVARCHAR(100)		NOT NULL,
	HerName			NVARCHAR(100)		NOT NULL,
	LastName		NVARCHAR(100)		NOT NULL,
	FullName		NVARCHAR(100)		NOT NULL,
	Address			NVARCHAR(100)		NOT NULL,
	City			NVARCHAR(100)		NULL,
	State			VARCHAR(32)			NULL,
	Zip				VARCHAR(10)			NULL,
	Phone			VARCHAR(20)			NOT NULL,
	[RowVersion]	RowVersion
);

CREATE TABLE BillingMigration.StagedPayments (
	StagedPaymentId	UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	StagedPersonId	UNIQUEIDENTIFIER	NOT NULL	REFERENCES BillingMigration.StagedPeople(StagedPersonId),
	[Date]			DATETIME			NOT NULL,
	Method			NVARCHAR(64)		NOT NULL,
	CheckNumber		NVARCHAR(64)			NULL,
	Account			VARCHAR(32)			NOT NULL	DEFAULT 'Operating Fund',
	Amount			MONEY				NOT NULL,
	Comments		NVARCHAR(512)		NULL,
	ExternalId		NVARCHAR(64)		NOT NULL,
	[RowVersion]	RowVersion
);
