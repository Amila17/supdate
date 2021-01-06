CREATE PROCEDURE [dbo].[AreasSaveOrder]
  @companyId int,
  @orderData  [EntityDisplayOrder] READONLY
AS
BEGIN
  UPDATE a
    SET DisplayOrder = od.DisplayOrder
  FROM @orderData od
  INNER JOIN Area a ON a.UniqueId = od.EntityUniqueId
  WHERE a.CompanyId = @companyId
END
