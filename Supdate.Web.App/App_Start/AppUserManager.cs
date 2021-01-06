using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Supdate.Business;
using Supdate.Data.Identity;
using Supdate.Model;
using Supdate.Model.Identity;
using Supdate.Web.App.Models;

namespace Supdate.Web.App
{
  public class AppUserManager : UserManager<AppUser, int>
  {
    public AppUserManager(IUserStore<AppUser, int> store)
      : base(store)
    {
    }

    public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
    {
      var userStore = new AppUserStore<AppUser>(context.Get<IdentityDatabaseContext<AppUser, IdentityRole>>());
      var manager = new AppUserManager(userStore);

      // Configure validation logic for usernames
      manager.UserValidator = new UserValidator<AppUser, int>(manager)
                              {
                                AllowOnlyAlphanumericUserNames = false,
                                RequireUniqueEmail = true
                              };

      // Configure validation logic for passwords
      manager.PasswordValidator = new PasswordValidator
                                  {
                                    RequiredLength = 6
                                  };

      // Configure user lockout defaults
      manager.UserLockoutEnabledByDefault = true;
      manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
      manager.MaxFailedAccessAttemptsBeforeLockout = 5;

      // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
      // You can write your own provider and plug in here.
      //      manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<AppUser, int>
      //      {
      //        MessageFormat = "Your security code is: {0}"
      //      });
      //      manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<AppUser, int>
      //      {
      //        Subject = "SecurityCode",
      //        BodyFormat = "Your security code is {0}"
      //      });

      manager.EmailService = new EmailService();

      var dataProtectionProvider = options.DataProtectionProvider;
      if (dataProtectionProvider != null)
      {
        manager.UserTokenProvider = new DataProtectorTokenProvider<AppUser, int>(dataProtectionProvider.Create("ASP.NET Identity"));
      }

      return manager;
    }

    public async Task<LiteUser> GetUserFromUniqueId(IOwinContext context, Guid uniqueId)
    {
      var userStore = new AppUserStore<AppUser>(context.Get<IdentityDatabaseContext<AppUser, IdentityRole>>());

      var user = await userStore.FindByUniqueIdAsync(uniqueId);

      if (user != null)
      {
        return user;
      }

      return null;
    }

    public LiteUser GetUserAttributes(IOwinContext context, int userId)
    {
      var userStore = new AppUserStore<AppUser>(context.Get<IdentityDatabaseContext<AppUser, IdentityRole>>());

      var user = userStore.GetUserAttributes(userId);

      return user;
    }

    public void ChangeDefaultCompany(IOwinContext context, Guid companyUniqueId, int userId)
    {
      var userStore = new AppUserStore<AppUser>(context.Get<IdentityDatabaseContext<AppUser, IdentityRole>>());
      userStore.ChangeDefaultCompany(companyUniqueId, userId);
    }

    public void SaveProfile(IOwinContext context, LiteUser user)
    {
      var userStore = new AppUserStore<AppUser>(context.Get<IdentityDatabaseContext<AppUser, IdentityRole>>());
      userStore.SaveProfile(user);
    }

    public bool IsUserAdminOfCompany(LiteUser currentUser, int companyId)
    {
      if (currentUser == null)
      {
        return false;
      }

      if (currentUser.CompanyId == companyId)
      {
        return true;
      }

      return false;
    }
  }
}
