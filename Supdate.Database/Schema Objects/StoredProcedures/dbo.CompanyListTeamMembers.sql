CREATE PROCEDURE [dbo].[CompanyListTeamMembers]
  @companyId INT
AS
BEGIN
  SELECT u.Id
  , c.CreatedDate
  , c.UpdatedDate
  , u.Email
  , u.UniqueId
  FROM CompanyUser c
  INNER JOIN AppUser u ON c.UserId = u.Id
  WHERE c.CompanyId = @companyId
    AND c.IsOwner = 0
  ORDER BY c.CreatedDate

  SELECT UserId, AreaId FROM CompanyUserAreaPermission
  WHERE CompanyId = @companyId
END
