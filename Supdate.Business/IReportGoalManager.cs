using System;
using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Business
{
  public interface IReportGoalManager : IManager<ReportGoal>
  {
    void SaveReportGoals(int companyId, List<ReportGoalView> reportGoals);

    IEnumerable<ReportGoalView> GetReportGoals(int companyId, DateTime reportDate, LiteUser currentUser = null);

    ReportGoalView GetReportGoal(int companyId, Guid goalUniqueId, DateTime reportDate);
  }
}
