CREATE PROCEDURE [dbo].[AreaGet]
  @companyId int,
  @uniqueId uniqueIdentifier
AS

  DECLARE @id int
  SELECT
    @id =Id
  FROM Area
  WHERE CompanyId = @companyId
  AND UniqueId = @uniqueId

  SELECT *
  FROM Area
  WHERE id = @id
