using System;
using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class DataSourcePanelViewModel
  {
    public Guid CompanyGuid { get; set; }
    public ExternalApi ExternalApi { get; set; }
    public IEnumerable<ExternalApiAuth> ExternalApiAuths { get; set; }
  }

  public class OAuthStartAuthorisation
  {
    public string StartUrl { get; set; }
    public ExternalApi ExternalApi { get; set; }
  }

  public class GoogleOAuthConfigView
  {
    public GoogleAnalyticsAccountsResponse Accounts { get; set; }
    public ExternalApi ExternalApi { get; set; }
    public ExternalApiAuth ExternalApiAuth { get; set; }
  }
}
