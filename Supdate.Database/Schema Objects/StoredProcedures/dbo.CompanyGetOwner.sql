CREATE PROCEDURE [dbo].[CompanyGetOwner]
  @companyId int
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @userId int
  SELECT @userId = UserId FROM CompanyUser WHERE CompanyId = @companyId AND IsOwner = 1

  SELECT TOP 1 u.Id
    , u.Email
    , c.Id AS CompanyId
    , c.Name AS Company
    , c.LogoPath as LogoPath
    , cu.IsOwner as isCompanyAdmin
    , (SELECT 1 FROM AdminUser WHERE UserId = @userId) AS IsAdmin
  FROM AppUser u
  INNER JOIN CompanyUser cu ON cu.UserId = u.Id
  INNER JOIN Company c ON c.Id = cu.CompanyId
  WHERE u.Id = @userId
    AND c.Id = @companyId

  RETURN 0
END
