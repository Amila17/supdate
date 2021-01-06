using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public class ReportGoalRepository : CrudRepository<ReportGoal>, IReportGoalRepository
  {
    public IEnumerable<ReportGoalView> GetReportGoals(int companyId, DateTime reportDate)
    {
      try
      {
        OpenConnection();

        var results = Connection.QueryMultiple("ReportGoalsGet",
          new { CompanyId = companyId, ReportDate = reportDate }, commandType: CommandType.StoredProcedure);
        var reportGoals = results.Read<ReportGoalView>().ToList();

        return reportGoals;
      }
      finally
      {
        CloseConnection();
      }
    }

    public ReportGoalView GetReportGoal(int companyId, Guid goalUniqueId, DateTime reportDate)
    {
      try
      {
        OpenConnection();

        return  Connection.Query<ReportGoalView>("ReportGoalsGet",
          new { CompanyId = companyId, GoalId = goalUniqueId, ReportDate = reportDate }, commandType: CommandType.StoredProcedure).FirstOrDefault();
      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
