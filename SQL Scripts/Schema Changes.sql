/*
 * This file should contain ALTER statements.
 * Any statements here should also be applied
 * to the original CREATEs in other files, so
 * this script will not be necessary to fill 
 * a new database from scratch.
 * 
 * Please comment changes with date and purpose
 */

ALTER TABLE Data.MasterDirectory ADD [RowVersion] RowVersion;
ALTER TABLE Billing.Payments ADD [RowVersion] RowVersion;
ALTER TABLE Billing.Pledges ADD [RowVersion] RowVersion;
ALTER TABLE Billing.Deposits ADD [RowVersion] RowVersion;
ALTER TABLE Billing.StatementLog ADD [RowVersion] RowVersion;
ALTER TABLE dbo.tblMLMembers ADD [RowVersion] RowVersion;
ALTER TABLE dbo.tblMLMembers ADD [RowId] UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL UNIQUE DEFAULT(newid());

ALTER TABLE Seating.SeatingReservations DROP COLUMN [Status];	--Cannot execute until after disting new client
ALTER TABLE Seating.SeatingReservations ALTER COLUMN [Status] NVARCHAR(64) NULL;

ALTER TABLE Data.MasterDirectory ADD Salutation		NVARCHAR(100)		NOT NULL DEFAULT('');

--For call list
ALTER TABLE MelaveMalka.Invitees ADD ShouldCall	BIT					NOT NULL	DEFAULT(0);
ALTER TABLE MelaveMalka.Invitees ADD [Caller]	UNIQUEIDENTIFIER	NULL		DEFAULT(NULL)	REFERENCES MelaveMalka.Callers(RowId);
ALTER TABLE MelaveMalka.Invitees ADD CallerNote	NVARCHAR(512)		NULL;

--For reminder emails
ALTER TABLE MelaveMalka.Invitees ADD ShouldEmail	BIT				NOT NULL	DEFAULT(0);
ALTER TABLE MelaveMalka.Invitees ADD EmailSubject	NVARCHAR(256)	NULL		DEFAULT(NULL);
ALTER TABLE MelaveMalka.Invitees ADD EmailSource	NTEXT			NULL		DEFAULT(NULL);


--2011-04-17 Email addresses don't fit
ALTER TABLE dbo.tblMLMembers ALTER COLUMN Name NVARCHAR(100);
ALTER TABLE dbo.tblMLMembers ALTER COLUMN Email NVARCHAR(100);

--2011-11-14 Unlisted CMS pages
ALTER TABLE Website.Pages ADD IsListed BIT NOT NULL DEFAULT(0);
UPDATE Website.Pages SET IsListed = 1 WHERE PageName = 'Home';