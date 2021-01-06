using System;
using System.ComponentModel.DataAnnotations;

namespace Supdate.Model.Base
{
  public abstract class GoalBase : ModelBase
  {
    public DateTime? DueDate { get; set; }

    public GoalStatus Status { get; set; }

    [Editable(false)]
    public bool Overdue
    {
      get
      {
        if (!DueDate.HasValue)
        {
          return false;
        }

        if (DueDate.Value.Date < DateTime.UtcNow.Date)
        {
          return true;
        }

        return false;
      }
    }
  }
}
