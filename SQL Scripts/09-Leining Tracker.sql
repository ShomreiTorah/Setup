--This file creates the tables used by the standalone LeiningTracker site.
--It has no dependencies.

IF schema_id('Leining') IS NULL
	EXECUTE('create schema Leining');

CREATE TABLE Leining.Leiners (
	Id					UNIQUEIDENTIFIER	NOT NULL ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
    Name				NVARCHAR(128)		NOT NULL,
    EmailAddresses		NVARCHAR(MAX)		NOT NULL,
    [Group]				NVARCHAR(64)		NOT NULL
);

CREATE TABLE Leining.Entries (
	Id					UNIQUEIDENTIFIER	NOT NULL ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
    [Date]				DATETIME			NOT NULL UNIQUE,
    LeinerId			UNIQUEIDENTIFIER		NULL REFERENCES Leining.Leiners(Id),
    Notes				NVARCHAR(256)		NOT NULL,
	Confirmed			BIT					NOT NULL DEFAULT(0),
	LastEmailed			DATETIME				NULL
);

CREATE TABLE Leining.ChangeHistory (
	Id					UNIQUEIDENTIFIER	NOT NULL ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
    DateChanged			DATETIME			NOT NULL			 DEFAULT(getdate()),
	EntryId				UNIQUEIDENTIFIER	NOT NULL REFERENCES Leining.Entries(Id),
	PreviousLeinerId	UNIQUEIDENTIFIER		NULL REFERENCES Leining.Leiners(Id),
	NewLeinerId			UNIQUEIDENTIFIER		NULL REFERENCES Leining.Leiners(Id)
);
