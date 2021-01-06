CREATE PROCEDURE [dbo].[CompanyInviteUser]
  @companyId INT,
  @emailAddress NVARCHAR(200),
  @accessibleAreaIds [IntegerList] READONLY,
  @canViewReports BIT
AS
BEGIN
  SET NOCOUNT ON
  --ensure user doesn't already exist
  DECLARE @existingId int
  SELECT @existingId = Id
  FROM AppUser
  WHERE Email = @emailAddress

  IF (@existingId IS NOT NULL) RETURN

  -- re-use existing invite if it's there
  DECLARE @inviteId INT
  SELECT @inviteId = Id
  FROM CompanyUserInvite
  WHERE EmailAddress = @emailAddress AND CompanyId = @companyId

  --invite doesn't already exist, create it
  IF (@inviteId IS NULL)
  BEGIN
    -- create the invite
    -- save invite
    INSERT INTO CompanyUserInvite
      (CompanyId, EmailAddress, CanViewReports)
    VALUES
      (@companyId,@emailAddress, @canViewReports)

    -- get the id
    SELECT @inviteId =  SCOPE_IDENTITY()
  END

  -- AREA PERMISSIONS
  -- delete any existing permissions for this invite (in case it's a resend and they've changed)
  DELETE CompanyUserInviteAreaPermission
  WHERE InviteId = @inviteId

  -- add new permissions
  INSERT INTO CompanyUserInviteAreaPermission(InviteId, AreaId)
  SELECT @inviteId, Id
  FROM Area
  WHERE CompanyId = @companyId
    AND Id IN (SELECT intVal FROM @accessibleAreaIds)

  -- return the invite
  SELECT cui.Id
    , cui.CompanyId
    , Company.Name AS CompanyName
    , cui.CanViewReports
    , cui.EmailAddress
    , cui.UsedDate
    , cui.ResultantUserId
    , cui.UniqueId
    , cui.CreatedDate
    , cui.UpdatedDate
  FROM CompanyUserInvite cui
  INNER JOIN Company ON cui.CompanyId = Company.Id
  WHERE cui.Id = @inviteId
END
