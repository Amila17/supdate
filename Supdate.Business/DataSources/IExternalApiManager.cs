using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Web;
using Supdate.Model;

namespace Supdate.Business.DataSources
{
  public interface IExternalApiManager
  {
    IList<MetricDataSource> MetricDataSources();
    Task<bool> TestCredentials(string token, string key, int externalApiId);
    Task<MetricDataImport> GetData(int companyId, ExternalApiAuth externalApiAuth, IEnumerable<MetricDataPoint> metricDataPoints);
    Task<MetricDataImport> GetData(int companyId, ExternalApiAuth externalApiAuth, AuthorizationCodeWebApp.AuthResult authResult, IEnumerable<MetricDataPoint> metricDataPoints);
  }
}
