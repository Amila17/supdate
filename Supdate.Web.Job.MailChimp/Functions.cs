using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MailChimp;
using MailChimp.Helper;
using MailChimp.Lists;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Supdate.Model.Admin;

namespace Supdate.Web.Job.MailChimp
{
  public class Functions
  {
    private const string MailChimpApiKey = "MailChimpApiKey";
    private const string MailChimpGroupNamesKey = "MailChimpGroupNames";
    private const string MailChimpListIdKey = "MailChimpListId";
    private const string MailChimpGroupIdKey = "MailChimpGroupId";

    #region MailChimp Queue

    public void ProcessMailChimpQueueMessage([QueueTrigger("%MailChimpQueue%")] string message, TextWriter log)
    {
      log.WriteLine("Message received: {0}", message);
      Console.WriteLine(message);

      // Read the configuration.
      var mailChimpApiKey = CloudConfigurationManager.GetSetting(MailChimpApiKey);
      var mailChimpListId = CloudConfigurationManager.GetSetting(MailChimpListIdKey);
      var mailChimpGroupId = CloudConfigurationManager.GetSetting(MailChimpGroupIdKey);
      var mailChimpGroupNameList = GetGroupNameList();

      // Convert the message to a model.
      var user = JsonConvert.DeserializeObject<MarketingData>(message);
      var mailchimpManager = new MailChimpManager(mailChimpApiKey);

      // Create an instance of the merge variable.
      var mergeVars = new MergeVar { Groupings = new List<Grouping> { new Grouping() } };
      mergeVars.Add("COMPANY", user.CompanyName);
      mergeVars.Add("AREAS", user.Areas.ToString());
      mergeVars.Add("METRICS", user.Metrics.ToString());
      mergeVars.Add("GOALCOUNT", user.Goals.ToString());
      mergeVars.Add("REPS_START", user.ReportsStarted.ToString());
      mergeVars.Add("REPS_COMP", user.ReportsCompleted.ToString());
      mergeVars.Add("REPS_SENT", user.ReportsSent.ToString());
      mergeVars.Add("REPSCOUNT", user.ReportsTotal.ToString());
      mergeVars.Add("LOGINS", user.LoginCount.ToString());
      mergeVars.Add("LASTLOGIN", user.LastLogin.ToString(@"MM\/dd\/yyyy HH:mm"));
      mergeVars.Add("SUB_STATUS", user.SubscriptionStatus.ToString());
      mergeVars.Add("SUB_EXPIRY", user.SubscriptionExpiryDate.ToString(@"MM\/dd\/yyyy HH:mm"));

      // New user
      if (string.IsNullOrEmpty(user.OldEmail))
      {
        // Create an instance of email parameter.
        var emailParameter = new EmailParameter { Email = user.Email };

        // Set the default groups.
        mergeVars.Groupings[0].Id = int.Parse(mailChimpGroupId, NumberStyles.Integer);
        mergeVars.Groupings[0].GroupNames = mailChimpGroupNameList;

        // Subscribe to the configured MailChimp list.
        mailchimpManager.Subscribe(mailChimpListId, emailParameter, mergeVars, updateExisting: true, doubleOptIn: false, sendWelcome: false);
      }
      else
      {
        // Case of existing user changed an email address.

        // Create an instance of email parameter.
        var emailParameter = new EmailParameter { Email = user.OldEmail };
        mergeVars.Add("NEW-EMAIL", user.Email);

        // Update the email address.
        mailchimpManager.UpdateMember(mailChimpListId, emailParameter, mergeVars, replaceInterests: false);
      }

      Console.WriteLine("Email {0} successfully pushed to MailChimp", user.Email);
    }

    private List<string> GetGroupNameList()
    {
      var mailChimpGroupNames = CloudConfigurationManager.GetSetting(MailChimpGroupNamesKey);

      return mailChimpGroupNames.Split('|').ToList();
    }

    #endregion
  }
}
