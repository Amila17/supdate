using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Supdate.Data;
using Supdate.Model;
using Supdate.Util;

namespace Supdate.Business
{
  public class ReportEmailManager : Manager<ReportEmail>, IReportEmailManager
  {
    private readonly IGenericEmailManager _genericEmailManager;
    private readonly ITemplateManager _templateManager;
    private readonly ICompanyManager _companyManager;
    private readonly IReportManager _reportManager;
    private readonly IRecipientManager _recipientManager;
    private readonly IReportEmailRepository _reportEmailRepository;
    private readonly IWebhookManager _webhookManager;

    public ReportEmailManager(IReportEmailRepository reportEmailRepository, ICompanyManager companyManager,
      IGenericEmailManager genericEmailManager, ITemplateManager templateManager, IRecipientManager recipientManager,
      IReportManager reportManager, IWebhookManager webhookManager)
      : base(reportEmailRepository)
    {
      _genericEmailManager = genericEmailManager;
      _templateManager = templateManager;
      _companyManager = companyManager;
      _reportEmailRepository = reportEmailRepository;
      _reportManager = reportManager;
      _recipientManager = recipientManager;
      _webhookManager = webhookManager;
    }

    private string GetReportEmailPart(TextTemplate template, string placeholderValue)
    {
      return string.Format(_templateManager.GetTemplateText(template), placeholderValue);
    }

    public string ParseReportEmailBody(LiteUser currentUser, Company company, Report report, Recipient recipient,
      ReportEmailBuilder reportEmailBuilder)
    {
      // Get the preview banner if required
      var previewBanner = reportEmailBuilder.SendPreview ? _templateManager.GetTemplateText(TextTemplate.ReportEmailPreviewBanner) : String.Empty;

      // Parse the email body defined by the user.
      // Trim line breaks after variables (since we use the <pre> tag.
      var userBody = new StringBuilder(string.Format("{0}{1}", previewBanner, reportEmailBuilder.ReportEmailBody));
      userBody.Replace("[COMPANY_NAME]" + Environment.NewLine, "[COMPANY_NAME]");
      userBody.Replace("[REPORT_TITLE]" + Environment.NewLine, "[REPORT_TITLE]");
      userBody.Replace("[SUMMARY]" + Environment.NewLine, "[SUMMARY]");
      userBody.Replace("[REPORT_BUTTON]" + Environment.NewLine, "[REPORT_BUTTON]");
      userBody.Replace("[REPORT_DISCUSSION]" + Environment.NewLine, "[REPORT_DISCUSSION]");

      // Generate the report link.
      var reportLink = string.Format("{0}reports/{1}", ConfigUtil.BaseAppUrl, report.UniqueId);
      if (reportEmailBuilder.ReportEmailGuid != null)
      {
        reportLink = string.Format("{0}reports/email/{1}/{2}", ConfigUtil.BaseAppUrl, reportEmailBuilder.ReportEmailGuid, reportEmailBuilder.ReportEmailViewKey);
      }

      // Replace the placeholders with the values.
      userBody.Replace("[COMPANY_NAME]", GetReportEmailPart(TextTemplate.ReportEmailCompanyNameSnippet, company.Name));
      userBody.Replace("[REPORT_TITLE]", GetReportEmailPart(TextTemplate.ReportEmailTitleSnippet, company.ReportTitle));
      userBody.Replace("[SUMMARY]", GetReportEmailPart(TextTemplate.ReportEmailSummarySnippet, report.Summary));
      userBody.Replace("[REPORT_BUTTON]", GetReportEmailPart(TextTemplate.ReportEmailButtonSnippet, reportLink));
      userBody.Replace("[MONTH]", report.Date.ToString("MMMM", CultureInfo.InvariantCulture));
      userBody.Replace("[YEAR]", report.Date.Year.ToString(CultureInfo.InvariantCulture));
      userBody.Replace("[FIRSTNAME]", recipient.FirstName);
      userBody.Replace("[LASTNAME]", recipient.LastName);
      userBody.Replace("[SENDER_FIRSTNAME]", currentUser.FirstName);
      userBody.Replace("[SENDER_LASTNAME]", currentUser.LastName);
      string discussionText = string.Empty;
      if (reportEmailBuilder.EnableCommenting && reportEmailBuilder.IsSubscriptionActive)
      {
        discussionText = GetReportEmailPart(TextTemplate.ReportEmailDiscussion, ConfigUtil.BaseAppUrl);
      }
      userBody.Replace("[REPORT_DISCUSSION]", discussionText);

      // Replace the final
      var template = _templateManager.GetTemplateText(TextTemplate.ReportEmail);
      template = template.Replace("[BODY]", userBody.ToString());

      return template;
    }

    public string ParseReportEmailSubject(Company company, Report report, string userSubject)
    {
      var subject = new StringBuilder(userSubject);
      subject.Replace("[COMPANY_NAME]", company.Name);
      subject.Replace("[TITLE]", company.ReportTitle);
      subject.Replace("[MONTH]", report.Date.ToString("MMMM", CultureInfo.InvariantCulture));
      subject.Replace("[YEAR]", report.Date.Year.ToString(CultureInfo.InvariantCulture));

      return subject.ToString();
    }

