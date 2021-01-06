using System;
using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class ReportGoalInfo
  {
    public DateTime ReportDate { get; set; }

    public IList<ReportGoalView> ReportGoalList { get; set; }

    public ListHelper ListHelper { get; set; }
  }
}
