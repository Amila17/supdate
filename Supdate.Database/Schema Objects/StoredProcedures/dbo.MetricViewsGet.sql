CREATE PROCEDURE [dbo].[MetricViewsGet]
  @companyId INT,
  @reportDate DATETIME = NULL,
  @singleAreaId INT = NULL
AS
BEGIN
  SET NOCOUNT ON;

  -- Get Report Id
  DECLARE @reportId INT
  DECLARE @reportUniqueId UNIQUEIDENTIFIER
  DECLARE @lastMonthReportId INT
  DECLARE @lastMonthReportDate DATETIME

  IF (@reportDate IS NULL)
  BEGIN
    -- Get most recent report date with a value entered for metrics
    SELECT TOP 1 @reportDate = [Date]
    FROM MetricDataPoint mdp
    INNER JOIN Metric m ON m.Id = mdp.MetricId
    WHERE m.CompanyId = @companyId
    AND mdp.Actual is not null
    ORDER BY [Date] DESC
  END

  SELECT
    @reportId = r.Id,
    @reportUniqueId = r.UniqueId
  FROM Report r
  WHERE r.CompanyId = @companyId
    AND r.[Date] = @reportDate

  -- get last months report date
  SELECT TOP 1 @lastMonthReportDate = r.[Date]
  FROM Report r
  WHERE r.CompanyId = @companyId AND r.[Date] < @reportDate
  ORDER BY r.[Date] DESC

  SELECT mdp.Id as MetricDataPointId, @ReportId AS ReportId, @reportUniqueId as reportUniqueId, mdp.Actual, mdp.[Target],
    mdpLM.Actual as LastMonthActual, isnull(mdp.[Date],@reportDate) as [Date],
    m.*, @reportDate as ReportDate,
    mdp.CreatedDate, mdp.UpdatedDate
  FROM Metric m
  LEFT JOIN MetricDataPoint mdp on mdp.MetricId = m.Id AND mdp.[Date] =  @reportDate
  LEFT JOIN MetricDataPoint  mdpLM ON mdpLM.MetricId = m.Id AND mdpLM.[Date] = @lastMonthReportDate
  WHERE m.CompanyId = @companyId AND (m.AreaId = @singleAreaId OR @singleAreaId IS NULL)
  ORDER BY m.DisplayOrder

END
