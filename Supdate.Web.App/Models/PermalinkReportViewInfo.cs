using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class PermalinkReportViewInfo
  {
    public IEnumerable<ReportPermalink> ReportPermalinkList { get; set; }

    public Report Report { get; set; }
  }
}
