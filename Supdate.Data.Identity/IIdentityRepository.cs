using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Supdate.Model.Base;

namespace Supdate.Data.Identity
{
  public interface IIdentityRepository<T> where T : ModelBase
  {
    int Insert(T model);
    Task<int> InsertAsync(T model);

    bool Remove(T model);
    Task<bool> RemoveAsync(T model);
    bool RemoveAll();
    Task<bool> RemoveAllAsync();

    void Remove(Expression<Func<T, bool>> predicate);
    Task RemoveAsync(Expression<Func<T, bool>> predicate);

    bool Update(T model);
    Task<bool> UpdateAsync(T model);

    T Get(int id);
    Task<T> GetAsync(int id);
    IEnumerable<T> GetList(object predicate = null);
    Task<IEnumerable<T>> GetListAsync(object predicate = null);
    IEnumerable<T> GetList(string whereClause);
  }
}
