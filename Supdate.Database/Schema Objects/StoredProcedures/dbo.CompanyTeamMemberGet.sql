CREATE PROCEDURE [dbo].[CompanyTeamMemberGet]
  @companyId INT,
  @userGuid UNIQUEIDENTIFIER
AS
BEGIN
  SET NOCOUNT ON;

  SELECT TOP 1 u.Id
    , u.UniqueId
    , u.Email
    , c.Id AS CompanyId
    , c.Name AS Company
    , c.LogoPath as LogoPath
    , cu.CanViewReports
    , cu.IsOwner as isCompanyAdmin
    , 0 as IsAdmin
  FROM AppUser u
  INNER JOIN CompanyUser cu ON cu.UserId = u.Id
  INNER JOIN Company c ON c.Id = cu.CompanyId
  WHERE u.UniqueId = @userGuid
    AND cu.CompanyId = @companyId

  EXEC CompanyTeamMemberAreaPermissionsList @companyId, null, @userGuid
END
