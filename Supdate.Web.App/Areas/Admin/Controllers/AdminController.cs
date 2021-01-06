using System;
using System.Web.Mvc;
using Supdate.Business;
using Supdate.Business.Admin;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Areas.Admin.Controllers
{
  [RouteArea("admin")]
  public class AdminController : AuthenticatedControllerBase
  {
    private readonly IAdminManager _adminManager;
    private readonly IReportManager _reportManager;

    public AdminController(IAdminManager adminManager, IReportManager reportManager) : base(adminAccessOnly: true)
    {
      _adminManager = adminManager;
      _reportManager = reportManager;
    }

    public ActionResult Index()
    {
      return RedirectToAction("Dashboard");
    }

    [Route("dashboard/{windowInDays?}")]
    public ActionResult Dashboard(int? windowInDays = 0)
    {
      var registrationStatistics = _adminManager.GetRegistrationStatistics(windowInDays.HasValue ? windowInDays.Value : 0);

      return View(registrationStatistics);
    }

    [Route("companies/{page}/{records?}")]
    public ActionResult Companies(int page = 1, int records = 0)
    {
      if (records == 0)
      {
        records = 20;
        int cookieRecords = CookieUtil.AdminRecordsPerPage;
        if (cookieRecords > 0)
        {
          records = cookieRecords;
        }
      }

      CookieUtil.AdminRecordsPerPage = records;

      // Get the users from the database.
      var users = _adminManager.GetRegisteredCompanies(page, records);

      // Save the page and records for use in the view.
      ViewBag.page = page;
      ViewBag.records = records;

      return View(users);
    }

    [Route("companies/{uniqueId:Guid}/details")]
    public ActionResult ViewCompany(Guid uniqueId)
    {
      var c = _adminManager.GetCompanyDetails(uniqueId);
      c.Permalinks = _reportManager.GetReportPermalinks(c.Id);
      return View(c);
    }

    [Route("users/{page:int}/{records=20}/{sortOption=0}")]
    public ActionResult Users(int page, int records = 0, int sortOption = 0)
    {
      if (records == 0)
      {
        records = 20;
        int cookieRecords = CookieUtil.AdminRecordsPerPage;
        if (cookieRecords > 0)
        {
          records = cookieRecords;
        }
      }

      CookieUtil.AdminRecordsPerPage = records;

      // Get the users from the database.
      var users = _adminManager.GetRegisteredUsers(page, records, sortOption);

      // Save the page and records for use in the view.
      ViewBag.page = page;
      ViewBag.records = records;
      ViewBag.sortOption = sortOption;

      return View(users);
    }

    [Route("users/{uniqueId:Guid}")]
    public ActionResult ViewUser(Guid uniqueId)
    {
      var user = _adminManager.GetUserDetails(uniqueId, true);

      return View(user);
    }

    [Route("users-unconfirmed")]
    public ActionResult Unconfirmed()
    {
      var users = _adminManager.GetUnconfirmedUsers(0);

      return View(users);
    }

    [Route("recent-reports/{records}")]
    public ActionResult RecentReports(int records)
    {
      ViewBag.records = records;
      var rr = _adminManager.GetRecentReports(records);

      return View(rr);
    }

    [HttpPost]
    [Route("search")]
    public ActionResult Search(string query)
    {
      var searchResults = _adminManager.Search(query);
      return View(searchResults);
    }

    [Route("companies/{uniqueId}/delete")]
    public ActionResult DeleteCompanyForm(Guid uniqueId)
    {
      var company = _adminManager.GetCompanyDetails(uniqueId);
      return View(company);
    }

    [HttpPost]
    [Route("delete")]
    public ActionResult DeleteCompany(Guid uniqueId, string name)
    {
      var company = _adminManager.GetCompanyDetails(uniqueId);
      if (_adminManager.DeleteCompany(company.Id, name))
      {
        this.SetNotificationMessage(NotificationType.Success, "The company was successfully deleted");
        return RedirectToAction("Dashboard");
      }

      this.SetNotificationMessage(NotificationType.Error, "The company was NOT deleted. Try again");
      return RedirectToAction("ViewCompany", new {uniqueId});

    }
  }
}
