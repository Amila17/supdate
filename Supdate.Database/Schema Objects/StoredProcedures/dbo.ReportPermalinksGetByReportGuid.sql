CREATE PROCEDURE [dbo].[ReportPermalinksGetByReportGuid]
  @reportGuid UNIQUEIDENTIFIER
AS
BEGIN
  SET NOCOUNT ON
  DECLARE @companyId INT

  SELECT @companyId = r.CompanyId
  FROM Report r
  WHERE UniqueId = @reportGuid

  EXEC [ReportPermalinksGetByCompanyId] @companyId
END
