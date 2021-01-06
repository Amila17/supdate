CREATE PROCEDURE [dbo].[CompanyTeamMemberPermissionsUpdate]
  @companyId INT,
  @userId INT = NULL,
  @userGuid UNIQUEIDENTIFIER = NULL,
  @accessibleAreaIds [IntegerList] READONLY,
  @canViewReports BIT
AS
BEGIN
  SET NOCOUNT ON;

  IF (@userId IS NULL)
  BEGIN
    SELECT TOP 1 @userId = Id
    FROM AppUser WHERE UniqueId = @userGuid
  END

  -- Only run if user has access to the company stated
  IF EXISTS (SELECT 1 FROM CompanyUser WHERE UserId = @userId AND CompanyId = @companyId)
  BEGIN
    UPDATE CompanyUser
    SET CanViewReports = @canViewReports
    WHERE UserId = @userId AND CompanyId = @companyId

    DELETE CompanyUserAreaPermission
    WHERE CompanyId = @companyId AND UserId = @userId

    INSERT INTO CompanyUserAreaPermission(CompanyId, UserId, AreaId)
    SELECT @companyId, @userId, a.Id
    FROM Area a
    WHERE CompanyId = @companyId
      AND a.Id IN (SELECT intVal FROM @accessibleAreaIds)
  END
END
