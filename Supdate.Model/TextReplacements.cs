using System;

namespace Supdate.Model
{
  public class TextReplacements
  {
    public string CompanyName { get; set; }
    public string WelcomeMessage { get; set; }
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string OwnerEmail { get; set; }
    public string RecipientEmail { get; set; }

    public string RecipientEmailEncoded
    {
      get { return Uri.EscapeDataString(RecipientEmail); }
    }

    public string BaseUrl { get; set; }
    public string RegisterLink { get; set; }
    public string ResetPasswordLink { get; set; }

    public string FullName { get; set; }
    public string FirstName { get; set; }
    public string GravatarUrl { get; set; }
    public string ReportLink { get; set; }
    public string ReportPeriodName { get; set; }
    public string DiscussionTitle { get; set; }
    public string Comment { get; set; }
    public string Text { get; set; }
    public bool ExcludePasswordResetLink { get; set; }
    public bool PromoteSlack { get; set; }
  }
}
