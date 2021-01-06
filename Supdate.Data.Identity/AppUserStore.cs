using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNet.Identity;
using Supdate.Model;
using Supdate.Model.Identity;

namespace Supdate.Data.Identity
{
  public class AppUserStore<TUser> : IdentityRepository<TUser>,
    IUserLoginStore<TUser, int>,
    // IUserRoleStore<TUser, int>,
    IUserClaimStore<TUser, int>,
    IUserPasswordStore<TUser, int>,
    IUserSecurityStampStore<TUser, int>,
    IQueryableUserStore<TUser, int>,
    IUserEmailStore<TUser, int>,
    IUserPhoneNumberStore<TUser, int>,
    IUserTwoFactorStore<TUser, int>,
    IUserLockoutStore<TUser, int>,
    IUserStore<TUser, int>
    where TUser : IdentityUser, IUser<int>
  {
    private readonly IdentityDatabaseContext<TUser, IdentityRole> _databaseContext;

    public AppUserStore(IdentityDatabaseContext<TUser, IdentityRole> databaseContext)
    {
      _databaseContext = databaseContext;
    }

    public Task CreateAsync(TUser user)
    {
      // Generate the unique id.
      user.UniqueId = Guid.NewGuid();

      return InsertAsync(user);
    }

    Task IUserStore<TUser, int>.UpdateAsync(TUser user)
    {
      return UpdateAsync(user);
    }

    public Task DeleteAsync(TUser user)
    {
      return RemoveAsync(user);
    }

    public Task<TUser> FindByIdAsync(int userId)
    {
      return GetAsync(userId);
    }

    public async Task<LiteUser> FindByUniqueIdAsync(Guid uniqueId)
    {
      try
      {
        OpenConnection();

        var results = await Connection.QueryAsync<LiteUser>("UserGetByUniqueId", new { UniqueId = uniqueId }, commandType: CommandType.StoredProcedure);

        var user = results.Single();
        if (user != null)
        {
          return user;
        }

      }
      finally
      {
        CloseConnection();
      }

      return null;
    }

    public LiteUser GetUserAttributes(int userId)
    {
      try
      {
        OpenConnection();

        var results = Connection.QueryMultiple("UserGetAttributes", new { userId }, commandType: CommandType.StoredProcedure);
        var user = results.Read<LiteUser>().FirstOrDefault();
        if (user != null)
        {
          user.OtherCompanies = results.Read<Company>().ToList();
          // remove current company
          user.OtherCompanies.Remove(user.OtherCompanies.SingleOrDefault(c => c.Id == user.CompanyId));

          user.AccessibleAreaIds = results.Read<int>().ToArray();
          return user;
        }

      }
      finally
      {
        CloseConnection();
      }

      return null;
    }

    public void ChangeDefaultCompany(Guid companyUniqueId, int userId)
    {
      try
      {
        OpenConnection();
        Connection.Execute("UserChangeDefaultCompany", new { CompanyUniqueId = companyUniqueId, UserId  = userId}, commandType: CommandType.StoredProcedure);
      }
      finally
      {
        CloseConnection();
      }

    }
    public void SaveProfile(LiteUser user)
    {
      try
      {
        OpenConnection();
        Connection.Execute("UserProfileUpdate", new { user.Id, user.FirstName, user.LastName }, commandType: CommandType.StoredProcedure);
      }
      finally
      {
        CloseConnection();
      }

    }

    public async Task<TUser> FindByNameAsync(string userName)
    {
      var users = await GetListAsync(new { UserName = userName });

      return users.FirstOrDefault();
    }

    public async Task AddLoginAsync(TUser user, UserLoginInfo login)
    {
      var userLogin = new UserLogin
                      {
                        UserId = user.Id,
                        LoginProvider = login.LoginProvider,
                        ProviderKey = login.ProviderKey
                      };

      await _databaseContext.UserLoginRepository.InsertAsync(userLogin);
    }

    public async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
    {
      var userLogin = new UserLogin
                      {
                        UserId = user.Id,
                        LoginProvider = login.LoginProvider,
                        ProviderKey = login.ProviderKey
                      };

      await _databaseContext.UserLoginRepository.RemoveAsync(userLogin);
    }

    public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
    {
      var userLogins = await _databaseContext.UserLoginRepository.GetListAsync(new {UserId = user.Id});

      var userLoginInfoList = new List<UserLoginInfo>();
      foreach (var userLogin in userLogins)
      {
        userLoginInfoList.Add(new UserLoginInfo(userLogin.LoginProvider, userLogin.ProviderKey));
      }

      return userLoginInfoList;
    }

    public async Task<TUser> FindAsync(UserLoginInfo login)
    {
      var userLogins = await _databaseContext.UserLoginRepository.GetListAsync(new { LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey });

      var userLogin = userLogins.FirstOrDefault();

      if (userLogin != null)
      {
        return Get(userLogin.UserId);
      }

      return null;
    }

    public async Task SetPasswordHashAsync(TUser user, string passwordHash)
    {
      user.PasswordHash = passwordHash;

      await UpdateAsync(user);
    }

    public async Task<string> GetPasswordHashAsync(TUser user)
    {
      return await Task.FromResult(user.PasswordHash);
    }

    public async Task<bool> HasPasswordAsync(TUser user)
    {
      var passwordHash = await GetPasswordHashAsync(user);

      return string.IsNullOrEmpty(passwordHash);
    }

    public async Task SetEmailAsync(TUser user, string email)
    {
      user.Email = email;

      await UpdateAsync(user);
    }

    public async Task<string> GetEmailAsync(TUser user)
    {
      return await Task.FromResult(user.Email);
    }

    public async Task<bool> GetEmailConfirmedAsync(TUser user)
    {
      return await Task.FromResult(user.EmailConfirmed);
    }

    public async Task SetEmailConfirmedAsync(TUser user, bool confirmed)
    {
      user.EmailConfirmed = confirmed;

      await UpdateAsync(user);
    }

    public async Task<TUser> FindByEmailAsync(string email)
    {
      var users = await GetListAsync(new { Email = email });

      return users.FirstOrDefault();
    }

    public async Task SetSecurityStampAsync(TUser user, string stamp)
    {
      user.SecurityStamp = stamp;
      await UpdateAsync(user);
    }

    public async Task<string> GetSecurityStampAsync(TUser user)
    {
      return await Task.FromResult(user.SecurityStamp);
    }

    public async Task SetPhoneNumberAsync(TUser user, string phoneNumber)
    {
      user.PhoneNumber = phoneNumber;
      await UpdateAsync(user);
    }

    public async Task<string> GetPhoneNumberAsync(TUser user)
    {
      return await Task.FromResult(user.PhoneNumber);
    }

    public async Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
    {
      return await Task.FromResult(user.PhoneNumberConfirmed);
    }

    public async Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
    {
      user.PhoneNumberConfirmed = confirmed;
      await UpdateAsync(user);
    }

    public async Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
    {
      user.TwoFactorEnabled = enabled;
      await UpdateAsync(user);
    }

    public async Task<bool> GetTwoFactorEnabledAsync(TUser user)
    {
      return await Task.FromResult(user.TwoFactorEnabled);
    }

    public async Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
    {
      return await Task.FromResult(user.LockoutEndDateUtc.HasValue
              ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
              : new DateTimeOffset());
    }

    public async Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
    {
      user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
      await UpdateAsync(user);
    }

    public async Task<int> IncrementAccessFailedCountAsync(TUser user)
    {
      user.AccessFailedCount++;
      await UpdateAsync(user);

      return user.AccessFailedCount;
    }

    public async Task ResetAccessFailedCountAsync(TUser user)
    {
      user.AccessFailedCount = 0;
      await UpdateAsync(user);
    }

    public async Task<int> GetAccessFailedCountAsync(TUser user)
    {
      return await Task.FromResult(user.AccessFailedCount);
    }

    public async Task<bool> GetLockoutEnabledAsync(TUser user)
    {
      return await Task.FromResult(user.LockoutEnabled);
    }

    public async Task SetLockoutEnabledAsync(TUser user, bool enabled)
    {
      user.LockoutEnabled = enabled;
      await UpdateAsync(user);
    }

    public async Task AddToRoleAsync(TUser user, string roleName)
    {
      var roles = (await _databaseContext.RoleRepository.GetListAsync(new {Name = roleName})).ToList();
      if (roles.Any())
      {
        var roleId = roles.First().Id;
        var userRole = new UserRole()
                       {
                         RoleId = roleId,
                         UserId = user.Id,
                       };
        await _databaseContext.UserRoleRepository.InsertAsync(userRole);
      }
    }

    public async Task RemoveFromRoleAsync(TUser user, string roleName)
    {
      var roles = (await _databaseContext.RoleRepository.GetListAsync(new { Name = roleName })).ToList();
      if (roles.Any())
      {
        var role = roles.First();
        await _databaseContext.UserRoleRepository.RemoveAsync(c => c.RoleId == role.Id && c.UserId == user.Id);
      }
    }

    public Task<IList<string>> GetRolesAsync(TUser user)
    {
//      var roles = await _databaseContext.RoleRepository.GetListAsync();
//      IList<string> rs = new List<string>();
//      if (roles != null)
//      {
//        foreach (var role in roles)
//        {
//          rs.Add(role);
//        }
//      }
//      return rs;
      throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(TUser user, string roleName)
    {
      throw new NotImplementedException();
    }

    public IQueryable<TUser> Users
    {
      get
      {
        return GetList().AsQueryable();
      }
    }

    public async Task<IList<Claim>> GetClaimsAsync(TUser user)
    {
      var userClaims = await _databaseContext.UserClaimRepository.GetListAsync(new { UserId = user.Id });

      return userClaims.Select(userClaim => new Claim(userClaim.ClaimType, userClaim.ClaimValue)).ToList();
    }

    public async Task AddClaimAsync(TUser user, Claim claim)
    {
      var existingClaims = (_databaseContext.UserClaimRepository.GetList(new { UserId = user.Id, ClaimType = claim.Type })).ToList();

      if (existingClaims.Count > 1)
      {
        throw new InvalidOperationException("Multiple claims for same type.");
      }

      // Update existing only if value is different.
      if ((existingClaims.Count == 1) && (existingClaims[0].ClaimValue != claim.Value))
      {
        existingClaims[0].ClaimValue = claim.Value;
        await _databaseContext.UserClaimRepository.UpdateAsync(existingClaims[0]);
      }

      // Only add if the claim type for this user is not found.
      if (existingClaims.Count == 0)
      {
        await _databaseContext.UserClaimRepository.InsertAsync(new UserClaim { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
      }
    }

    public Task RemoveClaimAsync(TUser user, Claim claim)
    {
      return _databaseContext.UserClaimRepository.RemoveAsync(new UserClaim { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
    }
  }
}
