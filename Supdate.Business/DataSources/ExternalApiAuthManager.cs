using System;
using System.Collections.Generic;
using System.Linq;
using Supdate.Data;
using Supdate.Model;
using Supdate.Model.Exceptions;

namespace Supdate.Business.DataSources
{
  public class ExternalApiAuthManager : Manager<ExternalApiAuth>, IExternalApiAuthManager
  {
    private static IExternalApiAuthRepository _externalApiAuthRepository;
    private readonly IGoogleAnalyticsApiManager _googleAnalyticsApiManager;
    private readonly IChartMogulApiManager _chartMogulApiManager;
    private readonly ICompanyManager _companyManager;

    public ExternalApiAuthManager(IExternalApiAuthRepository externalApiAuthRepository,
      IChartMogulApiManager chartMogulApiManager, IGoogleAnalyticsApiManager googleAnalyticsApiManager,
      ICompanyManager companyManager)
      : base(externalApiAuthRepository)
    {
      _externalApiAuthRepository = externalApiAuthRepository;
      _googleAnalyticsApiManager = googleAnalyticsApiManager;
      _chartMogulApiManager = chartMogulApiManager;
      _companyManager = companyManager;
    }

    public void SaveExternalApiAuth(ExternalApiAuth externalApiAuth)
    {
      if (externalApiAuth.Id > 0)
      {
        var check = _externalApiAuthRepository.Get(externalApiAuth.Id);
        if (check.CompanyId == externalApiAuth.CompanyId)
        {
          _externalApiAuthRepository.Update(externalApiAuth);
        }
      }
      else
      {
        _externalApiAuthRepository.Create(externalApiAuth);
      }
    }

    public IEnumerable<ExternalApiAuth> GetExternalApiAuths(int companyId)
    {
      return _externalApiAuthRepository.GetList(new { companyId });
    }

    public ExternalApiAuth GetExternalApiAuth(int companyId, Guid uniqueId)
    {
      return _externalApiAuthRepository.GetList(new { companyId, uniqueId }).FirstOrDefault();
    }

    public ExternalApiAuth GetExternalApiAuth(int companyId, int externalApiId)
    {
      return _externalApiAuthRepository.GetList(new { companyId, externalApiId }).FirstOrDefault();
    }

    public void DeleteExternalApiAuth(int companyId, Guid uniqueId)
    {
      var externalApiAuth = GetExternalApiAuth(companyId, uniqueId);
      _externalApiAuthRepository.Delete(externalApiAuth.Id);
    }

    public IExternalApiManager GetApiManager(int id)
    {
      switch (id)
      {
        case 1:
          return _chartMogulApiManager;

        case 2:
          return _googleAnalyticsApiManager;

        default:
          throw new BusinessException("No API Manager Found");
      }
    }

    public ExternalApiAuth GetByCompanyGuid(Guid companyGuid, int externalApiId)
    {
      return _externalApiAuthRepository.GetByCompanyGuid(companyGuid, externalApiId);
    }

    public void SaveWithCompanyGuid(Guid companyGuid, ExternalApiAuth externalApiAuth)
    {
      var company = _companyManager.GetList(new { uniqueId = companyGuid }).FirstOrDefault();
      externalApiAuth.CompanyId = company.Id;

      _externalApiAuthRepository.Create(externalApiAuth);
    }
  }
}
