CREATE PROCEDURE [dbo].[MetricsGraphData]
  @companyId int,
  @reportUniqueId UNIQUEIDENTIFIER = NULL
AS
BEGIN
  IF (@companyId = 0)
  BEGIN
    SELECT @companyId = CompanyId
    FROM Report
    WHERE UniqueId = @reportUniqueId
  END

  -- assume we're getting all metric data with no time limit
  DECLARE @maxDate DATE = CONVERT(VARCHAR(10), '2050-01-01', 120)

  -- if a report is specified, then we only want to get data up to and including the date for that report
  IF (@reportUniqueId IS NOT NULL)
  BEGIN
    SELECT @maxDate = [Date] FROM Report WHERE UniqueId = @reportUniqueId
  END

  -- get reported metric values and forecasts
  SELECT m.UniqueId, mdp.[Date], mdp.Actual, mdp.[Target], m.AreaId, m.Prefix, m.Suffix, m.ThousandsSeparator,
    CASE
      WHEN  mdp.[Date] > @maxDate THEN 1
      ELSE 0
    END as IsFutureData,
    CASE
      WHEN  mdp.[Date] =  @maxDate THEN 1
      ELSE 0
    END as IsCurrentDate

  FROM MetricDataPoint  mdp
  INNER JOIN Metric m ON mdp.MetricId = m.Id
  WHERE m.CompanyId = @companyId
    AND (mdp.Actual is not null OR mdp.Target is not null)
  ORDER BY mdp.[Date]
END
