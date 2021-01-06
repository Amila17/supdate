using Supdate.Model;
using Supdate.Model.Base;

namespace Supdate.Web.App.Models
{
  public static class ModelExtensions
  {
    public static string StatusClass(this GoalBase goal)
    {
      var goalStatusClass = string.Empty;

      switch (goal.Status)
      {
        case GoalStatus.NotStarted:
          goalStatusClass = "goal-status-not-started";
          break;
        case GoalStatus.InProgressOnSchedule:
          goalStatusClass = "goal-status-on-schedule";
          break;
        case GoalStatus.InProgressDelayed:
          goalStatusClass = "goal-status-delayed";
          break;
        case GoalStatus.Completed:
          goalStatusClass = "goal-status-completed";
          break;
        case GoalStatus.Cancelled:
          goalStatusClass = "goal-status-cancelled";
          break;
      }

      return goalStatusClass;
    }
  }
}