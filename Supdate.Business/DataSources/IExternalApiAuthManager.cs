using System;
using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Business.DataSources
{
  public interface IExternalApiAuthManager : IManager<ExternalApiAuth>
  {
    void SaveExternalApiAuth(ExternalApiAuth externalApiAuth);
    IEnumerable<ExternalApiAuth> GetExternalApiAuths(int companyId);
    ExternalApiAuth GetExternalApiAuth(int companyId, Guid uniqueId);
    ExternalApiAuth GetExternalApiAuth(int companyId, int externalApiId);
    void DeleteExternalApiAuth(int companyId, Guid uniqueId);
    IExternalApiManager GetApiManager(int id);
    ExternalApiAuth GetByCompanyGuid(Guid companyGuid, int externalApiId);
    void SaveWithCompanyGuid(Guid companyGuid, ExternalApiAuth externalApiAuth);
  }
}
