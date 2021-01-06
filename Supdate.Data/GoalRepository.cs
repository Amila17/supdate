using System;
using System.Data;
using System.Linq;
using Dapper;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public class GoalRepository : CrudRepository<Goal>, IGoalRepository
  {
    public override Goal Update(Goal model)
    {
      var modelCheck = GetGoal(model.CompanyId, model.UniqueId);
      if (modelCheck.Id == model.Id)
      {
        return base.Update(model);
      }
      return model;
    }

    public void DeleteGoal(int companyId, Guid goalGuid)
    {
      var goal = GetGoal(companyId, goalGuid);
      Delete(goal.Id);
    }

    public override bool Delete(int id)
    {
      try
      {
        OpenConnection();

        return Connection.Execute("GoalDeleteById", new { goalId = id }, commandType: CommandType.StoredProcedure) > 0;
      }
      finally
      {
        CloseConnection();
      }
    }

    public Goal GetGoal(int companyId, Guid goalGuid)
    {
      try
      {
        OpenConnection();
        var results = Connection.QueryMultiple("GoalGet", new { companyId, goalGuid }, commandType: CommandType.StoredProcedure);
        var goal = results.Read<Goal>().FirstOrDefault();
        return goal;
      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
