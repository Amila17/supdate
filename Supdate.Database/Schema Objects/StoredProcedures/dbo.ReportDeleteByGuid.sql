CREATE PROCEDURE [dbo].[ReportDeleteByGuid]
  @companyId INT,
  @reportGuid UNIQUEIDENTIFIER
AS
BEGIN

  DELETE
  FROM Report
  WHERE CompanyId = @companyId
    AND UniqueId = @reportGuid

END
