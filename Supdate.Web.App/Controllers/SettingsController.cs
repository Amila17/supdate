using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Supdate.Business;
using Supdate.Model;
using Supdate.Util;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("settings")]
  public class SettingsController : AuthenticatedControllerBase
  {
    private readonly ICompanyManager _companyManager;
    private readonly ILogoManager _logoManager;
    private readonly ISlackManager _slackManager;
    private readonly IWebhookManager _webhookManager;


    public SettingsController(ICompanyManager companyManager, ILogoManager logoManager, IWebhookManager webhookManager, ISlackManager slackManager, ListHelper listHelper)
      : base(ownerAccessOnly: true)
    {
      _webhookManager = webhookManager;
      _slackManager = slackManager;
      _companyManager = companyManager;
      _logoManager = logoManager;
      ListHelper = listHelper;
    }

    #region General

    [HttpGet]
    public ActionResult Index()
    {
      ListHelper.InitializeReportTypeList();

      // Get the company from the id.
      var company = _companyManager.Get(CompanyId);
      var companySettings = new CompanySettings { Company = company, SlackWebhooks = _slackManager.GetWebhooks(CompanyId), ListHelper = ListHelper };

      return View(companySettings);
    }

    [HttpPost]
    public ActionResult Index(CompanySettings companySettings, HttpPostedFileBase logoFile)
    {
      if (ModelState.IsValid)
      {
        if (logoFile != null && logoFile.ContentLength > 0)
        {
          var path = SaveFile(logoFile);
          companySettings.Company.LogoPath = path;
        }
        _companyManager.Update(companySettings.Company);
        this.SetNotificationMessage(NotificationType.Success, "Settings saved successfully.");

        if (companySettings.Company.EnableCommenting && (CurrentUser.DisplayName == CurrentUser.Email))
        {
          return Redirect(string.Format("{0}?comments-enabled=1", Url.Action("EditProfile", "Manage")));
        }

        return RedirectToAction("Index");
      }

      return View(companySettings);
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public ActionResult EnableDiscussions(string returnUrl)
    {
      var company = _companyManager.Get(CompanyId);
      company.EnableCommenting = true;
      _companyManager.Update(company);
      this.SetNotificationMessage(NotificationType.Success, "The Discussions feature has been enabled.");
      return Redirect(returnUrl);
    }

    [HttpPost]
    public ActionResult SaveReportTitle(string reportTitle)
    {

      var company = _companyManager.Get(CompanyId);
      company.ReportTitle = reportTitle;
      _companyManager.Update(company);
      return Json(new { success = true });
    }

    private string SaveFile(HttpPostedFileBase file)
    {
      var fileName = Path.GetFileName(file.FileName);
      var uploadedPath = _logoManager.Save(fileName, file.InputStream);

      return uploadedPath;
    }

    #endregion

    #region Wizard

    public ActionResult Wizard()
    {
      return View();
    }

    #endregion

    #region Slack Integration

    [Route("slack/start")]
    public ActionResult SlackStartOAuth()
    {
      string returnUrl = string.Format("{0}/{1}", ConfigUtil.BaseAppUrl, Url.Action("SlackReceiveCode"));
      return Redirect(_slackManager.Authorize(returnUrl));
    }

    [Route("slack/receive-code")]
    public ActionResult SlackReceiveCode(string code)
    {
      string returnUrl = string.Format("{0}/{1}", ConfigUtil.BaseAppUrl, Url.Action("SlackReceiveCode"));
      try
      {
        _slackManager.GetAccessToken(CompanyId, code, returnUrl);
        return RedirectToAction("Index", new { showSlack = 1 });
      }
      catch (Exception ex)
      {
        this.SetNotificationMessage(NotificationType.Error, string.Format("An error occurred whilst communicating with Slack: {0}", ex.Message));
        return RedirectToAction("Index");
      }
    }
    #endregion

    #region Webhooks

    [Route("webhook/{uniqueId}")]
    public ActionResult GetWebhook(Guid uniqueId)
    {
      var webhook = _webhookManager.GetList(new { CompanyId, uniqueId }).FirstOrDefault();
      if (webhook == null)
      {
        throw new HttpException(400, "Bad Request");
      }

      return View("_webhookDetails", webhook);
    }

    [HttpPost]
    [Route("webhook/{uniqueId}")]
    public ActionResult UpdateWebhookEvents(Guid uniqueId, Webhook webhook)
    {
      var originalWebhook = _webhookManager.GetList(new { CompanyId, uniqueId }).FirstOrDefault();
      if (originalWebhook == null)
      {
        throw new HttpException(400, "Bad Request");
      }

      originalWebhook.EventReportComment = webhook.EventReportComment;
      originalWebhook.EventReportViewed = webhook.EventReportViewed;
      originalWebhook.EventReportingAreaUpdated = webhook.EventReportingAreaUpdated;
      _webhookManager.Update(originalWebhook);

      return Json(new { success = true });
    }

    [HttpPost]
    public ActionResult DeleteWebhook(Guid webhookId)
    {
      var webhook = _webhookManager.GetList(new { CompanyId, UniqueId = webhookId }).FirstOrDefault();

      if (webhook == null)
      {
        throw new HttpException(400, "Bad Request");
      }

      _webhookManager.Delete(webhook.Id);

      return Json(new { success = true });
    }

    #endregion

  }
}
