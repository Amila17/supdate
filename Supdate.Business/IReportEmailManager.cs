using System;
using Supdate.Model;

namespace Supdate.Business
{
  public interface IReportEmailManager : IManager<ReportEmail>
  {
    string ParseReportEmailBody(LiteUser currentUser, Company company, Report report, Recipient recipient, ReportEmailBuilder reportEmailBuilder);
    string ParseReportEmailSubject(Company company, Report report, string userSubject);
    void SendReports(LiteUser currentUser, Company company, Report report, ReportEmailBuilder reportEmailBuilder);
    ReportEmail LogReportView(Guid reportEmailGuid);
    ReportEmail GetByEmailAddress(int companyId, int reportId, string emailAddress);
    ReportEmail GetByGuid(Guid reportEmailGuid);
  }
}
