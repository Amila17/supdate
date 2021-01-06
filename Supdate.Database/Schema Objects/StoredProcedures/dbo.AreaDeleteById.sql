CREATE PROCEDURE [dbo].[AreaDeleteById]
  @areaId INT
AS
BEGIN
  --Isolation levels?

  BEGIN TRY
    BEGIN TRANSACTION AREADELETE

      DELETE FROM [dbo].[ReportArea] WHERE AreaId = @areaId
      UPDATE [dbo].[Goal] SET AreaId = NULL WHERE AreaId = @areaId
      UPDATE [dbo].[Metric] SET AreaId = NULL WHERE AreaId = @areaId
      DELETE FROM [dbo].[Area] WHERE Id = @areaId

    COMMIT TRANSACTION AREADELETE
    RETURN 1
  END TRY
  BEGIN CATCH
    IF(@@TRANCOUNT > 0)
      ROLLBACK TRAN
      RETURN 0
  END CATCH
END