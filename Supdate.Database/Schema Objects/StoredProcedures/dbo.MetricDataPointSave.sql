CREATE PROCEDURE [dbo].[MetricDataPointSave]
  @companyId int,
  @metricid int,
  @date datetime,
  @actual float,
  @target float
AS
  DECLARE @mdpId int
  DECLARE @ownedMetricId int

  --check this metric belongs to this company
  SELECT @ownedMetricId = id FROM Metric WHERE Id = @metricid AND CompanyId = @companyId
  if (@ownedMetricId IS NULL) RETURN

  -- see if a data point already exists for this metricid/date combo
  SELECT @mdpId = id FROM MetricDataPoint WHERE [Date] = @date AND MetricId = @metricid

  if(@mdpId IS NULL)
  BEGIN
    -- it doesn't exist, create it
    INSERT INTO MetricDataPoint
     (MetricId, [Date], Actual, [Target], UpdatedDate)
    VALUES
    (@metricid, @date, @actual, @target, GETUTCDATE())
  END
  ELSE
  BEGIN
    -- already exists, update it
    UPDATE MetricDataPoint SET
      [Target] = @target,
      Actual = @actual,
      UpdatedDate = GETUTCDATE()
    WHERE id=@mdpId  END
