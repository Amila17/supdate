using System;
using System.Collections.Generic;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public interface IReportRepository : ICrudRepository<Report>
  {
    IEnumerable<Report> GetReportSummaryList(int companyId);

    Report GetReport(int companyId, DateTime reportDate);

    ReportEmailDetails GetReportEmailDetails(int reportEmailId);

    ReportEmailDetails GetReportEmailPreviewDetails(int reportId);

    int GetReportId(int companyId, DateTime reportDate);

    Guid GetReportGuid(int companyId, int reportId);

    IEnumerable<ReportPermalink> GetReportPermalinks(Guid reportGuid);

    IEnumerable<ReportPermalink> GetReportPermalinks(int companyId);

    void DeleteReport(int companyId, Guid reportGuid);

    Boolean CanUserViewReport(int userId, Guid reportGuid);

    Guid ReportGuidToCompanyGuid(Guid reportGuid);
  }
}
