CREATE PROCEDURE [dbo].[ReportPermalinksGetByCompanyId]
 @companyId int
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    CompanyId, UniqueId, [Date], CAST(StatusId AS SMALLINT) AS [Status]
  FROM Report
  WHERE CompanyId = @companyId

  RETURN 0;
END
GO
