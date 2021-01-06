CREATE PROCEDURE [dbo].[ReportGuidToCompanyGuid]
  @reportGuid uniqueidentifier
AS
BEGIN
  SELECT c.UniqueId FROM Report r
  INNER JOIN Company c ON r.CompanyId = c.Id
  WHERE r.UniqueId = @reportGuid
END
GO
