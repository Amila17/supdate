using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Supdate.Data.Base;
using Supdate.Model.Base;

namespace Supdate.Data.Identity
{
  public class IdentityRepository<T> : RepositoryBase, IIdentityRepository<T> where T : ModelBase
  {
    public int Insert(T model)
    {
      try
      {
        OpenConnection();

        return Connection.Insert<int>(model);
      }
      finally
      {
        CloseConnection();
      }
    }

    public async Task<int> InsertAsync(T model)
    {
      try
      {
        OpenConnection();

        var insertedModelId = await Connection.InsertAsync<int>(model);
        model.Id = insertedModelId;

        return insertedModelId;
      }
      finally
      {
        CloseConnection();
      }
    }

    public bool Remove(T model)
    {
      try
      {
        OpenConnection();

        return Connection.Delete<int>(model) > 0;
      }
      finally
      {
        CloseConnection();
      }
    }

    public async Task<bool> RemoveAsync(T model)
    {
      try
      {
        OpenConnection();

        return await Connection.DeleteAsync<int>(model) > 0;
      }
      finally
      {
        CloseConnection();
      }
    }

    public bool RemoveAll()
    {
      throw new NotImplementedException();
    }

    public Task<bool> RemoveAllAsync()
    {
      throw new NotImplementedException();
    }

    public void Remove(Expression<Func<T, bool>> predicate)
    {
      throw new NotImplementedException();
    }

    public Task RemoveAsync(Expression<Func<T, bool>> predicate)
    {
      throw new NotImplementedException();
    }

    public bool Update(T model)
    {
      try
      {
        OpenConnection();

        return Connection.Update(model) > 0;
      }
      finally
      {
        CloseConnection();
      }
    }

    public async Task<bool> UpdateAsync(T model)
    {
      try
      {
        OpenConnection();

        return await Connection.UpdateAsync(model) > 0;
      }
      finally
      {
        CloseConnection();
      }
    }

    public T Get(int id)
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

    public async Task<T> GetAsync(int id)
    {
      try
      {
        OpenConnection();

        return await Connection.GetAsync<T>(id);
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<T> GetList(object predicate = null)
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

    public async Task<IEnumerable<T>> GetListAsync(object predicate = null)
    {
      try
      {
        OpenConnection();

        return await Connection.GetListAsync<T>(predicate);
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<T> GetList(string whereClause)
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
  }
}
