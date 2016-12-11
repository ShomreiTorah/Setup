--This file creates tables used by the optional payment importer
--plugin for the billing system.
--It depends on Core and Billing.

CREATE TABLE Billing.ImportedPayments (
	ImportedPaymentId UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	PaymentId		UNIQUEIDENTIFIER	NOT NULL	UNIQUE		REFERENCES Billing.Payments(PaymentId) ON DELETE CASCADE,
	Source			NVARCHAR(64)		NOT NULL,
	ExternalId		NVARCHAR(64)		NOT NULL,
	DateImported	DATETIME			NOT NULL	DEFAULT getdate(),
	ImportingUser	NVARCHAR(32)		NOT NULL,
	[RowVersion]	RowVersion
);
