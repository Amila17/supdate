CREATE PROCEDURE [dbo].[CompanyTeamMemberRemove]
  @userGuid UNIQUEIDENTIFIER,
  @companyId INT
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @uid int

  SELECT TOP 1 @uid = Id
  FROM AppUser
  WHERE UniqueId = @userGuid

  DELETE CompanyUser
  WHERE CompanyId = @companyId AND UserId = @uid AND IsOwner = 0

  RETURN 0
END
