using System;
using System.Globalization;
using System.Linq;
using Supdate.Data;
using Supdate.Model;
using Supdate.Util;

namespace Supdate.Business
{
  public class DiscussionManager : Manager<Discussion>, IDiscussionManager
  {
    private readonly IDiscussionRepository _dicussionRepository;
    private readonly ICompanyManager _companyManager;
    private readonly IGenericEmailManager _genericEmailManager;
    private readonly IReportEmailManager _reportEmailManager;
    private readonly IWebhookManager _webhookManager;

    public DiscussionManager(IDiscussionRepository discussionRepository, ICompanyManager companyManager,
      IGenericEmailManager genericEmailManager, IReportEmailManager reportEmailManager, IWebhookManager webhookManager)
      : base(discussionRepository)
    {
      _dicussionRepository = discussionRepository;
      _companyManager = companyManager;
      _genericEmailManager = genericEmailManager;
      _reportEmailManager = reportEmailManager;
      _webhookManager = webhookManager;
    }

    public Discussion Get(Guid reportGuid, DiscussionTargetType targetType, Guid targetGuid)
    {
      return _dicussionRepository.Get(reportGuid, targetType, targetGuid);
    }

    private void NotifyOwnerOfNewComment(Discussion discussion, Comment newComment, LiteUser owner, bool hasWebhooks)
    {
      var subject = string.Format("{0} commented on your report", newComment.AuthorName);

      var textReplacements = new TextReplacements
                             {
                               Subject = subject,
                               FullName = newComment.AuthorName,
                               Text = subject,
                               DiscussionTitle = discussion.Title,
                               ReportPeriodName = discussion.ReportDate.ToString("MMMM \\'yy", CultureInfo.InvariantCulture),
                               Comment = newComment.Text,
                               GravatarUrl = GravatarHelper.GravatarHelper.CreateGravatarUrl(newComment.AuthorEmail, 96, ConfigUtil.DefaultGravatarImage, null, null, null),
                               ReportLink = string.Format("{0}reports/{1}?discuss={2}&requireLogin=true", ConfigUtil.BaseAppUrl, discussion.ReportGuId, discussion.DiscussionName),
                               PromoteSlack = !hasWebhooks
                             };

      _genericEmailManager.SendFromTemplate(owner.Email, subject, TextTemplate.ReportCommentNotificationEmail, textReplacements);
    }

    private void NotifyParticipantsOfNewComment(Discussion discussion, Comment newComment, LiteUser owner)
    {
      // Check if there are authors in this discussion part from the owner and the author of this new comment.
      if (discussion.Comments.Count(c => c.AuthorEmail != owner.Email && c.AuthorEmail != newComment.AuthorEmail) == 0)
      {
        //there's no one to notify
        return;
      }

      var subject = string.Format("{0} replied to your comment", newComment.AuthorName);

      var textReplacements = new TextReplacements
                             {
                               Subject = subject,
                               FullName = newComment.AuthorName,
                               Text = subject,
                               DiscussionTitle = discussion.Title,
                               ReportPeriodName = discussion.ReportDate.ToString("MMMM \\'yy", CultureInfo.InvariantCulture),
                               Comment = newComment.Text,
                               GravatarUrl = GravatarHelper.GravatarHelper.CreateGravatarUrl(newComment.AuthorEmail, 96, ConfigUtil.DefaultGravatarImage, null, null, null),
                               ExcludePasswordResetLink = true,
                               ReportLink = string.Format("{0}reports/{1}?discuss={2}&requireLogin=true", ConfigUtil.BaseAppUrl, discussion.ReportGuId, discussion.DiscussionName),
                             };

      var participants = discussion.Comments.Where(c => c.AuthorEmail != owner.Email && c.AuthorEmail != newComment.AuthorEmail).Select(c => c.AuthorEmail).Distinct();
      foreach (var email in participants)
      {
        var reportEmail = _reportEmailManager.GetByEmailAddress(discussion.CompanyId, discussion.ReportId, email);
        if (reportEmail != null)
        {
          textReplacements.ReportLink = string.Format("{0}reports/email/{1}/{2}?discuss={3}", ConfigUtil.BaseAppUrl, reportEmail.UniqueId, reportEmail.ViewKey, discussion.DiscussionName);
        }

        _genericEmailManager.SendFromTemplate(email, subject, TextTemplate.ReportCommentNotificationEmail, textReplacements);
      }
    }

    public void DeleteComment(Comment comment)
    {
      _dicussionRepository.DeleteComment(comment);
    }

    public Discussion AddComment(Discussion discussion, Comment comment)
    {
      discussion.Comments.Add(comment);
      Update(discussion);

      var owner = _companyManager.GetOwner(discussion.CompanyId);
      var hasWebhooks = _webhookManager.CommentPosted(discussion.CompanyId, discussion, comment);

      if (comment.AuthorEmail != owner.Email)
      {
        NotifyOwnerOfNewComment(discussion, comment, owner, hasWebhooks);
      }

      NotifyParticipantsOfNewComment(discussion, comment, owner);

      return Get(discussion.ReportGuId, discussion.TargetType, discussion.Target);
    }
  }
}
