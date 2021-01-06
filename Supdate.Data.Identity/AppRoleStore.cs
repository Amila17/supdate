using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Supdate.Model.Base;

namespace Supdate.Data.Identity
{
  public class AppRoleStore<TUser, TRole> : IdentityRepository<TRole>,
    IQueryableRoleStore<TRole, int>,
    IRoleStore<TRole, int>,
    IDisposable
    where TRole : ModelBase, IRole<int> where TUser : ModelBase, IUser<int>
  {
    private readonly IdentityDatabaseContext<TUser, TRole> _databaseContext;

    public AppRoleStore(IdentityDatabaseContext<TUser, TRole> databaseContext)
    {
      _databaseContext = databaseContext;
    }
    
    public Task CreateAsync(TRole role)
    {
      return InsertAsync(role);
    }

    Task IRoleStore<TRole, int>.UpdateAsync(TRole role)
    {
      return UpdateAsync(role);
    }

    public Task DeleteAsync(TRole role)
    {
      return RemoveAsync(role);
    }

    public Task<TRole> FindByIdAsync(int roleId)
    {
      return GetAsync(roleId);
    }

    public async Task<TRole> FindByNameAsync(string roleName)
    {
      var roles = await _databaseContext.RoleRepository.GetListAsync(new { UserName = roleName });

      return roles.FirstOrDefault();
    }

    public IQueryable<TRole> Roles
    {
      get { return GetList().AsQueryable(); }
    }
  }
}