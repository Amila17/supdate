using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Requests.Parameters;
using Supdate.Model;

namespace Supdate.Business.DataSources
{
  public class AppFlowMetadata : FlowMetadata
  {
    private static readonly string ConsumerKey = ExternalApi.GoogleAnalytics.ConsumerKey;
    private static readonly string ConsumerSecret = ExternalApi.GoogleAnalytics.ConsumerSecret;

    private readonly IAuthorizationCodeFlow _flow;

    private Guid _userId;

    public AppFlowMetadata(Guid userId, IGoogleOAuthDataStore dataStore)
    {
      _userId = userId;

      _flow =
        new ForceOfflineGoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                                                    {
                                                      ClientSecrets = new ClientSecrets { ClientId = ConsumerKey, ClientSecret = ConsumerSecret },
                                                      Scopes = new[] { AnalyticsReportingService.Scope.AnalyticsReadonly },
                                                      DataStore = dataStore
                                                    });
    }

    internal class ForceOfflineGoogleAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
    {
      public ForceOfflineGoogleAuthorizationCodeFlow(GoogleAuthorizationCodeFlow.Initializer initializer) : base(initializer) { }

      public override AuthorizationCodeRequestUrl CreateAuthorizationCodeRequest(string redirectUri)
      {
        return new GoogleAuthorizationCodeRequestUrl(new Uri(AuthorizationServerUrl))
               {
                 ClientId = ClientSecrets.ClientId,
                 Scope = string.Join(" ", Scopes),
                 RedirectUri = redirectUri,
                 AccessType = "offline",
                 ApprovalPrompt = "force"
               };
      }
    }

    public override string GetUserId(Controller controller)
    {
      return _userId.ToString();
    }

    public override IAuthorizationCodeFlow Flow
    {
      get { return _flow; }
    }
  }
}
