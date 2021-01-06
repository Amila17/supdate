CREATE PROCEDURE [dbo].[ReportsSummaryGet]
  @companyId int = 0
AS
BEGIN

  -- Return all reports
  SELECT Id, [StatusId], [Date], @companyId, Summary, UniqueId
  FROM Report
  WHERE CompanyId = @companyId

  -- Returns minimal data of report areas
  SELECT ra.Id, ra.AreaId, ra.ReportId, ra.Summary
  FROM ReportArea ra
  INNER JOIN Report r ON r.Id = ra.ReportId
  INNER JOIN Area a ON a.Id = ra.AreaId
  WHERE r.CompanyId = @CompanyId
  ORDER BY a.DisplayOrder, a.Id

  -- Returns minimal data of report goals
  SELECT rg.Id, rg.GoalId, rg.ReportId, g.AreaId
  FROM ReportGoal rg
  INNER JOIN Report r ON r.Id = rg.ReportId
  INNER JOIN Goal g ON g.Id = rg.GoalId
  WHERE r.CompanyId = @CompanyId

  -- Returns minimal data of report metrics
  SELECT mdp.Id, mdp.MetricId, r.Id AS ReportId, m.AreaId, mdp.Actual
  FROM MetricDataPoint mdp
  INNER JOIN Metric m ON m.Id = mdp.MetricId
  INNER JOIN Report r ON r.Date = mdp.Date
  WHERE r.CompanyId = @CompanyId AND m.CompanyId = @companyId

  -- Returns data of report attachments
  SELECT ra.Id, ra.FileName, ra.FilePath, ra.MimeType, ra.ReportId, ra.AreaId
  FROM ReportAttachment ra
  INNER JOIN Report r ON r.Id = ra.ReportId
  WHERE r.CompanyId = @CompanyId

  -- Returns data of report emails
  SELECT re.Id, re.UniqueId, re.[Views], re.LastViewedDate, re.RecipientId, re.ReportId, re.CompanyId
  FROM ReportEmail re
  INNER JOIN Report r ON r.Id = re.ReportId
  WHERE r.CompanyId = @CompanyId

  -- Returns all recipients to be matched to report emails
  SELECT rc.Id, rc.FirstName, rc.LastName, rc.Email
  FROM Recipient rc
  WHERE rc.CompanyId = @CompanyId
END
