using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Supdate.Business;
using Supdate.Model;
using Supdate.Util;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Filters;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("team")]
  public class TeamController : AuthenticatedControllerBase
  {
    private readonly ICompanyManager _companyManager;

    public TeamController(ICompanyManager companyManager, ListHelper listHelper)
      : base(ownerAccessOnly: true)
    {
      _companyManager = companyManager;
      ListHelper = listHelper;
    }

    [SubscriptionFilter]
    public ActionResult Index()
    {
      ListHelper.InitializeAreas(CurrentUser);
      var m = new TeamViewModel
      {
        TeamMembers = _companyManager.GetCompanyTeamMembers(CompanyId),
        Invites = _companyManager.GetCompanyTeamMemberInvites(CompanyId).Where(t => !t.ResultantUserId.HasValue),
        ListHelper = ListHelper
      };
      return View(m);
    }

    public ActionResult Intro()
    {
      return View();
    }

    [SubscriptionFilter]
    [Route("details/{uniqueId}/")]
    public ActionResult Details(Guid uniqueId)
    {
      ListHelper.InitializeAreas(CurrentUser);
      if (uniqueId == Guid.Empty)
      {
        return View("_invite", new TeamMemberViewModel {WelcomeMessage = ConfigUtil.DefaultInviteMessage, TeamMember = new LiteUser(), ListHelper = ListHelper });
      }
      var teamMember = _companyManager.GetTeamMember(CompanyId, uniqueId);
      return View("_details", new TeamMemberViewModel { TeamMember = teamMember, ListHelper = ListHelper });
    }

    [SubscriptionFilter]
    [HttpPost]
    [Route("details/{uniqueId?}")]
    public ActionResult Details(TeamMemberViewModel teamMemberViewModel, bool accessAllAreas = false, Guid uniqueId = default(Guid))
    {
      var teamMember = teamMemberViewModel.TeamMember;
      teamMember.UniqueId = uniqueId;
      teamMember.CompanyId = CompanyId;
      ListHelper.InitializeAreas(CurrentUser);
      if (uniqueId == Guid.Empty)
      {
        try
        {
          if (teamMember.AccessibleAreaIds == null && accessAllAreas)
          {
            // give access to all areas if none specified (ie, via wizard)
            ListHelper.InitializeAreas(CurrentUser);
            teamMember.AccessibleAreaIds = ListHelper.GetAreas().Select(x => x.Id).ToArray();
          }
          var registrationUrl = Url.Action("Register", "Account", new { inviteCode = "INVITE_CODE", email = "INVITE_EMAIL" });
          _companyManager.AddTeamMember(teamMember, teamMemberViewModel.WelcomeMessage, User.Identity.GetUserName(), registrationUrl);
          this.SetNotificationMessage(NotificationType.Success, teamMember.Email + " successfully invited.");
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          return Json(new { success = false, error = ex.Message });
        }
      }
      try
      {
        _companyManager.SaveTeamMember(teamMember);
      }
      catch (Exception ex)
      {
        return Json(new { success = false, error = ex.Message });
      }

      string areaListString = ListHelper.AreaIdsToString(teamMember.AccessibleAreaIds, ConfigUtil.AreaListMaxCharsForTableDisplay);

      return Json(new { success = true, teamMember, areaListString });
    }

    [SubscriptionFilter]
    [Route("delete")]
    public ActionResult Delete(Guid teamMemberId)
    {
      _companyManager.RemoveTeamMember(CompanyId, teamMemberId);
      return Json(new { success = true });
    }

    [SubscriptionFilter]
    [Route("delete-invite")]
    public ActionResult DeleteInvite(Guid inviteId)
    {
      _companyManager.DeleteInvite(CompanyId, null, inviteId);
      return Json(new { success = true });
    }

  }
}
