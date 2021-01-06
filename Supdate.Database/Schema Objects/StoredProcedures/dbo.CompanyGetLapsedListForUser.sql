CREATE PROCEDURE [dbo].[CompanyGetLapsedListForUser]
  @userId INT
AS
BEGIN
  SELECT c.Id
    , c.Name
    , c.TwitterHandle
    , c.StartMonth
    , c.LogoPath
    , c.ReportTitle
    , c.UniqueId
  FROM Company c
  INNER JOIN CompanyUser cu ON c.Id = cu.CompanyId
  LEFT JOIN Subscription s ON s.CompanyId = c.Id
  WHERE cu.UserId = @userId
    AND cu.IsOwner = 0
    AND s.ExpiryDate < GETUTCDATE()
   ORDER BY c.Name
END