    public void SendReports(LiteUser currentUser, Company company, Report report, ReportEmailBuilder reportEmailBuilder)
    {
      var subject = ParseReportEmailSubject(company, report, reportEmailBuilder.ReportEmailSubject);
      reportEmailBuilder.ReportEmailBody = reportEmailBuilder.ReportEmailBody.Replace(Environment.NewLine, "<br>");

      if (reportEmailBuilder.SendPreview)
      {
        var previewRecipient = new Recipient
                               {
                                 FirstName = reportEmailBuilder.PreviewFirstName,
                                 LastName = reportEmailBuilder.PreviewLastName,
                                 Email = reportEmailBuilder.PreviewAddress
                               };
        var body = ParseReportEmailBody(currentUser, company, report, previewRecipient, reportEmailBuilder);

        QueueEmail(company, previewRecipient, subject, body);
      }
      else
      {
        var recipients = reportEmailBuilder.RecipientList.Where(s => s.IsSelected).ToList();
        foreach (var recipient in recipients)
        {
          // log the email
          var reportEmail = Create(new ReportEmail
                                   {
                                     RecipientId = recipient.Id,
                                     ReportId = report.Id,
                                     CompanyId = company.Id,
                                     Status = EmailStatus.Sent
                                   });
          reportEmailBuilder.ReportEmailGuid = reportEmail.UniqueId;
          reportEmailBuilder.ReportEmailViewKey = reportEmail.ViewKey;

          var body = ParseReportEmailBody(currentUser, company, report, recipient, reportEmailBuilder);

          QueueEmail(company, recipient, subject, body);
        }
      }
    }

    private void QueueEmail(Company company, Recipient recipient, string subject, string body)
    {
      var mailMessage = _genericEmailManager.CreateEmailMessage(recipient.Name, recipient.Email, subject, body, "report-delivery");

      if (company.UseCustomSender)
      {
        // update sender fields
        mailMessage.Sender = new GenericEmailAddress(company.CustomSenderEmail, company.CustomSenderName);
      }
      else
      {
        // add ReplyTo fields
        var owner = _companyManager.GetOwner(company.Id);
        mailMessage.ReplyToList.Add(new GenericEmailAddress(owner.Email, company.Name));
      }

      // queue the email
      _genericEmailManager.QueueEmail(mailMessage);
    }

    public ReportEmail LogReportView(Guid reportEmailGuid)
    {
      var reportEmail = _reportEmailRepository.GetList(new { UniqueId = reportEmailGuid }).FirstOrDefault();

      if (reportEmail != null)
      {
        // Increment the view count and last viewed date.
        reportEmail.Views = reportEmail.Views + 1;
        reportEmail.LastViewedDate = DateTime.UtcNow;
        Update(reportEmail);

        // Notify listeners
        NotifyUserofReportView(reportEmail);

        // Get recipient information.
        var recipient = _recipientManager.Get(reportEmail.RecipientId);
        if (recipient.CompanyId == reportEmail.CompanyId)
        {
          reportEmail.Recipient = recipient;
        }
      }
      return reportEmail;
    }

    private void NotifyUserofReportView(ReportEmail reportEmail)
    {
      var report = _reportManager.Get(reportEmail.ReportId);
      var recipient = _recipientManager.Get(reportEmail.RecipientId);
      reportEmail.Recipient = recipient;

      // Notify listeners
      var webHooksSent = _webhookManager.ReportViewed(reportEmail.CompanyId, reportEmail, report);

      // Send a notification email if this is the first time the report is being viewed.
      if (reportEmail.Views == 1)
      {
        var company = _companyManager.Get(report.CompanyId);
        var owner = _companyManager.GetOwner(reportEmail.CompanyId);
        var subject = string.Format("{0} viewed your report", recipient.DisplayName);

        var textReplacements = new TextReplacements
                               {
                                 Subject = subject,
                                 CompanyName = company.Name,
                                 FullName = recipient.DisplayName,
                                 FirstName = string.IsNullOrWhiteSpace(recipient.FirstName) ? "this person" : recipient.FirstName,
                                 GravatarUrl = GravatarHelper.GravatarHelper.CreateGravatarUrl(recipient.Email, 96, ConfigUtil.DefaultGravatarImage, null, null, null),
                                 ReportLink = string.Format("{0}reports/{1}", ConfigUtil.BaseAppUrl, report.UniqueId),
                                 ReportPeriodName = report.Date.ToString("MMMM \\'yy", CultureInfo.InvariantCulture),
                                 PromoteSlack = !webHooksSent
                               };

        _genericEmailManager.SendFromTemplate(owner.Email, subject, TextTemplate.ReportViewedNotificationEmail, textReplacements);
      }
    }

    public ReportEmail GetByEmailAddress(int companyId, int reportId, string emailAddress)
    {
      return _reportEmailRepository.GetByEmailAddress(companyId, reportId, emailAddress);
    }

    public ReportEmail GetByGuid(Guid reportEmailGuid)
    {
      return _reportEmailRepository.GetByGuid(reportEmailGuid);
    }

  }
}
