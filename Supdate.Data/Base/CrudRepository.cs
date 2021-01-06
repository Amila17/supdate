using System.Collections.Generic;
using Dapper;
using Supdate.Model.Base;

namespace Supdate.Data.Base
{
  public class CrudRepository<T> : RepositoryBase, ICrudRepository<T> where T : ModelBase
  {
    public virtual T Create(T model)
    {
      try
      {
        OpenConnection();

        var id = Connection.Insert<int>(model);

        // Fetch the model and return.
        return Get(id);
      }
      finally
      {
        CloseConnection();
      }
    }

    public virtual T Get(int id)
    {
      try
      {
        OpenConnection();

        return Connection.Get<T>(id);
      }
      finally
      {
        CloseConnection();
      }
    }

    public virtual IEnumerable<T> GetList(object predicate = null)
    {
      try
      {
        OpenConnection();

        return Connection.GetList<T>(predicate);
      }
      finally
      {
        CloseConnection();
      }
    }

    public virtual IEnumerable<T> GetList(string whereClause)
    {
      try
      {
        OpenConnection();

        return Connection.GetList<T>(whereClause);
      }
      finally
      {
        CloseConnection();
      }
    }

    public virtual T Update(T model)
    {
      try
      {
        OpenConnection();
        var updatedModel = Connection.Update(model);

        return model;
      }
      finally
      {
        CloseConnection();
      }
    }

    public virtual bool Delete(int id)
    {
      try
      {
        OpenConnection();

        return Connection.Delete<T>(id) > 0;
      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
