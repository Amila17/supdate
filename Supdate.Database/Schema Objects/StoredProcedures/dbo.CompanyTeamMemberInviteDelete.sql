CREATE PROCEDURE [dbo].[CompanyTeamMemberInviteDelete]
  @companyId INT,
  @inviteId INT,
  @inviteGuid UNIQUEIDENTIFIER
AS
BEGIN
  SET NOCOUNT ON

  IF (@inviteId is null)
  BEGIN
    SELECT @inviteId = Id FROM CompanyUserInvite WHERE UniqueId = @inviteGuid
  END

  DELETE CompanyUserInvite
  WHERE CompanyId = @companyId
    AND Id = @inviteId
    AND ResultantUserId IS NULL

  RETURN @@ROWCOUNT
END
