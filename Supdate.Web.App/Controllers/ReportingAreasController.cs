using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Supdate.Business;
using Supdate.Model;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("reportingareas")]
  public class ReportingAreasController : AuthenticatedControllerBase
  {
    private readonly IAreaManager _areaManager;

    public ReportingAreasController(IAreaManager areaManager, ListHelper listHelper)
    {
      _areaManager = areaManager;
      ListHelper = listHelper;
    }

    public ActionResult Index()
    {
      ListHelper.InitializeMetrics(CurrentUser);
      ListHelper.InitializeGoals(CurrentUser);
      var allAreas = _areaManager.GetList(new { CompanyId });
      var areaList = allAreas.Where(a => CurrentUser.AccessibleAreaIds.Contains(a.Id));
      return View(new ReportingAreaViewModel{Areas = areaList, ListHelper = ListHelper});
    }

    [Route("details/{uniqueId?}")]
    [HttpPost]
    public ActionResult Details(Area area, Guid uniqueId = default(Guid))
    {
      area.UniqueId = uniqueId;
      if (!CurrentUser.CanManageAreas)
      {
        throw new HttpException(401, "Unauthorized");
      }

      if (ModelState.IsValid)
      {
        area.CompanyId = CompanyId;

        if (area.UniqueId == Guid.Empty)
        {
           _areaManager.Create(area);
        }
        else
        {
          _areaManager.Update(area);
        }
        this.SetNotificationMessage(NotificationType.Success, "Reporting Area successfully saved.");

        return Json(new { success = true });

      }
      return Json(new { success = false });

    }
    [Route("details/{uniqueId}")]
    public ActionResult Details(Guid uniqueId)
    {
      if (!CurrentUser.CanManageAreas)
      {
        throw new HttpException(401, "Unauthorized");
      }

      var area = new Area();
      if (uniqueId != Guid.Empty)
      {
        area = _areaManager.GetArea(CompanyId, uniqueId);
      }
      return View("_details", area);
    }

    [Route("delete")]
    public ActionResult Delete(Guid areaId)
    {
      if (!CurrentUser.CanManageAreas)
      {
        throw new HttpException(401, "Unauthorized");
      }
     _areaManager.DeleteArea(CompanyId, areaId);
      return Json(new { success = true });
    }

    [Route("save-display-order")]
    [HttpPost]
    public JsonResult SaveAreaDisplayOrder(string displayOrder)
    {
      if (!CurrentUser.CanManageAreas)
      {
        throw new HttpException(401, "Unauthorized");
      }
      var displayOrders = JsonConvert.DeserializeObject<List<EntityDisplayOrder>>(displayOrder);
      var success = false;

      if (ModelState.IsValid)
      {
        _areaManager.SaveDisplayOrder(CompanyId, displayOrders);
        success = true;
      }

      return Json(new { success });
    }

  }
}
