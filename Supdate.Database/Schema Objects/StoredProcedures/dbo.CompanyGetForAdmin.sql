CREATE PROCEDURE [dbo].[CompanyGetForAdmin]
  @guid nvarchar(50)
AS
BEGIN
  SET NOCOUNT ON;
  DECLARE @companyId int
  SELECT @companyId = Id FROM Company WHERE UniqueId = @guid

  -- Company record
  SELECT c.Id
    , c.Name
    , c.TwitterHandle
    , c.StartMonth
    , c.LogoPath
    , c.ReportTitle
    , c.UniqueId
    , u.Id AS OwnerId
    , u.UniqueId AS OwnerUniqueId
    , c.CreatedDate AS CreatedDate
    , cs.AreaCount
    , cs.MetricCount
    , cs.GoalCount
    , cs.ReportCount
    , cs.UserCount
  FROM Company c
  LEFT JOIN CompanyStats cs ON c.Id = cs.CompanyId
  INNER JOIN CompanyUser cu ON c.Id = cu.CompanyId
  INNER JOIN AppUser u ON (cu.UserId = u.Id AND cu.IsOwner = 1)
  WHERE c.Id = @companyId

  -- Team members
  SELECT Id
    , UserName
    , Email
    , UniqueId
    , LoginCount
    , LastLogin
  FROM AppUser
  WHERE Id IN (SELECT UserId FROM CompanyUser WHERE IsOwner = 0 AND CompanyId = @companyId)

  -- Pending invitations
  SELECT Id
    , CompanyId
    , EmailAddress
    , UniqueId
    , CreatedDate
    , UpdatedDate
  FROM CompanyUserInvite ui
  WHERE ui.CompanyId = @companyId
    AND ui.ResultantUserId IS NULL
  ORDER BY ui.CreatedDate ASC
END
