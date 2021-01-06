CREATE PROCEDURE [dbo].[MetricDataPointsGetForPeriod]
  @companyId INT,
  @year INT,
  @month INT = NULL
AS
BEGIN
  DECLARE @tbl TABLE (
    Id INT,
    MetricId INT,
    Actual FLOAT,
    [Target] FLOAT,
    [Date] DATETIME,
    DataSourceId INT NULL,
    CreatedDate DATETIME,
    UpdatedDate DATETIME
  )

  IF (@month IS NULL)
  BEGIN
    -- create an entry in above table for each month of the year for each Metric this company has
    DECLARE @themonth int =0
    WHILE (@themonth < 12)
    BEGIN
      SET @themonth = @themonth + 1

      INSERT INTO @tbl
        (MetricId, [Date], DataSourceId)
        SELECT Id, (DATEFROMPARTS(@year, @themonth, 1)), DataSourceId
	      FROM Metric where CompanyId = @companyId
    END
  END
  ELSE
  BEGIN
  -- create an entry in above table for each Metric this company has for this month
    INSERT INTO @tbl
      (MetricId, [Date], DataSourceId)
      SELECT Id, (DATEFROMPARTS(@year, @month, 1)), DataSourceId
	    FROM Metric where CompanyId = @companyId
  END

  --select all rows from @tbl, with matching data (if it exists) from MetricDataPoint table
  SELECT
    ISNULL(mdp.Id, 0) as Id,
    t.MetricId,
    t.[Date],
    mdp.Actual,
    mdp.Target,
    t.DataSourceId,
    mdp.CreatedDate,
    mdp.UpdatedDate as UpdatedDate
  FROM  @tbl t
  LEFT OUTER JOIN MetricDataPoint mdp
    ON mdp.[Date] = t.[Date] AND mdp.MetricId = t.MetricId
END
GO
