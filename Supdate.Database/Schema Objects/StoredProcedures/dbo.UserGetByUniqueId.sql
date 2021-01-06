CREATE PROCEDURE [dbo].[UserGetByUniqueId]
  @uniqueId UNIQUEIDENTIFIER
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @userId int, @companyId int
  SELECT @userId = Id from AppUser WHERE UniqueId = @uniqueId
  EXEC UserGetDefaultCompanyId @userId, @companyId OUTPUT

  SELECT TOP 1 u.Id
    , u.Email
    , u.UnConfirmedEmail
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

END
