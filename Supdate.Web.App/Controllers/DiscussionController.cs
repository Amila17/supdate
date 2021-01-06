using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using StackExchange.Exceptional;
using Supdate.Business;
using Supdate.Model;
using Supdate.Util;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Hubs;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("discuss")]
  public class DiscussionController : AuthenticatedControllerBase
  {
    private readonly IDiscussionManager _discussionManager;
    private readonly IReportManager _reportManager;

    public DiscussionController(IDiscussionManager discussionManager, IReportManager reportManager)
    {
      _discussionManager = discussionManager;
      _reportManager = reportManager;
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("details/{reportGuid}/{targetType}/{targetGuid}")]
    public ActionResult Details(Guid reportGuid, DiscussionTargetType targetType, Guid targetGuid)
    {
      var discussion = _discussionManager.Get(reportGuid, targetType, targetGuid);
      if (discussion == null)
      {
        discussion = new Discussion
        {
          Title = "Start a Discussion",
          ReportGuId = reportGuid,
          TargetType = targetType,
          Target = targetGuid
        };
      }

      return View("_details", discussion);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("details/{reportGuid}/{targetType}/{targetGuid}")]
    public ActionResult Details(Guid reportGuId, DiscussionTargetType targetType, Guid targetGuid, Comment comment)
    {
      InitializeContext();

      //ensure the Author Name or Email hasn't been messed with
      if (ConversionUtil.CommentToHashString(comment) != comment.AuthorHash)
      {
        throw new HttpException(400, "Bad Request");
      }

      // Get the report.
      var report = _reportManager.GetList(new { UniqueId = reportGuId }).SingleOrDefault();

      if (report == null)
      {
        throw new HttpException(400, "Bad Request");
      }

      // Get the discussion
      var discussion = _discussionManager.Get(reportGuId, targetType, targetGuid);
      if (discussion == null)
      {
        discussion = new Discussion { ReportGuId = reportGuId, ReportId = report.Id, ReportDate = report.Date, TargetType = targetType, Target = targetGuid, };
      }
      discussion.CompanyId = report.CompanyId;

      var updatedDiscussion =  _discussionManager.AddComment(discussion, comment);
      var commentsHtml = ViewToString("_comments", updatedDiscussion.Comments);

      try
      {
        // update other clients
        GlobalHost.ConnectionManager.GetHubContext<DiscussionHub>()
          .Clients.Group(discussion.DiscussionName)
          .refreshComments(discussion.DiscussionName, commentsHtml, updatedDiscussion.Title, discussion.CommentCount);
      }
      catch (Exception exception)
      {
        ErrorStore.LogException(exception, System.Web.HttpContext.Current);
      }

      return Json(new { success = true, discussionName = updatedDiscussion.DiscussionName, title = updatedDiscussion.Title, commentCount = discussion.CommentCount, commentsHtml });
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("details/{reportGuid}/{targetType}/{targetGuid}/comment/{commentGuid}/delete")]
    public ActionResult DeleteComment(Guid reportGuid, DiscussionTargetType targetType, Guid targetGuid, Guid commentGuid, string authorHash)
    {
      // company admins can delete any comment for their company.
      // non admins can only delete their own comments
      var deletionAuthorised = false;

      var discussion = _discussionManager.Get(reportGuid, targetType, targetGuid);
      var comment = discussion.Comments.SingleOrDefault(c => c.UniqueId == commentGuid);
      if (Request.IsAuthenticated)
      {
        InitializeContext();
        if (discussion.CompanyId == CurrentUser.CompanyId && CurrentUser.IsCompanyAdmin)
        {
          deletionAuthorised = true;
        }
      }

      if (!deletionAuthorised)
      {
        // let's see if the comment is made by the person trying to delete it

        if (authorHash == ConversionUtil.CommentToHashString(comment))
        {
          deletionAuthorised = true;
        }
      }

      if (deletionAuthorised)
      {

        discussion.Comments.Remove(comment);
        _discussionManager.DeleteComment(comment);

        try
        {
          // update others
          GlobalHost.ConnectionManager.GetHubContext<DiscussionHub>()
            .Clients.Group(discussion.DiscussionName)
            .removeComment(discussion.DiscussionName, commentGuid, discussion.CommentCount);
        }
        catch (Exception exception)
        {
          ErrorStore.LogException(exception, System.Web.HttpContext.Current);
        }
      }
      else
      {
        throw new HttpException(401, "Unauthorized");
      }

      return Json(new { success = true, commentGuid, commentCount = discussion.CommentCount });
    }

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
