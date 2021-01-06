CREATE PROCEDURE [dbo].[UsersGetRecentReports]
(
  @recordCount int = 20
)
AS
BEGIN
  SELECT
    r.Id AS ReportId
    , r.UniqueId AS ReportUniqueId
    , ISNULL(c.ReportTitle, 'Shareholder Update') AS ReportTitle
    , r.[Date] AS ReportDate
    , c.Name as CompanyName
    , c.Id as CompanyId
  FROM Report r
  INNER JOIN Company c ON r.CompanyId = c.Id
  WHERE (r.Id IN (SELECT DISTINCT TOP (@recordCount) ReportId
                  FROM ReportEmail
                  ORDER BY ReportId DESC))
  ORDER BY r.Id DESC
END
