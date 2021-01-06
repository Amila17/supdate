using System;
using System.Configuration;

namespace Supdate.Util
{
  public class ConfigUtil
  {
    /// <summary>
    /// The interval (in minutes) until which the authentication cookie is stored - this is a sliding expiration.
    /// </summary>
    public static double AuthCookieTimeout
    {
      get { return GetAppSetting("AuthCookieTimeout", 30.0); }
    }

    /// <summary>
    /// Returns whether premium features are available for free to all user.
    /// </summary>
    public static bool FreeAccessToPremiumFeatures
    {
      get { return GetAppSetting("FreeAccessToPremiumFeatures", true); }
    }

    public static string SendGridApiKey
    {
      get { return GetAppSetting("SendGridApiKey", "####################"); }
    }

    public static string NoreplyEmailAddress
    {
      get { return GetAppSetting("NoreplyEmailAddress", "no-reply@supdate.com"); }
    }

    public static string NoreplyDisplayName
    {
      get { return GetAppSetting("NoreplyDisplayName", "Supdate"); }
    }

    public static string ConnectionStringName
    {
      get { return GetAppSetting("ConnectionStringName", "Supdate.Database"); }
    }

    public static string StorageConnectionStringName
    {
      get { return GetAppSetting("StorageConnectionStringName", "Supdate.Storage"); }
    }

    public static string CacheConnectionStringName
    {
      get { return GetAppSetting("CacheConnectionStringName", "Supdate.RedisCache"); }
    }

    public static string LogoStorageContainerName
    {
      get { return GetAppSetting("LogoStorageContainer", "dev-company-logos"); }
    }

    public static string ReportAttachmentStorageContainerName
    {
      get { return GetAppSetting("ReportAttachmentStorageContainer", "dev-report-attachments"); }
    }

    public static string DefaultReportTitle
    {
      get { return GetAppSetting("DefaultReportTitle", "Shareholder update"); }
    }

    public static string DefaultReportEmailSubject
    {
      get { return GetAppSetting("DefaultReportEmailSubject", "[TITLE] - [MONTH] [YEAR]"); }
    }

    public static string DefaultReportEmailBody
    {
      get
      {
        return GetAppSetting("DefaultReportEmailBody", string.Format("Hi [FIRSTNAME],{0}{0}Please find below a summary of progress made in [MONTH].{0}Click the button for the full report.{0}{0}[COMPANY_NAME][REPORT_TITLE][SUMMARY]{0}[REPORT_BUTTON]{0}[REPORT_DISCUSSION]{0}Regards{0}{0}[SENDER_FIRSTNAME] [SENDER_LASTNAME]", Environment.NewLine));
      }
    }

    public static string DefaultInviteMessage
    {
      get { return GetAppSetting("DefaultInviteMessage", "I've given you access to our reporting system so that you can provide updates for your department."); }
    }

    public static string MailChimpQueueName
    {
      get { return GetAppSetting("MailChimpQueue", "####################"); }
    }

    public static string GenericEmailQueueName
    {
      get { return GetAppSetting("GenericEmailQueue", "####################"); }
    }

    public static string BaseAppUrl
    {
      get { return GetAppSetting("BaseAppUrl", "https://app.supdate.com/"); }
    }

    public static string DefaultLogoUrl
    {
      get { return GetAppSetting("DefaultLogoUrl", "/Assets/company-icon-default.png"); }
    }

    public static string DataCentreLocation
    {
      get { return GetAppSetting("DataCentreLocation", "not configured"); }
    }

    public static string FeedbackEmailSubject
    {
      get { return GetAppSetting("FeedbackEmailSubject", "supdate thoughts?"); }
    }

    public static string FeedbackEmailFromAddress
    {
      get { return GetAppSetting("FeedbackEmailFromAddress", "####################"); }
    }

    public static string FeedbackEmailDisplayName
    {
      get { return GetAppSetting("FeedbackEmailDisplayName", "####################"); }
    }

    public static string DefaultGravatarImage
    {
      get { return GetAppSetting("DefaultGravatarImage", "####################"); }
    }

    public static string CommentsHashKey
    {
      get { return GetAppSetting("CommentsHashKey", "####################"); }
    }

    public static int AreaListMaxCharsForTableDisplay
    {
      get { return GetAppSetting("AreaListMaxCharsForTableDisplay", 25); }
    }

    public static string SlackClientId
    {
      get { return GetAppSetting("SlackClientId", "####################"); }
    }

    public static string SlackClientSecret
    {
      get { return GetAppSetting("SlackClientSecret", "####################"); }
    }

    public static  int DefaultTrialDuration
    {
      get { return GetAppSetting("DefaultTrialDuration", 30); }
    }

    public static double DefaultMonthlyCost
    {
      get { return GetAppSetting("DefaultMonthlyCost", 29); }
    }

    public static string StripePlanId
    {
      get { return GetAppSetting("StripePlanId", "####################"); }
    }

    public static string StripeApiKey
    {
      get { return GetAppSetting("StripeApiKey", "####################"); }
    }

    public static string StripePublicKey
    {
      get { return GetAppSetting("StripePublicKey", "####################"); }
    }

    public static string ExampleReportUrl
    {
      get { return GetAppSetting("ExampleReportUrl", "####################"); }
    }

    private static T GetAppSetting<T>(string key, T defaultValue)
    {
      if (!string.IsNullOrEmpty(key))
      {
        // Get the value from the config.
        var value = ConfigurationManager.AppSettings[key];

        try
        {
          if (value != null)
          {
            var theType = typeof(T);
            if (theType.IsEnum)
            {
              return (T)Enum.Parse(theType, value, true);
            }

            return (T)Convert.ChangeType(value, theType);
          }

          return defaultValue;
        }
        catch (Exception)
        {
          // Unable to get config value for key - default will be returned.
        }
      }


      return defaultValue;
    }
  }
}
