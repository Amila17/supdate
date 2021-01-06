using System;
using Supdate.Model.Base;
using Supdate.Model.Identity;

namespace Supdate.Data.Identity
{
  public class IdentityDatabaseContext<TUser, TRole> : IDisposable where TUser : ModelBase where TRole : ModelBase
  {
    private IIdentityRepository<TUser> _userRepository;
    private IIdentityRepository<UserLogin> _userLoginRepository;
    private IIdentityRepository<UserClaim> _userClaimRepository;
    private IIdentityRepository<TRole> _roleRepository;
    private IIdentityRepository<UserRole> _userRoleRepository;

    public static IdentityDatabaseContext<TUser, TRole> Create()
    {
      return new IdentityDatabaseContext<TUser, TRole>();
    }

    public IIdentityRepository<TUser> UserRepository
    {
      get
      {
        return _userRepository ?? (_userRepository = new IdentityRepository<TUser>());
      }
    }
    public IIdentityRepository<TRole> RoleRepository
    {
      get
      {
        return _roleRepository ?? (_roleRepository = new IdentityRepository<TRole>());
      }
    }
    public IIdentityRepository<UserRole> UserRoleRepository
    {
      get
      {
        return _userRoleRepository ?? (_userRoleRepository = new IdentityRepository<UserRole>());
      }
    }
    public IIdentityRepository<UserLogin> UserLoginRepository
    {
      get
      {
        return _userLoginRepository ?? (_userLoginRepository = new IdentityRepository<UserLogin>());
      }
    }
    public IIdentityRepository<UserClaim> UserClaimRepository
    {
      get
      {
        return _userClaimRepository ?? (_userClaimRepository = new IdentityRepository<UserClaim>());
      }
    }

    public void Dispose()
    {
    }
  }

}
