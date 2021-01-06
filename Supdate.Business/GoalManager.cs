using System;
using Supdate.Data;
using Supdate.Model;

namespace Supdate.Business
{
  public class GoalManager : Manager<Goal>, IGoalManager
  {
    private readonly IGoalRepository _goalRepository;

    public GoalManager(IGoalRepository goalRepository)
      : base(goalRepository)
    {
      _goalRepository = goalRepository;
    }

    public Goal GetGoal(int companyId, Guid uniqueId)
    {
      return _goalRepository.GetGoal(companyId, uniqueId);
    }

    public void DeleteGoal(int companyId, Guid uniqueId)
    {
      _goalRepository.DeleteGoal(companyId,uniqueId);
    }
  }
}
