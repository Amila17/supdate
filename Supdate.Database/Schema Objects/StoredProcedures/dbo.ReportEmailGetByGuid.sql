CREATE PROCEDURE [dbo].[ReportEmailGetByGuid]
  @reportEmailGuid UNIQUEIDENTIFIER
AS
BEGIN
  DECLARE @reportEmailId INT
  SELECT @reportEmailId = Id FROM ReportEmail WHERE UniqueId = @reportEmailGuid

  EXEC ReportEmailDetailsGet @reportEmailId
END
GO
