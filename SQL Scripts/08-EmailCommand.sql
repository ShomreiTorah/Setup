--This file creates the tracking table used by the EmailCommand system.
--It has no dependencies.
CREATE TABLE dbo.ProcessedEmails(
	UID				VARCHAR(70)		NOT NULL	PRIMARY KEY,
	DateProcessed	DATETIME		NOT NULL	DEFAULT(getdate())
)