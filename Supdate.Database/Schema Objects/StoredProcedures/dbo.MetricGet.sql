CREATE PROCEDURE [dbo].[MetricGet]
  @companyId INT,
  @metricGuid UNIQUEIDENTIFIER
AS
BEGIN
  DECLARE @id int
  SELECT
    @id =Id
  FROM Metric
  WHERE CompanyId = @companyId
  AND UniqueId = @metricGuid

  SELECT *
  FROM Metric
  WHERE id = @id

END
GO
