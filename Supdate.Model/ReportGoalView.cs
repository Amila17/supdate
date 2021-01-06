using System;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class ReportGoalView : Goal
  {
    public int ReportGoalId { get; set; }

    public int ReportId { get; set; }
    public Guid ReportUniqueId { get; set; }
    /// <summary>
    /// Provides update of a particular month report area
    /// </summary>
    public string Summary { get; set; }

    public string AreaName { get; set; }

    public DateTime ReportDate { get; set; }


    public ReportGoal ToReportGoal()
    {
      return new ReportGoal { Id = ReportGoalId, GoalId = Id, ReportId = ReportId, ReportUniqueId = ReportUniqueId, DueDate = DueDate, Status = Status, Summary = Summary };
    }
  }
}
