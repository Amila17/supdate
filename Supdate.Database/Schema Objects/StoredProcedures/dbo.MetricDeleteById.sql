CREATE PROCEDURE [dbo].[MetricDeleteById]
  @metricId INT
AS
BEGIN
  --Isolation levels?

  BEGIN TRY
    BEGIN TRANSACTION METRICDELETE

    DELETE FROM [dbo].[MetricDataPoint] WHERE MetricId = @metricId
    DELETE FROM [dbo].[Metric] WHERE Id = @metricId

    COMMIT TRANSACTION METRICDELETE
    RETURN 1
  END TRY
  BEGIN CATCH
    IF(@@TRANCOUNT > 0)
      ROLLBACK TRAN
      RETURN 0
  END CATCH
END
