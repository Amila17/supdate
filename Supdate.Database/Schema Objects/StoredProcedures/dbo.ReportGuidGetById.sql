CREATE PROCEDURE [dbo].[ReportGuidGetById]
  @companyId INT,
  @reportId INT
AS
BEGIN

  SELECT UniqueId
  FROM Report r
  WHERE r.CompanyId = @companyId
    AND r.Id = @reportId

END
