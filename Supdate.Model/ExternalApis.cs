using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Exceptions;
using Newtonsoft.Json;

namespace Supdate.Model
{
  public class ExternalApi
  {
    public static readonly ExternalApi ChartMogul = new ExternalApi(
      1,
      "ChartMogul",
      "Connect to your <a href='http://chartmogul.com' target='_blank' class='link'>ChartMogul</a> account and you can map your Metrics to any of the values reported by ChartMogul.",
      "/Assets/external-logos/chartmogul.png",
      "https://api.chartmogul.com/v1/",
      "customers",
      "Log in to your ChartMogul account and click the gear icon at the bottom-left of the page.<br>Select API from the menu, then copy and paste your Token and Key into the boxes below.",
      ExternalApiAuthorizationType.TokenAndKey,
      string.Empty, string.Empty
    );
    public static readonly ExternalApi GoogleAnalytics = new ExternalApi(
      2,
      "Google Analytics",
      "Connect to <a href='https://analytics.google.com/' target='_blank' class='link'>Google Analytics</a> and instantly fill Metric values such as number of website visitors, conversion information and more.",
      "/Assets/external-logos/google-analytics.png",
      "https://accounts.google.com/",
      string.Empty,
      "Click the button below and you'll be sent to Google Analytics to authorize us to access your data.",
      ExternalApiAuthorizationType.OAuth20,
      "589674786225-3c12v5hid6li4053u5mt00eag3md32ak.apps.googleusercontent.com",
      "d06zjpVwm2fCpo-P3k3_wdZs"
     );


    public static IEnumerable<ExternalApi> Values
    {
      get
      {
        yield return ChartMogul;
        yield return GoogleAnalytics;
      }
    }

    private ExternalApi(int id, string name, string introText, string logo, string baseUrl, string testUrl, string setupText, ExternalApiAuthorizationType apiAuthorizationType, string consumerKey, string consumerSecret)
    {
      Id = id;
      Name = name;
      IntroText = introText;
      Logo = logo;
      BaseUrl = baseUrl;
      TestUrl = testUrl;
      SetupText = setupText;
      ApiAuthorizationType = apiAuthorizationType;
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string IntroText { get; private set; }
    public string Logo { get; private set; }
    public string BaseUrl { get; private set; }
    public string TestUrl { get; private set; }
    public string SetupText { get; private set; }
    public ExternalApiAuthorizationType ApiAuthorizationType { get; private set; }
    public string ConsumerKey { get; private set; }
    public string ConsumerSecret { get; private set; }

    public ExternalApi GetById(int id)
    {
      foreach (var obj in Values)
      {
        if (obj.Id == id)
        {
          return obj;
        }
      }

      throw new BusinessException(string.Format("An object with id {0} does not exist", id));
    }

    public override string ToString()
    {
      return Name;
    }
  }

  public enum ExternalApiAuthorizationType
  {
    [Display(Name = "Token and Key")]
    TokenAndKey = 0,

    [Display(Name = "oAuth 2.0")]
    OAuth20 = 1,
  }
  public class ChartMogulResponse
  {
    public List<ChartMogulEntry> Entries { get; set; }
  }

  public class ChartMogulEntry
  {
    public string Date { get; set; }
    [JsonProperty(PropertyName = "customer-churn-rate")]
    public double CustomerChurnRate { get; set; }
    [JsonProperty(PropertyName = "mrr-churn-rate")]
    public double MrrChurnRate { get; set; }
    public double Ltv { get; set; }
    public double Customers { get; set; }
    public double Asp { get; set; }
    public double Arpa { get; set; }
    public double Arr { get; set; }
    public double Mrr { get; set; }
  }

  public class GoogleAnalyticsAccountsResponse
  {
    public string kind { get; set; }
    public string username { get; set; }
    public int totalResults { get; set; }
    public int startIndex { get; set; }
    public int itemsPerPage { get; set; }
    public List<GoogleAnalyticsAccountsItem> items { get; set; }
  }

  public class GoogleAnalyticsAccountsItem
  {
    public string id { get; set; }
    public string kind { get; set; }
    public string name { get; set; }
    public List<WebProperty> webProperties { get; set; }
  }

  public class WebProperty
  {
    public string kind { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string internalWebPropertyId { get; set; }
    public string level { get; set; }
    public string websiteUrl { get; set; }
    public List<WebPropertyProfile> profiles { get; set; }
  }

  public class WebPropertyProfile
  {
    public string kind { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
  }
}
