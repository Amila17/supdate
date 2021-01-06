using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Web;
using Supdate.Model;

namespace Supdate.Business.DataSources
{
  public interface IGoogleAuthorizer
  {
    Task<AuthorizationCodeWebApp.AuthResult> Authorize(Guid companyGuid, ExternalApiAuth externalApiAuth, string state, CancellationToken token);

    AppFlowMetadata GetAppFlowMetaData(Guid companyGuid);

    Task<GoogleAnalyticsAccountsResponse> GetAccountSummaries(AuthorizationCodeWebApp.AuthResult authResult);
  }
}
