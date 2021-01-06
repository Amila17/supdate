using System;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public interface IGoalRepository : ICrudRepository<Goal>
  {
    Goal GetGoal(int companyId, Guid goalGuid);
    void DeleteGoal(int companyId, Guid goalGuid);
  }
}
