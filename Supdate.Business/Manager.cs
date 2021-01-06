using System.Collections.Generic;
using Supdate.Data.Base;
using Supdate.Model.Base;

namespace Supdate.Business
{
  public class Manager<T> : IManager<T> where T : ModelBase
  {
    protected readonly ICrudRepository<T> Repository;

    public Manager(ICrudRepository<T> repository)
    {
      Repository = repository;
    }

    public virtual T Create(T model)
    {
      return Repository.Create(model);
    }

    public virtual T Get(int id)
    {
      return Repository.Get(id);
    }

    public virtual IEnumerable<T> GetList(object predicate = null)
    {
      return Repository.GetList(predicate);
    }

    public virtual IEnumerable<T> GetList(string whereClause)
    {
      return Repository.GetList(whereClause);
    }

    public virtual T Update(T model)
    {
      return Repository.Update(model);
    }

    public virtual bool Delete(int id)
    {
      return Repository.Delete(id);
    }
  }
}
