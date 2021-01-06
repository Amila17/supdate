using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Util;
using Newtonsoft.Json;
using Supdate.Model;
using Supdate.Model.Exceptions;
using Supdate.Util;

// https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth#web-applications-aspnet-mvc

namespace Supdate.Business.DataSources
{
  public class GoogleAuthorizer : IGoogleAuthorizer
  {
    private static IGoogleOAuthDataStore _googleOAuthDataStore;

    public GoogleAuthorizer(IGoogleOAuthDataStore googleOAuthDataStore)
    {
      _googleOAuthDataStore = googleOAuthDataStore;
    }

    public async Task<AuthorizationCodeWebApp.AuthResult> Authorize(Guid companyGuid, ExternalApiAuth externalApiAuth, string state, CancellationToken token)
    {
      var appFlowMetaData = GetAppFlowMetaData(companyGuid);

      var callbackUrl = string.Format("{0}{1}", ConfigUtil.BaseAppUrl, appFlowMetaData.AuthCallback.Substring(1));
      var result = await new AuthorizationCodeWebApp(appFlowMetaData.Flow, callbackUrl, state).AuthorizeAsync(companyGuid.ToString(), token);

      // Check if token has expired
      if (result.Credential != null)
      {
        if (result.Credential.Token.IsExpired(SystemClock.Default))
        {
          // token has expired we should refresh it
          var refresh = result.Credential.RefreshTokenAsync(CancellationToken.None).Result;
        }
      }

      return result;
    }

    public AppFlowMetadata GetAppFlowMetaData(Guid companyGuid)
    {
      return new AppFlowMetadata(companyGuid, _googleOAuthDataStore);
    }

    public async Task<GoogleAnalyticsAccountsResponse> GetAccountSummaries(AuthorizationCodeWebApp.AuthResult authResult)
    {
      var client = new HttpClient { BaseAddress = new Uri("https://www.googleapis.com") };

      var response = await client.GetAsync(string.Format("/analytics/v3/management/accountSummaries?oauth_token={0}", authResult.Credential.Token.AccessToken));

      if (response.IsSuccessStatusCode)
      {
        // Process resultant JSON
        var json = await response.Content.ReadAsStringAsync();
        var results = JsonConvert.DeserializeObject<GoogleAnalyticsAccountsResponse>(json);

        return results;
      }

      throw new BusinessException("Error getting Google Accounts list");
    }
  }
}
