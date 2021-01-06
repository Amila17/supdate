CREATE PROCEDURE [dbo].[MetricsSaveOrder]
  @companyId int,
  @orderData  [EntityDisplayOrder] READONLY
AS
BEGIN
  UPDATE m
    SET DisplayOrder = od.DisplayOrder
  FROM @orderData od
  INNER JOIN Metric m ON m.UniqueId = od.EntityUniqueId
  WHERE m.CompanyId = @companyId
END
