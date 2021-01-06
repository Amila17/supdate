using System;
using System.Collections.Generic;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public interface IExternalApiAuthRepository : ICrudRepository<ExternalApiAuth>
  {
    ExternalApiAuth GetByCompanyGuid(Guid companyGuid, int externalApiId);
  }
}
