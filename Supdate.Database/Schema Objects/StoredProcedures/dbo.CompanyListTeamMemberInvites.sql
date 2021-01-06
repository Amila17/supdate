CREATE PROCEDURE [dbo].[CompanyListTeamMemberInvites]
  @companyId INT
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
  WHERE cui.CompanyId = @companyId
  ORDER BY cui.CreatedDate

  SELECT InviteId, AreaId
  FROM CompanyUserInviteAreaPermission
  WHERE InviteId IN (SELECT Id FROM CompanyUserInvite WHERE CompanyId = @companyId )
END
