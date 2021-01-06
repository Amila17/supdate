using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supdate.Data;
using Supdate.Model;
using Supdate.Util;

namespace Supdate.Business
{
  public class GenericEmailManager : IGenericEmailManager
  {
    private readonly ITemplateManager _templateManager;
    private readonly ICloudStorage _cloudStorage;

    public GenericEmailManager(ITemplateManager templateManager, ICloudStorage cloudStorage)
    {
      _templateManager = templateManager;
      _cloudStorage = cloudStorage;
    }

    public void SendFromTemplate(string toEmail, string subject, TextTemplate template, TextReplacements replacements)
    {
      // Setup replacements.
      if (string.IsNullOrEmpty(replacements.RecipientEmail))
      {
        replacements.RecipientEmail = toEmail;
      }
      replacements.BaseUrl = ConfigUtil.BaseAppUrl;

      // Compile email body and subject.
      var emailBody = _templateManager.Compile(template, replacements);
      var emailSubject = _templateManager.Compile(subject, replacements);

      var categories = GetEmailCategories(template);

      // Prepare the email message and queue it.
      var mailMessage = CreateEmailMessage(toEmail, toEmail, emailSubject, emailBody, categories);

      Task.Run(() => QueueEmail(mailMessage));
    }

    public void SendBasic(string toName, string toAddress, string subject, string body)
    {
      var mailMessage = CreateEmailMessage(toName, toAddress, subject, body);

      Task.Run(() => QueueEmail(mailMessage));
    }

    public GenericEmail CreateEmailMessage(string toName, string toAddress, string subject, string body, params string[] categories)
    {
      var mailMessage = new GenericEmail();

      mailMessage.Sender = new GenericEmailAddress(ConfigUtil.NoreplyEmailAddress, ConfigUtil.NoreplyDisplayName);
      mailMessage.ToAddresses.Add(new GenericEmailAddress(toAddress, toName));

      // Body and subject
      mailMessage.Subject = subject;
      mailMessage.HtmlBody = body;

      // Categories.
      mailMessage.Categories = categories;

      return mailMessage;
    }

    public void QueueEmail(GenericEmail mailMessage)
    {
      var message = JsonConvert.SerializeObject(mailMessage);

      _cloudStorage.Enqueue(ConfigUtil.GenericEmailQueueName, message);
    }

    private string[] GetEmailCategories(TextTemplate textTemplate)
    {
      var categories = new List<string>();

      switch (textTemplate)
      {
        case TextTemplate.WelcomeEmail:
          categories.Add("welcome-email");
          break;

        case TextTemplate.TeamInvitationEmail:
          categories.Add("team-invitation");
          break;

        case TextTemplate.TeamGrantAccessEmail:
          categories.Add("team-grant-access");
          break;

        case TextTemplate.ForgotPasswordEmail:
          categories.Add("forgot-password");
          break;

        case TextTemplate.ReportEmail:
          categories.Add("report-delivery");
          break;

        case TextTemplate.ReportViewedNotificationEmail:
          categories.Add("report-view-notification");
          break;

        case TextTemplate.ReportCommentNotificationEmail:
          categories.Add("report-comment-notification");
          break;
      }

      return categories.ToArray();
    }
  }
}
