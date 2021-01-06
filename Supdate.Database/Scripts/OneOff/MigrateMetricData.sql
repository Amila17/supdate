-- This is a one time script for migrating data from ReportMetric and MetricForecast tables.

-- table for holding reportMetric values with date from report
DECLARE @tbl TABLE (
  MetricId int,
  Value float,
  [Date] datetime,
  CreatedDate datetime,
  UpdatedDate datetime
)

-- populate temp table
INSERT INTO @tbl
SELECT rm.MetricId, rm.Value, r.Date, r.CreatedDate, r.UpdatedDate
FROM   ReportMetric rm INNER JOIN
  Report r ON rm.ReportId = r.Id

-- insert combined data into MetricDataPoint
INSERT INTO MetricDataPoint
(MetricId, CreatedDate, UpdatedDate, [Date], [Target], Actual)
SELECT
  ISNULL(mf.MetricId, rm.MetricId) as MetricId,
  ISNULL(mf.CreatedDate, rm.CreatedDate) as CreatedDate,
  ISNULL(mf.UpdatedDate,rm.UpdatedDate) as UpdatedDate ,
  ISNULL(mf.Month, rm.Date) as [Date],
  mf.Value AS Forecast,
  rm.Value AS Actual
FROM  MetricForecast mf
FULL OUTER JOIN @tbl rm
ON mf.Month = rm.Date AND mf.MetricId = rm.MetricId
