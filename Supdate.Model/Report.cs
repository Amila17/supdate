using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class Report : ModelBase
  {
    public Report()
    {
      AreaList = new List<ReportArea>();
      MetricList = new List<MetricView>();
      GoalList = new List<ReportGoalView>();
      AttachmentList = new List<ReportAttachment>();
      ReportEmails = new List<ReportEmail>();
    }

    [ReadOnly(true)]
    public Guid UniqueId { get; set; }

    public int CompanyId { get; set; }

    [Editable(false)]
    public string CompanyLogo { get; set; }

    [Editable(false)]
    public string CompanyName { get; set; }

    [Editable(false)]
    public string Title { get; set; }

    public DateTime Date { get; set; }

    public string Summary { get; set; }

    public virtual int StatusId { get; set; }

    [EnumDataType(typeof(ReportStatus))]
    [Editable(false)]
    public ReportStatus Status
    {
      get
      {
        return (ReportStatus)StatusId;
      }
      set
      {
        StatusId = (int)value;
      }
    }

    public bool IsStatusManual { get; set; }

    [Editable(false)]
    public int RecipientCount
    {
      get { return ReportEmails.Count(); }
    }

    [Editable(false)]
    public int RecipientViewedCount
    {
      get { return ReportEmails.Count(r => r.Views > 0); }
    }

    [Editable(false)]
    public int AreaCount
    {
      get { return AreaList.Count; }
    }

    [Editable(false)]
    public int MetricCount
    {
      get { return MetricList.Count(m=> m.Actual != null); }
    }

    [Editable(false)]
    public int GoalCount
    {
      get { return GoalList.Count; }
    }

    [Editable(false)]
    public int AreasCompleted
    {
      get { return AreaList != null ? AreaList.Count(r => r.Completed) : 0; }
    }

    public IList<ReportArea> AreaList { get; set; }

    public IList<MetricView> MetricList { get; set; }

    public IList<ReportGoalView> GoalList { get; set; }

    public IList<ReportAttachment> AttachmentList { get; set; }

    public IList<ReportEmail> ReportEmails { get; set; }
  }
}
