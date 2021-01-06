CREATE PROCEDURE [dbo].[ReportEmailPreviewDetailsGet]
  @reportId int
AS
BEGIN
  --SET NOCOUNT ON added to prevent extra result sets from
  --interfering with SELECT statements.
  SET NOCOUNT ON;

  SELECT c.Name AS CompanyName
    , u.Email AS CompanyEmail
    , C.ReportTitle AS ReportTitle
    , r.[Date] AS ReportDate
    , r.Summary AS ReportSummary
    , r.UniqueId AS ReportUniqueId
  FROM Report r
    INNER JOIN Company c ON c.Id = r.CompanyId
    INNER JOIN CompanyUser cu ON cu.CompanyId = r.CompanyId
    INNER JOIN AppUser u ON u.Id = cu.UserId
  WHERE r.Id = @reportId

END
