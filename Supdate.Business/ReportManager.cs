using System;
using System.Collections.Generic;
using System.Linq;
using Supdate.Data;
using Supdate.Model;

namespace Supdate.Business
{
  public class ReportManager : Manager<Report>, IReportManager
  {
    private readonly ICompanyRepository _companyRepository;
    private readonly IReportRepository _reportRepository;
    private const string DefaultReportTitle = "Shareholder Update";

    public ReportManager(ICompanyRepository companyRepository, IReportRepository reportRepository)
      : base(reportRepository)
    {
      _companyRepository = companyRepository;
      _reportRepository = reportRepository;
    }

    public IEnumerable<Report> GetReportSummaryList(int companyId, LiteUser currentUser, int totalMetrics)
    {
      const string dateFormat = "yyyy MMMM";
      var reportSummaryList = _reportRepository.GetReportSummaryList(companyId).ToList();

      // Remove areas this user can't access
      if (!currentUser.IsCompanyAdmin)
      {
        foreach (var report in reportSummaryList)
        {
          report.AreaList = report.AreaList.Where(a => currentUser.AccessibleAreaIds.Contains(a.AreaId)).ToList();
          report.MetricList = report.MetricList.Where(a => a.AreaId.HasValue && currentUser.AccessibleAreaIds.Contains(a.AreaId.Value)).ToList();
          report.GoalList = report.GoalList.Where(a => a.AreaId.HasValue && currentUser.AccessibleAreaIds.Contains(a.AreaId.Value)).ToList();

          if (report.Status == ReportStatus.InProgress)
          {
            // Override status and set it to complete if user has done all of their areas
            if (report.AreasCompleted == currentUser.AccessibleAreaIds.Count() && report.MetricCount == totalMetrics)
            {
              report.Status = ReportStatus.Completed;
            }
          }
        }
      }

      // Add missing months.
      var company = _companyRepository.Get(companyId);
      var companyStartDate = company.StartMonth ?? DateTime.Now;
      var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

      // Setting report date for the months which are in the range of company start date to current date.
      while (companyStartDate <= currentDate)
      {
        if (reportSummaryList.All(r => r.Date.ToString(dateFormat) != companyStartDate.ToString(dateFormat)))
        {
          // If report for this month not exist, building dummy report
          reportSummaryList.Add(new Report { CompanyId = company.Id, Date = companyStartDate });
        }

        companyStartDate = companyStartDate.AddMonths(1);
      }


      return reportSummaryList;
    }

    public Report GetReport(int companyId, DateTime reportDate, LiteUser currentUser = null)
    {
      var report = _reportRepository.GetReport(companyId, reportDate);

      if (report != null && string.IsNullOrWhiteSpace(report.Title))
      {
        report.Title = DefaultReportTitle;
      }

      if (report != null && currentUser != null && !currentUser.IsCompanyAdmin)
      {
        report.MetricList = report.MetricList.Where(m => m.AreaId.HasValue && currentUser.AccessibleAreaIds.Contains(m.AreaId.Value)).ToList();
        report.GoalList = report.GoalList.Where(g => g.AreaId.HasValue && currentUser.AccessibleAreaIds.Contains(g.AreaId.Value)).ToList();
        report.AreaList = report.AreaList.Where(g => currentUser.AccessibleAreaIds.Contains(g.AreaId)).ToList();
      }

      return report;
    }

    public bool IsReportNewlyCompleted(Report report, CompanyMetadata companyMetadata)
    {
      // We don't change the report status if it's already set manually or already complete
      if (report.IsStatusManual || report.Status == ReportStatus.Completed)
      {
        return false;
      }

      if (!string.IsNullOrWhiteSpace(report.Summary) && companyMetadata.AreaCount == report.AreaCount && companyMetadata.MetricCount == report.MetricCount)
      {
        report.Status = ReportStatus.Completed;
        Update(report);

        return true;
      }

      return false;
    }


    public ReportEmailDetails GetReportEmailDetails(int reportEmailId)
    {
      return _reportRepository.GetReportEmailDetails(reportEmailId);
    }

    public ReportEmailDetails GetReportEmailPreviewDetails(int reportId)
    {
      return _reportRepository.GetReportEmailPreviewDetails(reportId);
    }

    public int GetReportId(int companyId, DateTime reportDate)
    {
      return _reportRepository.GetReportId(companyId, reportDate);
    }

    public IEnumerable<ReportPermalink> GetReportPermalinks(Guid reportGuid)
    {
      return _reportRepository.GetReportPermalinks(reportGuid);
    }

    public IEnumerable<ReportPermalink> GetReportPermalinks(int companyId)
    {
      return _reportRepository.GetReportPermalinks(companyId);
    }

    public void DeleteReport(int companyId, Guid uniqueId)
    {
      _reportRepository.DeleteReport(companyId, uniqueId);
    }

    public bool CanUserViewReport(int userId, Guid reportGuid)
    {
      return _reportRepository.CanUserViewReport(userId, reportGuid);
    }

    public Guid ReportGuidToCompanyGuid(Guid reportGuid)
    {
      return _reportRepository.ReportGuidToCompanyGuid(reportGuid);
    }
  }
}
