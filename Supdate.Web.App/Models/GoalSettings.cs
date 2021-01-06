using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class GoalSettings
  {
    public Goal Goal { get; set; }

    public IEnumerable<Goal> Goals { get; set; }
    public ListHelper ListHelper { get; set; }
  }
}
