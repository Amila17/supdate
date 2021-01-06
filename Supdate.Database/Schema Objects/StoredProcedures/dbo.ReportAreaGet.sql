CREATE PROCEDURE [dbo].[ReportAreaGet]
  @CompanyId INT,
  @AreaUniqueId UNIQUEIDENTIFIER,
  @ReportDate DATETIME
AS
BEGIN

  DECLARE @ReportId INT
  DECLARE @AreaId INT

  SELECT @AreaId = Id FROM Area WHERE UniqueId = @AreaUniqueId and CompanyId = @CompanyId

  SELECT @ReportId = r.Id FROM Report r WHERE r.CompanyId = @CompanyId AND r.[Date] = @ReportDate

  IF EXISTS(SELECT ra.Id FROM ReportArea ra WHERE ra.ReportId = @ReportId AND ra.AreaId = @AreaId)
  BEGIN
    SELECT a.Id AS AreaId, a.UniqueId as AreaUniqueId, a.Name AS AreaName, @ReportDate AS ReportDate, ra.*
    FROM ReportArea ra
    INNER JOIN Area a ON a.Id=ra.AreaId
    WHERE ra.ReportId = @ReportId AND ra.AreaId = @AreaId
  END
  ELSE
  BEGIN
    SELECT a.Id AS AreaId, a.UniqueId AS AreaUniqueId, @ReportId AS ReportId, @ReportDate AS ReportDate, a.Name AS AreaName
    FROM Area a
    WHERE a.Id = @AreaId
  END

  EXEC MetricViewsGet @companyId, @reportDate, @AreaId

  EXEC ReportGoalsGet  @companyId, @reportDate, @AreaId

  SELECT ra.* FROM ReportAttachment ra
  INNER JOIN Report r ON r.Id = ra.ReportId
  WHERE r.CompanyId = @CompanyId AND r.[Date] = @reportDate
  AND ra.AreaId = @AreaId

END
