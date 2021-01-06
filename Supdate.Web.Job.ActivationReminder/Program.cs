using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using RazorEngine;
using RazorEngine.Templating;
using SimpleInjector;
using Supdate.Business;
using Supdate.Business.Admin;
using Supdate.Data.Identity;
using Supdate.DI;
using Supdate.Model.Identity;
using Supdate.Web.App.Models;

namespace Supdate.Web.Job.ActivationReminder
{
  public class Program
  {
    private static Container _container;
    private static IAdminManager _adminManager;
    private static AppUserStore<AppUser> _userStore;
    private static UserManager<AppUser, int> _userManager;

    public static void Main(string[] args)
    {
      // SimpleInjector container.
      _container = new Container();

      // Register dependencies.
      SimpleInjectorInitializer.InitializeContainer(_container, Lifestyle.Singleton);

      _userStore = new AppUserStore<AppUser>(new IdentityDatabaseContext<AppUser, IdentityRole>());
      _userManager = new UserManager<AppUser, int>(_userStore);
      _adminManager = _container.GetInstance<IAdminManager>();
      _userManager.EmailService = new EmailService();

      // Send confirmation email reminders.
      SendConfirmationEmailReminders().Wait();
    }

    private static async Task SendConfirmationEmailReminders()
    {
      // Get list of unconfirmed users
      var unconfirmedUsers = _adminManager.GetUnconfirmedUsers(24);

      foreach (var user in unconfirmedUsers)
      {
        // Form email body.
        var emailBody = GetEmailBody(confirmationUrl: user.Url);

        Console.WriteLine("Sending account confirmation reminder to {0}", user.Email);

        // Send the email.
        await _userManager.SendEmailAsync(user.UserId, "Account confirmation reminder", emailBody);

        // Increment the count of reminders sent.
        _adminManager.IncrementConfirmationReminderCount(user.UserId);

        Console.WriteLine("Sent successfully!");
      }
    }

    private static string GetEmailBody(string confirmationUrl)
    {
      var templatePath = Path.Combine(AssemblyDirectory, "AccountConfirmation.cshtml");
      var template = File.ReadAllText(templatePath);

      var model = new { ConfirmationUrl = confirmationUrl };
      var body = Engine.Razor.RunCompile(template, "confirmationEmailTemplate", null, model);

      return body;
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
