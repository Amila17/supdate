CREATE PROCEDURE [dbo].[CompanyAddTeamMember]
  @companyId INT,
  @emailAddress NVARCHAR(200),
  @accessibleAreaIds [IntegerList] READONLY,
  @canViewReports BIT
AS
BEGIN
  DECLARE @uid INT
  DECLARE  @status INT

  SELECT TOP 1 @uid = Id FROM AppUser WHERE Email = @emailAddress

  IF (@uid IS NOT NULL)
  BEGIN
    -- check user isn't already in CompanyUser table
    DECLARE @relationshipId INT
    SELECT TOP 1 @relationshipId = Id FROM CompanyUser WHERE CompanyId = @companyId AND UserId = @uid

    IF (@relationshipId IS NOT NULL)
      SET @status = 0 -- user already exists and is already connected to company
    ELSE
    BEGIN
      EXEC CompanyAssociateUser @companyId, @uid, 0, @canViewReports -- add user to CompanyUser
      EXEC CompanyTeamMemberPermissionsUpdate @companyId, @uid, null, @accessibleAreaIds, @canViewReports
      SET @status = 1 -- user added
    END
  END
  ELSE
    SET @status = 2 -- new user, need to send invite

  SELECT @status as status
END
