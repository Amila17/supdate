using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class ReportArea : ModelBase
  {
    public int AreaId { get; set; }

    [Editable(false)]
    public Guid AreaUniqueId { get; set; }
    public int ReportId { get; set; }

    public string Summary { get; set; }

    [Editable(false)]
    public bool Completed
    {
      get
      {
        return (!string.IsNullOrWhiteSpace(Summary));
      }
    }

    [Editable(false)]
    public DateTime ReportDate { get; set; }

    [Editable(false)]
    public string AreaName { get; set; }

    public IList<MetricView> MetricList { get; set; }

    public IList<ReportGoalView> ReportGoalList { get; set; }

    public IList<ReportAttachment> Attachments { get; set; }

  }
}
