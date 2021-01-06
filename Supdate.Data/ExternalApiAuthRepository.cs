using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supdate.Data.Base;
using Supdate.Model;
using Supdate.Util;

namespace Supdate.Data
{
  public class ExternalApiAuthRepository : CrudRepository<ExternalApiAuth>, IExternalApiAuthRepository
  {
    public ExternalApiAuth GetByCompanyGuid(Guid companyGuid, int externalApiId)
    {
      try
      {
        OpenConnection();
        return Connection.Query<ExternalApiAuth>("ExternalApiAuthGetByCompanyGuid", new { companyGuid, externalApiId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
