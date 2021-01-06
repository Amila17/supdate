using System;
using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Business
{
  public interface IReportManager : IManager<Report>
  {
    int GetReportId(int companyId, DateTime reportDate);

    IEnumerable<Report> GetReportSummaryList(int companyId, LiteUser currentUser, int totalMetrics);

    Report GetReport(int companyId, DateTime reportDate, LiteUser currentUser = null);

    bool IsReportNewlyCompleted(Report report, CompanyMetadata companyMetadata);

    ReportEmailDetails GetReportEmailDetails(int reportEmailId);

    ReportEmailDetails GetReportEmailPreviewDetails(int reportId);

    IEnumerable<ReportPermalink> GetReportPermalinks(Guid reportGuid);

    IEnumerable<ReportPermalink> GetReportPermalinks(int companyId);

    void DeleteReport(int companyId, Guid uniqueId);

    bool CanUserViewReport(int userId, Guid reportGuid);

    Guid ReportGuidToCompanyGuid(Guid reportGuid);
  }
}
