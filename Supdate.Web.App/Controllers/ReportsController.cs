using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using Supdate.Business;
using Supdate.Model;
using Supdate.Util;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("reports")]
  public class ReportsController : AuthenticatedControllerBase
  {
    private readonly IReportManager _reportManager;
    private readonly IMetricManager _metricManager;
    private readonly ICompanyManager _companyManager;
    private readonly IReportGoalManager _reportGoalManager;
    private readonly IReportAreaManager _reportAreaManager;
    private readonly IRecipientManager _recipientManager;
    private readonly IReportEmailManager _reportEmailManager;
    private readonly IAreaManager _areaManager;
    private readonly IReportAttachmentManager _reportAttachmentManager;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IWebhookManager _webhookManager;

    public ReportsController(IReportManager reportManager, IMetricManager metricManager, IReportGoalManager reportGoalManager,
      ICompanyManager companyManager, IReportAreaManager reportAreaManager, ListHelper listHelper,
      IRecipientManager recipientManager, IReportEmailManager reportEmailManager, IAreaManager areaManager,
      IReportAttachmentManager reportAttachmentManager, ISubscriptionManager subscriptionManager, IWebhookManager webhookManager)
    {
      _reportManager = reportManager;
      _metricManager = metricManager;
      _companyManager = companyManager;
      ListHelper = listHelper;
      _reportGoalManager = reportGoalManager;
      _reportAreaManager = reportAreaManager;
      _recipientManager = recipientManager;
      _reportEmailManager = reportEmailManager;
      _areaManager = areaManager;
      _reportAttachmentManager = reportAttachmentManager;
      _subscriptionManager = subscriptionManager;
      _webhookManager = webhookManager;
    }

    #region Report Main Page

    [HttpGet]
    public ActionResult Index(int? filter)
    {
      InitializeMasterLists(CurrentUser);

      var reportSummaries = _reportManager.GetReportSummaryList(CompanyId, CurrentUser, ListHelper.GetMetrics().Count()).ToList();
      var filterString = "All Reports";

      if (filter.HasValue)
      {
        var reportFiler = (ReportStatus)filter;
        switch (reportFiler)
        {
          case ReportStatus.Completed:
            reportSummaries.RemoveAll(r => r.Status != ReportStatus.Completed);
            filterString = "Completed Reports";
            break;

          case ReportStatus.InProgress:
            reportSummaries.RemoveAll(r => r.Status != ReportStatus.InProgress);
            filterString = "Reports In Progress";
            break;

          case ReportStatus.NotStarted:
            reportSummaries.RemoveAll(r => r.Status != ReportStatus.NotStarted);
            filterString = "Reports Not Started";
            break;
        }
      }

      ViewBag.filterString = filterString;
      ViewBag.filter = filter;

      return View(new ReportListSummary
                  {
                    Reports = reportSummaries.OrderByDescending(r => r.Date).ToList(),
                    CompanyMetadata = CompanyMetadata,
                    CurrentUser = CurrentUser
                  });
    }

    [Route("{year}/{month}/edit")]
    [HttpGet]
    public ActionResult Edit(int year, int month)
    {
      ViewBag.NewlyComplete = false;
      var reportDate = new DateTime(year, month, 1);
      int[] accessibleAreaIds = (CurrentUser.IsCompanyAdmin) ? null : CurrentUser.AccessibleAreaIds;
      var report = _reportManager.GetReport(CompanyId, reportDate, CurrentUser);
      InitializeMasterLists(CurrentUser);

      if (report == null)
      {
        report = _reportManager.Create(new Report
        {
          CompanyId = CompanyId,
          Date = reportDate,
          Status = ReportStatus.InProgress

        });

        report.MetricList = _metricManager.GetMetricViews(CompanyId, reportDate, CurrentUser).ToList();
      }
      else if (report.Status != ReportStatus.Completed && CurrentUser.IsCompanyAdmin)
      {
        if (_reportManager.IsReportNewlyCompleted(report, CompanyMetadata))
        {
          report.Status = ReportStatus.Completed;
          ViewBag.NewlyComplete = true;
          _reportManager.Update(report);
        }
      }

      var reportSummary = new ReportSummary
      {
        Report = report,
        CompanyMetadata = CompanyMetadata,
        ListHelper = ListHelper,
        CurrentUser = CurrentUser
      };

      if ((GetPanelCount(reportSummary) % 2) == 0) // There are an even number of panels shown
      {
        // Change width of Attachments panel to make things look nice
        ViewBag.AttachmentsColWidth = 6;
      }
      return View(reportSummary);
    }

    private int GetPanelCount(ReportSummary model)
    {
      var panelCount = 1; // Areas panel is always shown
      if (model.ListHelper.GetMetrics().Any())
      {
        panelCount++; // Metrics panel will be shown
      }

      if (model.ListHelper.GetGoals().Any())
      {
        panelCount++; // Goals panel will be shown
      }

      if (CurrentUser.CanWriteReportSummary)
      {
        panelCount++; // Summary panel will be shown
      }

      if (CurrentUser.IsCompanyAdmin)
      {
        panelCount++; // Attachments panel will be shown
      }

      return panelCount;
    }

    [Route("{year}/{month}/status")]
    [HttpPost]
    public ActionResult Status(int year, int month, int statusId)
    {
      var reportDate = new DateTime(year, month, 1);
      var report = _reportManager.GetReport(CompanyId, reportDate);
      report.Status = (ReportStatus)statusId;
      report.IsStatusManual = true;
      _reportManager.Update(report);

      return Json(new { success = true });
    }

    [Route("summary/edit")]
    [HttpPost]
    public ActionResult ReportSummary(int year, int month, string summary)
    {
      summary = HttpUtility.HtmlDecode(summary);
      var newlyCompleted = false;
      if (CurrentUser.CanWriteReportSummary)
      {
        var reportDate = new DateTime(year, month, 1);
        var report = _reportManager.GetReport(CompanyId, reportDate);
        report.Summary = summary;
        _reportManager.Update(report);

        if (report.Status != ReportStatus.Completed && CurrentUser.IsCompanyAdmin)
        {
          InitializeMasterLists(CurrentUser);
          newlyCompleted = _reportManager.IsReportNewlyCompleted(report, CompanyMetadata);
          if (newlyCompleted)
          {
            report.Status = ReportStatus.Completed;
            _reportManager.Update(report);
          }
        }
      }

      return Json(new { success = true, value = HttpUtility.HtmlEncode(summary), newlyCompleted });
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("email/{reportEmailGuid}/{viewKeyGuid}")]
    public ActionResult ReportEmail(Guid reportEmailGuid, Guid viewKeyGuid)
    {
      var reportEmail = _reportEmailManager.GetByGuid(reportEmailGuid);

      if (reportEmail != null && reportEmail.ViewKey == viewKeyGuid)
      {
        // Set the cookie so they can view reports for this company
        HttpCookie securityCookie = new HttpCookie(string.Format("reportView_{0}", reportEmail.CompanyGuid)) { Expires = DateTime.Now.AddMonths(1) };
        Response.Cookies.Add(securityCookie);

        // Redirect to the report
        var reportUrl = string.Format("{0}{1}", Url.Action("ReportView", new { id = reportEmail.ReportGuid, reportEmailGuid = reportEmail.UniqueId }), Request.Url.Query);
        return Redirect(reportUrl);
      }

      return View("NoPageFound");
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{id:guid}/{reportEmailGuid?}")]
    public ActionResult ReportView(Guid id, string reportEmailGuid = null, bool requireLogin = false, bool editEnabled = false)
    {
      InitializeContext();

      // for requests that explicitly say user must be logged in
      if (requireLogin && !Request.IsAuthenticated)
      {
        return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });
      }

      var reportViewable = false;
      if (Request.IsAuthenticated)
      {
        //can currently logged in user view this report?
        reportViewable = _reportManager.CanUserViewReport(CurrentUser.Id, id);
      }

      if (!reportViewable)
      {
        // either request is not authenticated, or user is not authorised for this report
        // so check for an authorisation cookie

        // we need the company guid to look for a cookie
        var companyGuid = _reportManager.ReportGuidToCompanyGuid(id);
        if (Request.Cookies.Get(string.Format("reportView_{0}", companyGuid)) != null)
        {
          // Cookie exists, they can view the report.
          reportViewable = true;
        }
      }

      if (!reportViewable)
      {
        // this person can't view this report
        return View("UnauthorisedReportLink");
      }

      var reportPermalinkList = _reportManager.GetReportPermalinks(id);
      var reportPermalinks = reportPermalinkList as IList<ReportPermalink> ?? reportPermalinkList.ToList();
      var canComment = false;
      string commenterName = null;
      string commenterEmail = null;
      var deleteOwnCommentsOnly = 1;

      // Remove all the incomplete reports.
      for (var i = reportPermalinks.Count - 1; i >= 0; i--)
      {
        if ((reportPermalinks[i].Status != ReportStatus.Completed) && (reportPermalinks[i].UniqueId != id))
        {
          reportPermalinks.RemoveAt(i);
        }
      }

      var currentReport = reportPermalinks.FirstOrDefault(rp => rp.UniqueId == id);

      if (currentReport == null)
      {
        return View("NoPageFound");
      }

      // Remove the current report from the dropdown.
      reportPermalinks.Remove(currentReport);

      // Get the details of the report to be rendered.
      var report = _reportManager.GetReport(currentReport.CompanyId, currentReport.Date);
      var company = _companyManager.Get(report.CompanyId);

      // Disable commenting if there isn't a valid subscription
      if (company.EnableCommenting)
      {
        var subscription = _subscriptionManager.GetSubscription(report.CompanyId);

        // If for some reason a subscription does not exist (the user hasn't logged in since the subscriptions were introduced)
        // or if the user doesn't have an active subscription
        if (subscription == null || !subscription.IsActive())
        {
          company.EnableCommenting = false;
        }
      }

      if (reportEmailGuid != null)
      {
        var reportEmail = _reportEmailManager.LogReportView(Guid.Parse(reportEmailGuid));

        if (reportEmail != null && company.EnableCommenting)
        {
          canComment = true;
          commenterName = reportEmail.Recipient.DisplayName;
          commenterEmail = reportEmail.Recipient.Email;
        }
      }

      // Initialize.
      ViewBag.CanToggleEdit = false;
      ViewBag.CanEdit = false;

      if (Request.IsAuthenticated)
      {
        // determine if viewer is owner of report so is able to edit
        ViewBag.CanToggleEdit = (report.CompanyId == CompanyId && CurrentUser.IsCompanyAdmin);
        ViewBag.CanEdit = (report.CompanyId == CompanyId && CurrentUser.IsCompanyAdmin && editEnabled);

        // determine if viewer is owner of report so is able to comment
        if (canComment == false && company.EnableCommenting)
        {
          // reportEmail details take priority over logged in user details
          canComment = true;

          if (report.CompanyId == CompanyId)
          {
            deleteOwnCommentsOnly = 0;
          }

          commenterName = CurrentUser.DisplayName;
          commenterEmail = CurrentUser.Email;
        }
      }

      ViewBag.CanComment = canComment;
      ViewBag.CommenterName = commenterName;
      ViewBag.CommenterEmail = commenterEmail;
      ViewBag.DeleteOwnCommentsOnly = deleteOwnCommentsOnly;

      return View("ReportView", new PermalinkReportViewInfo { ReportPermalinkList = reportPermalinks.OrderByDescending(r => r.Date), Report = report });
    }

    [HttpPost]
    public ActionResult Delete(Guid uniqueId)
    {
      if (CurrentUser.IsCompanyAdmin)
      {
        _reportManager.DeleteReport(CompanyId, uniqueId);
        this.SetNotificationMessage(NotificationType.Success, "Report successfully deleted.");
      }
      return RedirectToAction("Index", "Reports");
    }

    #endregion

    #region Graph Data

    [HttpGet]
    [AllowAnonymous]
    [Route("graphdata/{reportUniqueId}")]
    public JsonResult MetricGraphData(string reportUniqueId)
    {
      var graphData = _metricManager.GraphData(0, Guid.Parse(reportUniqueId));
      JsonResult result = Json(graphData);
      result.MaxJsonLength = int.MaxValue;
      result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return result;
    }

    #endregion

    #region Report Metrics

    [Route("{year}/{month}/metrics")]
    [HttpGet]
    public ActionResult Metrics(int year, int month)
    {
      ListHelper.InitializeAreas(CurrentUser);
      ListHelper.InitializeMetrics(CurrentUser);
      ListHelper.InitializeMetricDataSources(CompanyId);
      ViewBag.ShowImportButton = false;
      if (ListHelper.GetMetricDataSources().Any() && ListHelper.GetMetrics().Any(m=>m.DataSourceId.HasValue) )
      {
        ViewBag.ShowImportButton = true;
      }
      var metricList = _metricManager.GetMetricViews(CompanyId, new DateTime(year, month, 1), CurrentUser);
      return View(new ReportMetricInfo { MetricViews = metricList.ToList(), ReportDate = new DateTime(year, month, 1), ListHelper = ListHelper });
    }

    [Route("{year}/{month}/metrics")]
    [HttpPost]
    public ActionResult Metrics(ReportMetricInfo reportMetricInfo)
    {
      if (ModelState.IsValid)
      {
        ListHelper.InitializeMetrics(CurrentUser);
        _metricManager.SaveMetricDataPoints(CompanyId, reportMetricInfo.ToMetricDataPoints().ToList(), ListHelper.MetricIds());

        this.SetNotificationMessage(NotificationType.Success, "Report metrics saved successfully.");

        return RedirectToAction("Edit", new { year = reportMetricInfo.ReportDate.Year, month = reportMetricInfo.ReportDate.Month });
      }

      return View(reportMetricInfo);
    }

    #endregion

    #region Report Goals

    [Route("{year}/{month}/goals")]
    [HttpGet]
    public ActionResult Goals(int year, int month)
    {
      var reportGoals = _reportGoalManager.GetReportGoals(CompanyId, new DateTime(year, month, 1), CurrentUser);

      ListHelper.InitializeGoalStatusList();
      ListHelper.InitializeAreas(CurrentUser);

      return View(new ReportGoalInfo { ReportGoalList = reportGoals.ToList(), ReportDate = new DateTime(year, month, 1), ListHelper = ListHelper });
    }

    [Route("{year}/{month}/goals")]
    [HttpPost]
    public ActionResult Goals(ReportGoalInfo reportGoalInfo)
    {
      if (ModelState.IsValid)
      {
        _reportGoalManager.SaveReportGoals(CompanyId, reportGoalInfo.ReportGoalList.ToList());

        this.SetNotificationMessage(NotificationType.Success, "Report goals saved successfully.");

        return RedirectToAction("Edit", new { year = reportGoalInfo.ReportDate.Year, month = reportGoalInfo.ReportDate.Month });
      }

      return View(reportGoalInfo);
    }

    [HttpPost]
    [Route("{year}/{month}/goals/{goalUniqueId}/summary")]
    public ActionResult UpdateGoalSummary(int month, int year, Guid goalUniqueId, string summary)
    {
      var reportDate = new DateTime(year, month, 1);
      var goal = _reportGoalManager.GetReportGoal(CompanyId, goalUniqueId, reportDate);
      goal.Summary = summary;

      _reportGoalManager.Update(goal.ToReportGoal());

      var dateString = "No Due Date";
      if (goal.DueDate.HasValue)
      {
        var dueDateDayMonth = goal.DueDate.Value.ToString("dd MMM", CultureInfo.InvariantCulture);
        var dueDateYear = goal.DueDate.Value.ToString("yy", CultureInfo.InvariantCulture);
        dateString = string.Format("Due {0} '{1}", dueDateDayMonth, dueDateYear);
      }
      return Json(new { success = true, value = string.Format("<em>{0}</em>{1}", dateString, HttpUtility.HtmlEncode(summary)) });
    }

    #endregion

    #region Report Area

    [Route("{year}/{month}/areas/{areaUniqueId}")]
    [HttpGet]
    public ActionResult ReportAreaInfo(int year, int month, Guid areaUniqueId)
    {
      var area = _areaManager.GetArea(CompanyId, areaUniqueId);
      if (!CurrentUser.AccessibleAreaIds.Contains(area.Id))
      {
        throw new HttpException(403, "Forbidden");
      }
      return View("ReportArea", _reportAreaManager.GetReportArea(CompanyId, area.UniqueId, new DateTime(year, month, 1)));
    }

    [Route("{year}/{month}/areas")]
    [HttpPost]
    public ActionResult ReportArea(ReportArea reportArea)
    {
      if (ModelState.IsValid)
      {
        if (!CurrentUser.AccessibleAreaIds.Contains(reportArea.AreaId))
        {
          throw new HttpException(403, "Forbidden");
        }
        reportArea.Summary = HttpUtility.HtmlDecode(reportArea.Summary);
        var savedReportArea = reportArea.Id == 0 ? _reportAreaManager.Create(reportArea) : _reportAreaManager.Update(reportArea);

        if (savedReportArea != null)
        {
          this.SetNotificationMessage(NotificationType.Success, "Report area saved successfully.");

          // Notify listeners
          var reportEditLink = Url.Action("Edit", "Reports", new { year = reportArea.ReportDate.Year, month = reportArea.ReportDate.Month });
          _webhookManager.ReportingAreaUpdated(CompanyId, CurrentUser, reportArea, reportEditLink);
        }

        return RedirectToAction("Edit", new { year = reportArea.ReportDate.Year, month = reportArea.ReportDate.Month });
      }

      return View(reportArea);
    }

    [HttpPost]
    [Route("{year}/{month}/areas/{areaUniqueId}/summary")]
    public ActionResult UpdateAreaSummary(int month, int year, Guid areaUniqueId, string summary)
    {
      var reportDate = new DateTime(year, month, 1);
      var reportArea = _reportAreaManager.GetReportArea(CompanyId, areaUniqueId, reportDate);
      reportArea.Summary = HttpUtility.HtmlDecode(summary);
      var result = (reportArea.Id > 0) ? _reportAreaManager.Update(reportArea) : _reportAreaManager.Create(reportArea);

      return Json(new { success = true, value = summary });
    }

    #endregion

    #region Attachments
    [Route("{year}/{month}/area/{areaUniqueId}/attachfile")]
    [HttpPost]
    public ActionResult AttachFile(int year, int month, Guid areaUniqueId, string fileDescription, HttpPostedFileBase newAttachment)
    {
      if (ModelState.IsValid)
      {
        if (newAttachment != null && newAttachment.ContentLength > 0)
        {
          ListHelper.InitializeAreas(CurrentUser);
          int? areaId = null;
          if (areaUniqueId != Guid.Empty) { areaId = ListHelper.GetAreas().SingleOrDefault(a => a.UniqueId == areaUniqueId).Id; }
          if (!CurrentUser.IsCompanyAdmin && !CurrentUser.AccessibleAreaIds.Contains(areaId.Value))
          {
            throw new HttpException(400, "Bad Request");
          }
          var reportDate = new DateTime(year, month, 1);
          var reportId = _reportManager.GetReportId(CompanyId, reportDate);

          if (!_reportAttachmentManager.IsFilenameUnique(new ReportAttachment
                           {
                             CompanyId = CompanyId,
                             ReportId = reportId,
                             FileName = newAttachment.FileName,
                             AreaId = areaId
                           }))
          {
            return Json(new { success = false, error = "A file with this name already exists. Delete it before trying to replace it." });
          }

          var attachment = new ReportAttachment
                           {
                             CompanyId = CompanyId,
                             ReportId = reportId,
                             FileName = newAttachment.FileName,
                             Description = fileDescription,
                             MimeType = newAttachment.ContentType,
                             AreaId = areaId
                           };
          var model = _reportAttachmentManager.Create(attachment, newAttachment.InputStream);
          var html = ViewToString("_Attachment_Row", model);
          return Json(new { success = true, html = html });
        }
      }
      else
      {
        return Json(new { success = false, error = "File attachment failed, try again." });
      }
      return Json(new { success = false, error = "File attachment failed, try again." });
    }

    public ActionResult DeleteAttachment(Guid attachmentId)
    {
      var model = _reportAttachmentManager.GetByUniqueId(CompanyId, attachmentId);
      var isDeleted = _reportAttachmentManager.Delete(model);
      return Json(isDeleted ? new { success = true } : new { success = false });
    }

    #endregion

    #region Publish Report

    [HttpPost]
    [ValidateInput(false)]
    [Route("{year}/{month}/email-body-preview")]
    public ActionResult ReportEmailBodyPreview(int year, int month, string userBody, string firstName, string lastName)
    {
      if (!CurrentUser.IsCompanyAdmin)
      {
        throw new HttpException(403, "Forbidden");
      }
      var reportDate = new DateTime(year, month, 1);
      var report = _reportManager.GetReport(CompanyId, reportDate);
      var company = _companyManager.Get(CompanyId);
      var recipient = new Recipient { FirstName = firstName, LastName = lastName };
      var reportEmailBuilder = new ReportEmailBuilder { ReportEmailBody = userBody.Replace(Environment.NewLine, "<br>"), IsSubscriptionActive = IsSubscriptionActive, EnableCommenting = company.EnableCommenting };

      return Content(_reportEmailManager.ParseReportEmailBody(CurrentUser, company, report, recipient, reportEmailBuilder));
    }

    [HttpGet]
    [Route("{year}/{month}/send")]
    public ActionResult Send(int year, int month)
    {
      if (!CurrentUser.IsCompanyAdmin)
      {
        throw new HttpException(403, "Forbidden");
      }
      var reportDate = new DateTime(year, month, 1);
      var recipients = _recipientManager.GetReportRecipients(CompanyId, reportDate);
      var report = _reportManager.GetReport(CompanyId, reportDate);
      var company = _companyManager.Get(CompanyId);
      var owner = _companyManager.GetOwner(CompanyId);

      InitializeMasterLists(CurrentUser);

      var reportStatus = report.Status;
      var reportId = report.Id;

      if (string.IsNullOrEmpty(company.ReportEmailBody))
      {
        company.ReportEmailBody = ConfigUtil.DefaultReportEmailBody;
      }

      if (string.IsNullOrEmpty(company.ReportEmailSubject))
      {
        company.ReportEmailSubject = ConfigUtil.DefaultReportEmailSubject;
      }

      // Preview string for sender details
      var defaultSender = string.Format("{0} <{1}>", ConfigUtil.NoreplyDisplayName, ConfigUtil.NoreplyEmailAddress);
      var senderPreview = (company.UseCustomSender)
        ? string.Format("{0} <{1}>", company.CustomSenderName, company.CustomSenderEmail)
        : defaultSender;

      // Default values for custom address
      if (string.IsNullOrWhiteSpace(company.CustomSenderEmail)) company.CustomSenderEmail = owner.Email;
      if (string.IsNullOrWhiteSpace(company.CustomSenderName)) company.CustomSenderName = company.Name;

      string[] ownerEmailParts = owner.Email.Split('@');

      return View(new ReportEmailBuilder
                  {
                    ReportId = reportId,
                    ReportDate = reportDate,
                    ReportStatus = reportStatus,
                    ReportTitle = company.ReportTitle,
                    ReportDisplayMonth = reportDate.ToString("MMMM", CultureInfo.InvariantCulture),
                    ReportDisplayYear = reportDate.Year.ToString(CultureInfo.InvariantCulture),
                    ReportEmailSubject = company.ReportEmailSubject,
                    ReportEmailBody = company.ReportEmailBody,
                    UseCustomSender = company.UseCustomSender,
                    CustomSenderEmail = company.CustomSenderEmail,
                    CustomSenderName = company.CustomSenderName,
                    DefaultReportEmailBody = ConfigUtil.DefaultReportEmailBody,
                    DefaultReportEmailSubject = ConfigUtil.DefaultReportEmailSubject,
                    DefaultSender = defaultSender,
                    SenderPreview = senderPreview,
                    OwnerEmail = owner.Email,
                    PreviewFirstName = ownerEmailParts[0],
                    EnableCommenting = company.EnableCommenting,
                    RecipientList = recipients.ToList(),
                    SendEmail = false
                  });
    }

    [HttpPost]
    public JsonResult SendEmail(ReportEmailBuilder reportEmailBuilder)
    {
      if (!CurrentUser.IsCompanyAdmin)
      {
        throw new HttpException(403, "Forbidden");
      }
      var company = _companyManager.Get(CompanyId);
      var report = _reportManager.GetReport(CompanyId, reportEmailBuilder.ReportDate);

      reportEmailBuilder.IsSubscriptionActive = IsSubscriptionActive;
      reportEmailBuilder.EnableCommenting = company.EnableCommenting;

      if (!reportEmailBuilder.SendPreview)
      {
        SaveReportEmailTemplateChanges(company, reportEmailBuilder);
      }

      if (reportEmailBuilder.SendEmail || reportEmailBuilder.SendPreview)
      {
        _reportEmailManager.SendReports(CurrentUser, company, report, reportEmailBuilder);

        if (reportEmailBuilder.SendEmail)
        {
          this.SetNotificationMessage(NotificationType.Success, "Report sent successfully.");
        }
      }

      return Json(new { success = true });
    }

    [HttpGet]
    [Route("sendThanks")]
    public ActionResult SendThanks()
    {
      // Don't ask paying subscribers to tweet thanks
      if (CurrentUser.HasValidSubscription)
      {
        return Redirect(Url.Action("Index"));
      }

      return View();
    }

    private void SaveReportEmailTemplateChanges(Company company, ReportEmailBuilder reportEmailBuilder)
    {
      if (!CurrentUser.IsCompanyAdmin)
      {
        throw new HttpException(403, "Forbidden");
      }
      // ### SUBJECT ###
      // update subject if it's changed from the default
      if (reportEmailBuilder.ReportEmailSubject != reportEmailBuilder.DefaultReportEmailSubject)
      {
        company.ReportEmailSubject = reportEmailBuilder.ReportEmailSubject;
      }
      else
      {
        //email subject is default, so set to null (overwrite any previous customisation)
        company.ReportEmailSubject = null;
      }

      // ### BODY ###
      // update body if it's changed from the default
      if (reportEmailBuilder.ReportEmailBody != reportEmailBuilder.DefaultReportEmailBody)
      {
        company.ReportEmailBody = reportEmailBuilder.ReportEmailBody;
      }
      else
      {
        //email body is default, so set to null (overwrite any previous customisation)
        company.ReportEmailBody = null;
      }

      // ### SENDER ###
      company.UseCustomSender = reportEmailBuilder.UseCustomSender;
      company.CustomSenderEmail = reportEmailBuilder.CustomSenderEmail;
      company.CustomSenderName = reportEmailBuilder.CustomSenderName;

      _companyManager.Update(company);
    }

    #endregion

    private string ViewToString(string viewName, dynamic model)
    {
      ViewData.Model = model;
      using (var sw = new StringWriter())
      {
        var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
        var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
        viewResult.View.Render(viewContext, sw);

        return sw.GetStringBuilder().ToString();
      }
    }
  }
}
