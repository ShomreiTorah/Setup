IF schema_id('Website') IS NULL
	EXECUTE('create schema Website');
      
CREATE TABLE Website.Pages (
	Id				UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	PageName		VARCHAR(256)		COLLATE SQL_Latin1_General_Cp1251_CI_AS	NOT NULL	UNIQUE,
	Title			NVARCHAR(512)		NOT NULL,
	Content			NTEXT				NOT NULL,
	IsListed		BIT					NOT NULL	DEFAULT(0),
	DateModified	DATETIME			NOT NULL	DEFAULT(getdate())
);
INSERT INTO Website.Pages(PageName, Title, Content, IsListed) 
	VALUES ('Home', 'Home', 'Welcome to Congregation Shomrei Torah', 1);
	
CREATE TABLE Website.PageHistory (
	Id				UNIQUEIDENTIFIER	NOT NULL	ROWGUIDCOL	PRIMARY KEY DEFAULT(newid()),
	PageName		VARCHAR(256)		NOT NULL,
	Title			NVARCHAR(512)		NOT NULL,
	Content			NTEXT				NOT NULL,
	IsListed		BIT					NOT NULL,
	[Date]			DATETIME			NOT NULL	DEFAULT(getdate()),
	[User]			NVARCHAR(256)		NOT NULL,
	[UserIP]		VARCHAR(45)			NOT NULL
);


GO
CREATE VIEW Website.PublicPages AS SELECT * FROM Website.Pages WHERE IsListed = 1;
GO

ALTER PROCEDURE Website.SavePage(
	@PageName	VARCHAR(256),
	@Title		NVARCHAR(512),
	@IsListed	BIT,
	@Content	NVARCHAR(MAX),
	@Saver		NVARCHAR(256),
	@IPAddress	VARCHAR(45)
) AS
	DECLARE @DateModified	DATETIME;
	SET @DateModified = getdate();

	UPDATE Website.Pages SET Title = @Title, Content = @Content, IsListed = @IsListed, DateModified = @DateModified
		WHERE PageName = @PageName
	IF (@@ROWCOUNT = 0)
		INSERT INTO Website.Pages(PageName, Title, Content, IsListed, DateModified) 
			VALUES (@PageName, @Title, @Content, @IsListed, @DateModified)
			
	INSERT INTO Website.PageHistory (PageName,  Title,  Content,  IsListed, [User], UserIP)
							VALUES (@PageName, @Title, @Content, @IsListed, @Saver, @IPAddress);

	SELECT @DateModified;
GO
--------------------------------------------------------------------------------------------
CREATE TABLE dbo.SSRequests (
	ID				INT				IDENTITY,
	[Date]			DATETIME		NOT NULL,
	FullName		NVARCHAR(64)	NOT NULL,
	Phone			VARCHAR(10)		NOT NULL,
	Email			NVARCHAR(64)	NOT NULL,
	Text			NVARCHAR(512)	NOT NULL
);
CREATE TABLE dbo.MBList (
	ID				INT				IDENTITY,
	FullName		NVARCHAR(128)	NOT NULL,
	Phone			VARCHAR(10)		NOT NULL,
	Email			NVARCHAR(64)	NOT NULL,
	CholehName		NVARCHAR(512)	NOT NULL,
	Until			DATETIME		NOT NULL
);
CREATE TABLE Website.SSEmails (
	PledgeId		UNIQUEIDENTIFIER	NOT NULL	UNIQUE REFERENCES Billing.Pledges(PledgeId) ON DELETE CASCADE,
	Email			VARCHAR(64)			NOT NULL
);
GO
CREATE VIEW Website.SSSponsors AS 
	SELECT TOP 1000000 --TOP is required for ORDER BY
		Billing.Pledges.PledgeId,
		Date,
		FullName, 
		Phone, 
		Email, 
		Amount,
		Note AS Text
	FROM 
		Billing.Pledges JOIN Data.MasterDirectory	ON Billing.Pledges.PersonId = Data.MasterDirectory.Id
				   LEFT JOIN Website.SSEmails		ON Billing.Pledges.PledgeId = Website.SSEmails.PledgeId
	WHERE Type = N'סעודה שלישית'
	ORDER BY Modified;
GO
--------------------------------------------------------------------------------------------

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

CREATE TABLE dbo.tblMLNewsletter (
	Newsletter_ID		INT				NOT NULL IDENTITY(1,1)	PRIMARY KEY,
	Newsletter			NTEXT			NOT NULL,
	Newsletter_subject	NVARCHAR(512)	NOT NULL,
	Newsletter_note		NVARCHAR(130)	NULL,
	Newsletter_date		DATETIME		NOT NULL	DEFAULT(getdate()),
	Display				BIT				NOT NULL	DEFAULT(0),
	HTML				BIT				NOT NULL	DEFAULT(0)
);
GO
--------------------------------------------------------------------------------------------
--Used by ManageAdmins.aspx in admin site
CREATE VIEW UserNamesInRoles AS
	SELECT dbo.aspnet_UsersInRoles.UserId, dbo.aspnet_UsersInRoles.RoleId, dbo.aspnet_Users.UserName
	FROM   dbo.aspnet_Users INNER JOIN dbo.aspnet_UsersInRoles 
		ON dbo.aspnet_Users.UserId  =  dbo.aspnet_UsersInRoles.UserId