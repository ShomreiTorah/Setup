DECLARE @ApplicationId uniqueidentifier
EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

--I don't use the aspnet_Roles_CreateRole sproc because I want to create descriptions
INSERT INTO dbo.aspnet_Roles (ApplicationId, RoleName, LoweredRoleName, Description)
SELECT @ApplicationId, [Name], LOWER([Name]), [Desc] 
FROM (
		SELECT N'Mailing List' AS [Name],	N'View and edit the Shul''s mailing list' AS [Desc]
  UNION	SELECT N'Manage Admins',			N'Create and delete administrators and set their permissions'
  UNION	SELECT N'Manage Content',			N'Edit and create pages on the Shul''s public website'
  UNION	SELECT N'Send Email',				N'Send email to the Shul''s mailing list'
  UNION	SELECT N'שלוש סעודות',				N'Manage סעודה שלישית sponsorships'
	) AS Roles