CREATE PROCEDURE [dbo].[MetricGuidToId]
  @companyId int,
  @uniqueId uniqueIdentifier
AS
  SELECT Id
  FROM Metric
  WHERE UniqueId = @uniqueId AND CompanyId = CompanyId
