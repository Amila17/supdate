using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class Dashboard
  {
    public IEnumerable<Report> ReportSummaries { get; set; }
    public IEnumerable<MetricView> MetricViews { get; set; }
    public ListHelper ListHelper;
    public bool ShowWizard { get; set; }
    public bool PromoteTeams { get; set; }
    public bool PromoteDataSources { get; set; }
  }
}
