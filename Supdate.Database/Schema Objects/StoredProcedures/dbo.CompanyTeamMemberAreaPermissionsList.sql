CREATE PROCEDURE [dbo].[CompanyTeamMemberAreaPermissionsList]
  @companyId INT,
  @userId INT = NULL,
  @userGuid UNIQUEIDENTIFIER = NULL
AS
BEGIN
  SET NOCOUNT ON;

  IF (@userId IS NULL)
  BEGIN
    SELECT TOP 1 @userId = Id
    FROM AppUser
    WHERE UniqueId = @userGuid
  END

  SELECT AreaId
  FROM CompanyUserAreaPermission
  WHERE CompanyId = @companyId AND UserId = @userId
END
