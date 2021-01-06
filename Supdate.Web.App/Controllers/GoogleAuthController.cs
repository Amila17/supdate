using System;
using Supdate.Business.DataSources;
using Supdate.Model.Exceptions;

namespace Supdate.Web.App.Controllers
{
  public class AuthCallbackController : Google.Apis.Auth.OAuth2.Mvc.Controllers.AuthCallbackController
  {
    private readonly IGoogleAuthorizer _googleAuthorizer;
    public AuthCallbackController(IGoogleAuthorizer googleAuthorizer)
    {
      _googleAuthorizer = googleAuthorizer;
    }

    protected override Google.Apis.Auth.OAuth2.Mvc.FlowMetadata FlowData
    {
      get
      {
        Guid companyGuid;

        if (Guid.TryParse(Session["CompanyGuid"].ToString(), out companyGuid))
        {
          return _googleAuthorizer.GetAppFlowMetaData(companyGuid);
        }

        throw new BusinessException("Received Token for Unknown Company");
      }
    }
  }
}
