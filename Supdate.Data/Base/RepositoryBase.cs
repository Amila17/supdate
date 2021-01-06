using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using StackExchange.Profiling;
using Supdate.Model.Exceptions;
using Supdate.Util;

namespace Supdate.Data.Base
{
  public class RepositoryBase : IDisposable
  {
    protected IDbConnection Connection;

    protected void OpenConnection()
    {
      // Check if connection instance already exists.
      if (Connection == null)
      {
        var connectionStringName = ConfigUtil.ConnectionStringName;
        var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];

        if (connectionString == null)
        {
          throw new ConfigurationErrorsException(MessageConstants.InvalidConnectionConfiguration);
        }

        Connection = new StackExchange.Profiling.Data.ProfiledDbConnection(new SqlConnection(connectionString.ConnectionString), MiniProfiler.Current);
      }

      // Open the connection if it is not open.
      if (Connection != null && Connection.State != ConnectionState.Open)
      {
        Connection.Open();
      }
    }

    protected void CloseConnection()
    {
      if (Connection != null && Connection.State == ConnectionState.Open)
      {
        Connection.Close();
      }
    }

    public void Dispose()
    {
      CloseConnection();
    }
  }
}
