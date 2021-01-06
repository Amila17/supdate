using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Supdate.Business;
using Supdate.Model;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("goals")]
  public class GoalsController : AuthenticatedControllerBase
  {
    private readonly IGoalManager _goalManager;
    public GoalsController(IGoalManager goalManager,
      ListHelper listHelper)
    {
      _goalManager = goalManager;
      ListHelper = listHelper;
    }

    [Route("{areaUniqueId?}")]
    [HttpGet]
    public ActionResult Index(Guid? areaUniqueId)
    {
      ListHelper.InitializeAreas(CurrentUser);
      ListHelper.InitializeGoalStatusList();
      ListHelper.InitializeGoals(CurrentUser);

      var goalsList = ListHelper.GetGoals();

      //enable filter list if more than one reporting area
      ViewBag.FilterListShow = (CurrentUser.AccessibleAreaIds.Count() > 1);
      ViewBag.FilterListTitle = "All Goals";
      ViewBag.FilterListIndex = areaUniqueId.HasValue ? areaUniqueId.Value.ToString() : string.Empty;

      // filter list of metrics if filter is acrive
      if (areaUniqueId.HasValue)
      {
        var selectedArea = ListHelper.GetAreas().SingleOrDefault(a => a.UniqueId == areaUniqueId);
        ViewBag.FilterListTitle = selectedArea.Name + " Goals";
        ViewBag.SelectAreaName = selectedArea.Name;
        var areaId = selectedArea.Id;
        goalsList = goalsList.Where(a => a.AreaId == areaId);

      }

      return View(new GoalSettings { Goals = goalsList , ListHelper = ListHelper});
    }

    [Route("details/{uniqueId?}")]
    [HttpPost]
    public ActionResult Details(Goal goal, Guid uniqueId = default(Guid))
    {
      goal.UniqueId = uniqueId;
      if (ModelState.IsValid)
      {
        goal.CompanyId = CompanyId;

        if (goal.AreaId == -1)
        {
          goal.AreaId = null;
        }
        else
        {
          if (!CurrentUser.AccessibleAreaIds.Contains(goal.AreaId.Value))
          {
              throw new HttpException(400, "Bad Request");
          }
        }
        this.SetNotificationMessage(NotificationType.Success, "Goal successfully saved.");

        return
          Json(goal.UniqueId == Guid.Empty
            ? new { success = _goalManager.Create(goal) != null }
            : new { success = _goalManager.Update(goal) != null });
      }

      return Json(new { success = false });
    }

    [Route("details/{uniqueId}")]
    public ActionResult Details(Guid uniqueId, Guid? areaUniqueId)
    {
      ListHelper.InitializeAreas(CurrentUser);
      ListHelper.InitializeGoalStatusList();
      int? areaId = null;
      if (areaUniqueId.HasValue)
      {
        areaId = ListHelper.GetAreas().SingleOrDefault(a => a.UniqueId == areaUniqueId).Id;
      }
      var goal = new Goal{AreaId = areaId};
      if (uniqueId != Guid.Empty)
      {
        goal = _goalManager.GetGoal(CompanyId, uniqueId);
      }
      return View("_details", new GoalSettings{ListHelper = ListHelper, Goal = goal});
    }

    [Route("delete")]
    public ActionResult Delete(Guid goalId)
    {
      _goalManager.DeleteGoal(CompanyId, goalId);
      return Json(new { success = true });
    }

  }
}
