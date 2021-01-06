using System;
using Supdate.Model;

namespace Supdate.Business
{
  public interface IGoalManager : IManager<Goal>
  {
    Goal GetGoal(int companyId, Guid uniqueId);
    void DeleteGoal(int companyId, Guid uniqueId);
  }
}
