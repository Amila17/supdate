CREATE PROCEDURE [dbo].[CompanyGetTeamMemberInvite]
  @inviteGuid UNIQUEIDENTIFIER
AS
BEGIN
  SELECT cui.Id
    , cui.CompanyId
    , Company.Name AS CompanyName
    , cui.EmailAddress
    , cui.UsedDate
    , cui.ResultantUserId
    , cui.UniqueId
    , cui.CreatedDate
    , cui.UpdatedDate
  FROM CompanyUserInvite cui
  INNER JOIN Company ON cui.CompanyId = Company.Id
  WHERE cui.UniqueId = @inviteGuid
  RETURN 0
END
