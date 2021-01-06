using System;
using System.Collections.Generic;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public interface IReportGoalRepository : ICrudRepository<ReportGoal>
  {
    IEnumerable<ReportGoalView> GetReportGoals(int companyId, DateTime reportDate);
    ReportGoalView GetReportGoal(int companyId, Guid goalUniqueId, DateTime reportDate);
  }
}
