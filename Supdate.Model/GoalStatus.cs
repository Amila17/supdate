using System.ComponentModel.DataAnnotations;

namespace Supdate.Model
{
  public enum GoalStatus
  {
    [Display(Name = "Not Started")]
    NotStarted = 0,
    
    [Display(Name = "On schedule")]
    InProgressOnSchedule,

    [Display(Name = "Delayed")]
    InProgressDelayed,

    [Display(Name = "Completed")]
    Completed,

    [Display(Name = "Cancelled")]
    Cancelled
  }
}
