CREATE PROCEDURE [dbo].[MetricUpdateMetadata]
  @metricId INT
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @DataPoints int, @value float

  SELECT @DataPoints = count(Id) FROM MetricDataPoint WHERE MetricId = @metricId AND Actual IS NOT NULL AND [Date] <= GETUTCDATE();
  SELECT TOP 1 @value = Actual FROM MetricDataPoint WHERE MetricId = @metricId and Actual IS NOT NULL AND [Date] <= GETUTCDATE() ORDER BY DATE DESC;

  UPDATE Metric
  SET [LatestValue] = @value
    , DataPoints = @DataPoints
  WHERE Id = @metricId
END
