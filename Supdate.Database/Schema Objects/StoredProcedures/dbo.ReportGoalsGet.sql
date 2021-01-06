CREATE PROCEDURE [dbo].[ReportGoalsGet]
  @companyId INT,
  @reportDate DATETIME = null,
  @singleAreaId int = null,
  @goalId UNIQUEIDENTIFIER = null
AS
BEGIN
  -- Get Report Id
  DECLARE @reportId INT
  DECLARE @reportUniqueId UNIQUEIDENTIFIER

  IF (@reportDate IS NULL)
  BEGIN
    SELECT TOP 1 @reportDate = [Date]
    FROM Report
    WHERE CompanyId = @companyId AND StatusId = 2
    ORDER BY [Date] DESC
  END

  SELECT  @reportId = r.Id,
          @reportUniqueId = r.UniqueId
  FROM Report r
  WHERE r.CompanyId = @companyId and r.[Date] = @reportDate

  SELECT g.Id, rg.Id as reportGoalId, @ReportId AS ReportId, @reportUniqueId as ReportUniqueId, ISNULL(rg.DueDate, g.DueDate) as DueDate, ISNULL(rg.Status, g.Status) AS [Status], rg.Summary,
    g.UniqueId, g.Description, g.Title, g.AreaId, @reportDate as reportDate,
    rg.CreatedDate, rg.UpdatedDate
  FROM Goal g
  LEFT JOIN ReportGoal rg ON rg.GoalId = g.Id AND rg.ReportId = @reportId
  WHERE g.CompanyId = @companyId
    AND (g.AreaId = @singleAreaId OR @singleAreaId IS NULL)
    AND (g.UniqueId = @goalId OR @goalId IS NULL)

END
