using System;
using System.Collections.Generic;
using System.Linq;
using Supdate.Data;
using Supdate.Model;

namespace Supdate.Business
{
  public class ReportGoalManager : Manager<ReportGoal>, IReportGoalManager
  {
    private readonly IReportGoalRepository _reportGoalRepository;
    private readonly IGoalRepository _goalRepository;

    public ReportGoalManager(IReportGoalRepository reportGoalRepository, IGoalRepository goalRepository)
      : base(reportGoalRepository)
    {
      _reportGoalRepository = reportGoalRepository;
      _goalRepository = goalRepository;
    }

    public void SaveReportGoals(int companyId, List<ReportGoalView> reportGoals)
    {
      foreach (var reportGoalView in reportGoals)
      {
        var reportGoal = reportGoalView.ToReportGoal();
        if (reportGoal.Id == 0 && !string.IsNullOrWhiteSpace(reportGoal.Summary))
        {
          Create(reportGoal);
          UpdateGoalStatus(reportGoalView, companyId);
        }
        else if (reportGoal.Id != 0 && !string.IsNullOrWhiteSpace(reportGoal.Summary))
        {
          Update(reportGoal);
          UpdateGoalStatus(reportGoalView, companyId);
        }
        else if (reportGoal.Id != 0 && string.IsNullOrWhiteSpace(reportGoal.Summary))
        {
          Delete(reportGoal.Id);
        }
      }
    }

    public ReportGoalView GetReportGoal(int companyId, Guid goalUniqueId, DateTime reportDate)
    {
      return _reportGoalRepository.GetReportGoal(companyId, goalUniqueId, reportDate);
    }

    private void UpdateGoalStatus(ReportGoalView reportGoal, int companyId)
    {
      var goal = _goalRepository.Get(reportGoal.Id);
      goal.CompanyId = companyId;
      goal.Status = reportGoal.Status;
      _goalRepository.Update(goal);
    }

    public IEnumerable<ReportGoalView> GetReportGoals(int companyId, DateTime reportDate, LiteUser currentUser = null)
    {
      var results =  _reportGoalRepository.GetReportGoals(companyId, reportDate);
      if (results != null && currentUser != null && !currentUser.IsCompanyAdmin)
      {
        results = results.Where(m => m.AreaId.HasValue && currentUser.AccessibleAreaIds.Contains(m.AreaId.Value)).ToList();
      }
      return results;
    }
  }
}
