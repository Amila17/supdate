CREATE PROCEDURE [dbo].[ReportIdGetByMonth]
  @companyId INT,
  @reportDate DATETIME
AS
BEGIN
  SELECT Id
  FROM Report r
  WHERE r.CompanyId = @companyId
    AND r.[Date] = @reportDate
END
