using System.Data;
using Dapper;
using Supdate.Data.Base;

namespace Supdate.Data
{
  public class TermsAndConditionsRepository : RepositoryBase, ITermsAndConditionsRepository
  {
    public void AcceptTermsAndConditions(int userId)
    {
      try
      {
        OpenConnection();
        Connection.Execute("TermsAndConditionsAcceptLatest", new { userId }, commandType: CommandType.StoredProcedure);
      }
      finally
      {
        CloseConnection();
      }
    }

    public bool HasPendingTermsAndConditions(int userId)
    {
      try
      {
        OpenConnection();
        var hasPendingTermsAndConditions = Connection.ExecuteScalar<bool>("TermsAndConditionsHasPending", new { userId }, commandType: CommandType.StoredProcedure);

        return hasPendingTermsAndConditions;
      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
