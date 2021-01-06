CREATE PROCEDURE [dbo].[CompaniesGetRegistered]
  @desiredPageNumber INT = 1,
  @numberOfRows INT = 10
AS
BEGIN
   SET NOCOUNT ON
    SELECT c.Id
    , c.Name
    , c.TwitterHandle
    , c.StartMonth
    , c.LogoPath
    , c.ReportTitle
    , c.UniqueId
    , u.Id AS OwnerId
    , u.Email AS OwnerEmail
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
  WHERE u.EmailConfirmed = 1
  ORDER BY c.Id DESC
  OFFSET (@numberOfRows * (@desiredPageNumber - 1)) ROWS
  FETCH NEXT @NumberOfRows ROWS ONLY;
END
