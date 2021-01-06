using System;
using System.Collections.Generic;
using System.Linq;
using Supdate.Model;
using Supdate.Util;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Supdate.Business
{
  public class SlackManager : ISlackManager
  {
    private readonly IWebhookManager _webhookManager;
    private const string SlackBaseUrl = "https://slack.com/";
    private const string AuthorizeEndpoint = "oauth/authorize";
    private const string AccessTokenEndpoint = "api/oauth.access";

    private readonly Encoding _encoding = new UTF8Encoding();

    public SlackManager(IWebhookManager webhookManager)
    {
      _webhookManager = webhookManager;
    }

    public string Authorize(string returnUrl)
    {
      // The following values should be passed as GET parameters:

      //  client_id    - issued when you created your app (required)
      //  scope        - permissions to request (see below) (required)
      //  redirect_uri - URL to redirect back to (see below) (optional)
      //  state        - unique string to be passed back upon completion (optional)
      //  team         - Slack team ID to restrict to (optional)

      return string.Format("{0}{1}?client_id={2}&scope=incoming-webhook&redirect_uri={3}", SlackBaseUrl, AuthorizeEndpoint, ConfigUtil.SlackClientId, returnUrl);
    }

    public void GetAccessToken(int companyId, string code, string returnUrl)
    {
      var client = new HttpClient { BaseAddress = new Uri(SlackBaseUrl) };

      var requestBody =
        new FormUrlEncodedContent(new[]
                                  {
                                    new KeyValuePair<string, string>("client_id", ConfigUtil.SlackClientId),
                                    new KeyValuePair<string, string>("client_secret", ConfigUtil.SlackClientSecret),
                                    new KeyValuePair<string, string>("code", code),
                                    new KeyValuePair<string, string>("redirect_uri", returnUrl)
                                  });

      var response = client.PostAsync(AccessTokenEndpoint, requestBody).Result;

      if (response.IsSuccessStatusCode)
      {
        var buffer = response.Content.ReadAsByteArrayAsync().Result;
        var responseJson = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

        var slackResponse = JObject.Parse(responseJson);

        if (slackResponse.GetValue("ok").ToString().ToLower() == "true")
        {
          var slackWebHookInfo = JObject.Parse(slackResponse.GetValue("incoming_webhook").ToString());
          var webhook = new Webhook
          {
            ServiceName = "Slack",
            CompanyId = companyId,
            WebhookUrl = slackWebHookInfo.GetValue("url").ToString(),
            ConfigUrl = slackWebHookInfo.GetValue("configuration_url").ToString(),
            ConfigInfo1 = slackResponse.GetValue("team_name").ToString(),
            ConfigInfo2 = slackWebHookInfo.GetValue("channel").ToString(),
            EventReportComment = true,
            EventReportViewed = true,
            EventReportingAreaUpdated = true
          };

          _webhookManager.Create(webhook);
        }
        else
        {
          throw new Exception(slackResponse.GetValue("error").ToString());
        }
      }
    }

    public IList<Webhook> GetWebhooks(int companyId)
    {
      return _webhookManager.GetList(new { CompanyId = companyId, ServiceName = "Slack" }).ToList();
    }

  }
}
