using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public class ReportRepository : CrudRepository<Report>, IReportRepository
  {
    public IEnumerable<Report> GetReportSummaryList(int companyId)
    {
      try
      {
        OpenConnection();

        var results = Connection.QueryMultiple("ReportsSummaryGet", new { CompanyId = companyId }, commandType: CommandType.StoredProcedure);

        var reportSummaryList = results.Read<Report>().ToList();
        var reportAreas = results.Read<ReportArea>().ToList();
        var reportGoals = results.Read<ReportGoalView>().ToList();
        var metricViews = results.Read<MetricView>().ToList();
        var reportAttachments = results.Read<ReportAttachment>().ToList();
        var reportEmails = results.Read<ReportEmail>().ToList();
        var recipients = results.Read<Recipient>().ToList();

        // a new list to contain single reportEmail per report/recipient combo
        var reportEmailsClean = new List<ReportEmail>();

        foreach (var reportEmail in reportEmails.OrderByDescending(r => r.Views))
        {
          if (reportEmailsClean.FindIndex(r => (r.ReportId == reportEmail.ReportId) && (r.RecipientId == reportEmail.RecipientId)) < 0)
          {
            reportEmail.Recipient = recipients.FirstOrDefault(r => r.Id == reportEmail.RecipientId);
            reportEmailsClean.Add(reportEmail);
          }
        }

        reportSummaryList = reportSummaryList.Select(r =>
                                                     {
                                                       r.AreaList = reportAreas.Where(ra => ra.ReportId == r.Id).ToList();
                                                       r.GoalList = reportGoals.Where(rg => rg.ReportId == r.Id).ToList();
                                                       r.MetricList = metricViews.Where(rm => rm.ReportId == r.Id).ToList();
                                                       r.AttachmentList = reportAttachments.Where(ra => ra.ReportId == r.Id).ToList();
                                                       r.ReportEmails = reportEmailsClean.Where(re => re.ReportId == r.Id).ToList();

                                                       return r;
                                                     }).ToList();

        return reportSummaryList;
      }
      finally
      {
        CloseConnection();
      }
    }

    public Report GetReport(int companyId, DateTime reportDate)
    {
      try
      {
        OpenConnection();

        var results = Connection.QueryMultiple("ReportGetByMonth", new { CompanyId = companyId, ReportDate = reportDate }, commandType: CommandType.StoredProcedure);

        var report = results.Read<Report>().FirstOrDefault();
        if (report != null)
        {
          var reportAreas = results.Read<ReportArea>().ToList();
          report.GoalList = results.Read<ReportGoalView>().ToList();
          report.MetricList = results.Read<MetricView>().ToList();
          report.AttachmentList = results.Read<ReportAttachment>().ToList();

          foreach (var reportArea in reportAreas)
          {
            reportArea.ReportGoalList = report.GoalList.Where(rg => rg.AreaId == reportArea.AreaId).ToList();
            reportArea.MetricList = report.MetricList.Where(rm => rm.AreaId == reportArea.AreaId).ToList();
            reportArea.Attachments = report.AttachmentList.Where(a => a.AreaId == reportArea.AreaId).ToList();
            report.AreaList.Add(reportArea);
          }
        }

        return report;
      }
      finally
      {
        CloseConnection();
      }
    }

    public ReportEmailDetails GetReportEmailDetails(int reportEmailId)
    {
      try
      {
        OpenConnection();
        var reportEmailDetails = Connection.Query<ReportEmailDetails>("ReportEmailDetailsGet", new { ReportEmailId = reportEmailId }, commandType: CommandType.StoredProcedure);

        return reportEmailDetails.FirstOrDefault();
      }
      finally
      {
        CloseConnection();
      }
    }

    public ReportEmailDetails GetReportEmailPreviewDetails(int reportId)
    {
      try
      {
        OpenConnection();
        var reportEmailDetails = Connection.Query<ReportEmailDetails>("ReportEmailPreviewDetailsGet", new { ReportId = reportId }, commandType: CommandType.StoredProcedure);

        return reportEmailDetails.FirstOrDefault();
      }
      finally
      {
        CloseConnection();
      }
    }

    public int GetReportId(int companyId, DateTime reportDate)
    {
      try
      {
        OpenConnection();
        var reportId = Connection.Query<int>("ReportIdGetByMonth", new { CompanyId = companyId, ReportDate = reportDate }, commandType: CommandType.StoredProcedure).First();

        return reportId;
      }
      finally
      {
        CloseConnection();
      }
    }

    public Guid GetReportGuid(int companyId, int reportId)
    {
      try
      {
        OpenConnection();
        var reportGuid = Connection.Query<Guid>("ReportGuidGetById", new { CompanyId = companyId, ReportId = reportId }, commandType: CommandType.StoredProcedure).Single();

        return reportGuid;
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<ReportPermalink> GetReportPermalinks(Guid reportGuid)
    {
      try
      {
        OpenConnection();
        var result = Connection.QueryMultiple("ReportPermalinksGetByReportGuid", new { reportGuid }, commandType: CommandType.StoredProcedure);

        return result.Read<ReportPermalink>().ToList();
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<ReportPermalink> GetReportPermalinks(int companyId)
    {
      try
      {
        OpenConnection();
        var result = Connection.QueryMultiple("ReportPermalinksGetByCompanyId", new { companyId }, commandType: CommandType.StoredProcedure);

        return result.Read<ReportPermalink>().ToList();
      }
      finally
      {
        CloseConnection();
      }
    }

    public void DeleteReport(int companyId, Guid reportGuid)
    {
      try
      {
        OpenConnection();
        Connection.Execute("ReportDeleteByGuid", new { companyId, reportGuid }, commandType: CommandType.StoredProcedure);
      }
      finally
      {
        CloseConnection();
      }
    }

    public Boolean CanUserViewReport(int userId, Guid reportGuid)
    {
      try
      {
        OpenConnection();
        var result = Connection.Query<Boolean>("ReportIsViewableByUser", new { userId, reportGuid }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        return result;
      }
      finally
      {
        CloseConnection();
      }
    }

    public Guid ReportGuidToCompanyGuid(Guid reportGuid)
    {
      try
      {
        OpenConnection();
        var result = Connection.Query<Guid>("ReportGuidToCompanyGuid", new { reportGuid }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        return result;
      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
