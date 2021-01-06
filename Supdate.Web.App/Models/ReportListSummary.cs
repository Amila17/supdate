using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class ReportListSummary
  {
    public List<Report> Reports;
    public CompanyMetadata CompanyMetadata;
    public LiteUser CurrentUser;
  }
}
