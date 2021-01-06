CREATE PROCEDURE [dbo].[CompanyTeamMemberAcceptInvite]
  @inviteGuid UNIQUEIDENTIFIER,
  @userid INT
AS
BEGIN
  DECLARE @inviteId INT, @companyId INT, @canViewReports BIT

  SELECT @inviteId = Id
    , @companyId = CompanyId
    , @canViewReports = CanViewReports
  FROM CompanyUserInvite
  WHERE UniqueId = @inviteGuid AND ResultantUserId IS NULL

  -- invite guid is valid
  IF (@inviteId IS NOT NULL)
  BEGIN
    -- void invite
    UPDATE CompanyUserInvite
    SET ResultantUserId = @userid,
      UsedDate = GETUTCDATE()
    WHERE Id = @inviteId

    -- associate user
    EXEC CompanyAssociateUser @companyId, @userid, 0, @canViewReports

    -- copy across area permissions
    INSERT INTO CompanyUserAreaPermission(CompanyId, UserId, AreaId)
    SELECT @companyId, @userid, AreaId
    FROM CompanyUserInviteAreaPermission
    WHERE InviteId = @inviteId
  END
END
