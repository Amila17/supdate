CREATE PROCEDURE [dbo].[GoalGet]
  @companyId INT,
  @goalGuid UNIQUEIDENTIFIER
AS
BEGIN
  DECLARE @id int
  SELECT
    @id = Id
  FROM Goal
  WHERE CompanyId = @companyId
  AND UniqueId = @goalGuid

  SELECT *
  FROM Goal
  WHERE id = @id
END
GO
