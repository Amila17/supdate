CREATE PROCEDURE [dbo].[CompanyGetListForUser]
  @userId INT,
  @isOwner INT = NULL
AS
BEGIN
  SELECT c.Id
    , c.Name
    , c.TwitterHandle
    , c.StartMonth
    , c.LogoPath
    , c.ReportTitle
    , c.UniqueId
    , cu.CanViewReports
  FROM Company c
  INNER JOIN CompanyUser cu ON c.Id = cu.CompanyId
  LEFT JOIN Subscription s ON s.CompanyId = c.Id
  WHERE cu.UserId = @userId
    AND (@isOwner IS NULL OR cu.IsOwner = @isOwner)
    AND (cu.IsOwner = 1 OR s.ExpiryDate >= GETUTCDATE()) -- don't include companies where user is a team member, but company doesn't have a subscription
  ORDER BY c.Name
END
