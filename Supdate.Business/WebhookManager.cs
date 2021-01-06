using System;
using System.Collections.Generic;
using Supdate.Model;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Supdate.Data.Base;
using Supdate.Util;

namespace Supdate.Business
{
  public class WebhookManager : Manager<Webhook>, IWebhookManager
  {
    private readonly ICrudRepository<Webhook> _webhookRepository;
    private readonly Encoding _encoding = new UTF8Encoding();

    public WebhookManager(ICrudRepository<Webhook> webhookRepository)
      : base(webhookRepository)
    {
      _webhookRepository = webhookRepository;
    }

    #region Events

    public void ReportingAreaUpdated(int companyId, LiteUser currentUser, ReportArea reportArea, string reportUrl)
    {
      var webhooks = _webhookRepository.GetList(new { CompanyId = companyId, EventReportingAreaUpdated = true });

      Task.Run(() => NotifyReportingAreaUpdated(currentUser, reportArea, webhooks, reportUrl));
    }

    private void NotifyReportingAreaUpdated(LiteUser currentUser, ReportArea reportArea, IEnumerable<Webhook> webhooks, string url)
    {
      var payload = new WebhookPayload { Text = string.Format("{0} updated a Reporting Area", currentUser.DisplayName) };
      payload.Attachments.Add(new WebhookPayloadAttachment
                              {
                                Title = string.Format("{0}, {1}", reportArea.AreaName, reportArea.ReportDate.ToString("MMMM \\'yy")),
                                Text = string.Format("{0}\n<{1}{2}|Go to Report>", reportArea.Summary, ConfigUtil.BaseAppUrl, url)
                              });

      PostMessages(webhooks, payload);
    }

    public bool ReportViewed(int companyId, ReportEmail reportEmail, Report report)
    {
      var webhooks = _webhookRepository.GetList(new { CompanyId = companyId, EventReportViewed = true });

      Task.Run(() => NotifyReportViewed(reportEmail, report, webhooks));

      return webhooks.Any();
    }

    private void NotifyReportViewed(ReportEmail reportEmail, Report report, IEnumerable<Webhook> webhooks)
    {
      var timesViewed = (reportEmail.Views > 1) ? StringUtil.Ordinal(reportEmail.Views) : "first";
      var recipient = reportEmail.Recipient;
      var url = string.Format("{0}reports/{1}", ConfigUtil.BaseAppUrl, report.UniqueId);
      var payload = new WebhookPayload
                    {
                      Text =
                        string.Format("{0} viewed the {1} report for the {2} time.\n<{3}|Go to Report> or <mailto:{4}|Email {5}>", recipient.DisplayName, report.Date.ToString("MMMM \\'yy"), timesViewed, url, recipient.Email, recipient.FirstName)
                    };

      PostMessages(webhooks, payload);
    }

    public bool CommentPosted(int companyId, Discussion discussion, Comment comment)
    {
      var webhooks = _webhookRepository.GetList(new { CompanyId = companyId, EventReportComment = true });

      Task.Run(() => NotifyCommentPosted(discussion, comment, webhooks));

      return webhooks.Any();
    }

    private void NotifyCommentPosted(Discussion discussion, Comment comment, IEnumerable<Webhook> webhooks)
    {
      var url = string.Format("{0}reports/{1}/?discuss={2}&requireLogin=true", ConfigUtil.BaseAppUrl, discussion.ReportGuId, discussion.DiscussionName);
      var payload = new WebhookPayload { Text = string.Format("{0} commented on your report", comment.AuthorName) };

      payload.Attachments.Add(new WebhookPayloadAttachment
                              {
                                Title = string.Format("{0}, {1}", discussion.Title, discussion.ReportDate.ToString("MMMM \\'yy")),
                                Text = string.Format("{0}\n<{1}|View Discussion>", comment.Text, url)
                              });

      PostMessages(webhooks, payload);
    }

    #endregion

    private void PostMessages(IEnumerable<Webhook> webhooks, WebhookPayload payload)
    {
      foreach (var webhook in webhooks)
      {
        PostMessage(webhook.WebhookUrl, payload);
      }
    }

    private void PostMessage(string url, WebhookPayload payload)
    {
      try
      {
        var payloadJson = JsonConvert.SerializeObject(payload);

        using (var client = new WebClient())
        {
          var data = new NameValueCollection();
          data["payload"] = payloadJson;

          var response = client.UploadValues(url, "POST", data);

          // The response text is usually "ok"
          var responseText = _encoding.GetString(response);
        }
      }
      catch (Exception)
      {

      }
    }
  }

}
