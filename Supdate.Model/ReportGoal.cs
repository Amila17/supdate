using System;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class ReportGoal : GoalBase
  {
    public int GoalId { get; set; }

    public int ReportId { get; set; }

    [Editable(false)]
    public Guid ReportUniqueId { get; set; }

    [Editable(false)]
    public DateTime ReportDate { get; set; }

    /// <summary>
    /// Provides update of a particular month report area
    /// </summary>
    public string Summary { get; set; }
  }
}
