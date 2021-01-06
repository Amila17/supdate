CREATE PROCEDURE [dbo].[ReportGetByMonth]
  @CompanyId INT,
  @reportDate DATETIME
AS
BEGIN
  --SET NOCOUNT ON added to prevent extra result sets from
  --interfering with SELECT statements.
  SET NOCOUNT ON;
  DECLARE @reportId int

  SELECT @reportId = Id FROM Report WHERE [Date] = @reportDate and CompanyId = @CompanyId

  SELECT r.Id, r.UniqueId, r.CompanyId, r.[Date], c.ReportTitle as Title, r.[StatusId], r.[IsStatusManual],
  c.Name AS CompanyName, c.LogoPath AS CompanyLogo,
  r.Summary, r.CreatedDate, r.UpdatedDate, EmailedTo = (SELECT COUNT(*) FROM ReportEmail WHERE ReportId = r.Id)
  FROM Report r
  INNER JOIN Company c ON r.CompanyId = c.Id
  WHERE r.CompanyId = @CompanyId AND r.[Date] = @reportDate

  SELECT a.Id as AreaId, @reportId as ReportId, @reportDate as ReportDate, a.Name as AreaName,
        ra.Summary, a.UniqueId as AreaUniqueId, ra.CreatedDate, ra.UpdatedDate
  FROM Area a
  LEFT OUTER JOIN ReportArea ra ON ra.AreaId = a.Id AND ra.ReportId = @reportId
  WHERE a.CompanyId = @CompanyId
  ORDER BY a.DisplayOrder, a.Id

  EXEC ReportGoalsGet  @companyId, @reportDate

  EXEC MetricViewsGet @companyId, @reportDate

  SELECT ra.* FROM ReportAttachment ra
  INNER JOIN Report r ON r.Id = ra.ReportId
  WHERE r.CompanyId = @CompanyId AND r.[Date] = @reportDate
END
