using System;
using System.IO;
using System.Reflection;
using RazorEngine;
using RazorEngine.Templating;
using SimpleInjector;
using Supdate.Business;
using Supdate.Business.Admin;
using Supdate.DI;
using Supdate.Model;
using Supdate.Util;

namespace Supdate.Web.Job.Feedback
{
  class Program
  {
    private static Container _container;
    private static IAdminManager _adminManager;
    private static IGenericEmailManager _genericEmailManager;

    public static void Main(string[] args)
    {
      // SimpleInjector container.
      _container = new Container();

      // Register dependencies.
      SimpleInjectorInitializer.InitializeContainer(_container, Lifestyle.Singleton);
      _adminManager = _container.GetInstance<IAdminManager>();
      _genericEmailManager = _container.GetInstance<IGenericEmailManager>();

      // Send confirmation email reminders.
      SendFeedbackEmail();
    }

    private static void SendFeedbackEmail()
    {
      // Get list of unconfirmed users
      var newUsers = _adminManager.GetNewUsers(24 * 3);

      // Form subject and email body.
      var subject = ConfigUtil.FeedbackEmailSubject;
      var emailBody = GetEmailBody();

      foreach (var user in newUsers)
      {
        Console.WriteLine("Sending feedback email to {0}", user.Email);

        // Send the email.
        SendEmail(subject, emailBody, user.Email);

        // Increment the count of reminders sent.
        _adminManager.UpdateFeedbackEmailSent(user.Id);
      }
    }

    private static string GetEmailBody()
    {
      var templatePath = Path.Combine(AssemblyDirectory, "Feedback.cshtml");
      var template = File.ReadAllText(templatePath);

      var body = Engine.Razor.RunCompile(template, "confirmationEmailTemplate");

      return body;
    }

    private static void SendEmail(string subject, string body, string recipientEmail)
    {
      var mailMessage = new GenericEmail();

      mailMessage.Sender = new GenericEmailAddress(ConfigUtil.FeedbackEmailFromAddress, ConfigUtil.FeedbackEmailDisplayName);
      mailMessage.ToAddresses.Add(new GenericEmailAddress(recipientEmail, string.Empty));
      mailMessage.HtmlBody = body;
      mailMessage.Subject = subject;
      mailMessage.Categories = new[] { "supdate-feedback" };

      _genericEmailManager.QueueEmail(mailMessage);

      Console.WriteLine("Queued email to {0}", recipientEmail);
    }

    private static string AssemblyDirectory
    {
      get
      {
        var codeBase = Assembly.GetExecutingAssembly().CodeBase;
        var uri = new UriBuilder(codeBase);
        var path = Uri.UnescapeDataString(uri.Path);

        return Path.GetDirectoryName(path);
      }
    }
  }
}
