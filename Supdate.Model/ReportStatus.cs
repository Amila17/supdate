using System.ComponentModel.DataAnnotations;

namespace Supdate.Model
{
  public enum ReportStatus
  {
    [Display(Name = "Not Started")]
    NotStarted = 0,

    [Display(Name = "In Progress")]
    InProgress = 1,

    [Display(Name = "Completed")]
    Completed = 2
  }
}
