using System;
using System.Web.Mvc;
using Supdate.Business;
using Supdate.Business.Admin;
using Supdate.Web.App.Controllers.Base;

namespace Supdate.Web.App.Areas.Admin.Controllers
{
  [RouteArea("admin")]
  public class UserController : AuthenticatedControllerBase
  {
    private readonly IAdminManager _adminManager;
    private readonly ICompanyManager _companyManager;

    public UserController(IAdminManager adminManager, ICompanyManager companyManager)
    {
      _adminManager = adminManager;
      _companyManager = companyManager;
    }

    [HttpPost]
    [Route("delete-member")]
    public ActionResult DeleteMember(int memberId)
    {
      _adminManager.DeleteMemeber(memberId);

      return RedirectToAction("Users", "Admin", new { area = "Admin", page = 1 });
    }

    [HttpPost]
    [Route("remove-member")]
    public ActionResult RemoveMember(int companyId, Guid companyUniqueId, Guid userUniqueId)
    {
      _companyManager.RemoveTeamMember(companyId, userUniqueId);

      return RedirectToAction("ViewCompany", "Admin", new { area = "Admin", uniqueId = companyUniqueId });
    }

    [HttpPost]
    [Route("delete-invite")]
    public ActionResult DeleteInvite(int companyId, Guid companyUniqueId, int inviteId)
    {
      _companyManager.DeleteInvite(companyId, inviteId);

      return RedirectToAction("ViewCompany", "Admin", new { area = "Admin", uniqueId = companyUniqueId });
    }
  }
}
