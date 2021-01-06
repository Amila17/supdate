using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Supdate.Web.App.Models;

namespace Supdate.Web.App
{
  public class AppSignInManager : SignInManager<AppUser, int>
  {
    public AppSignInManager(AppUserManager userManager, IAuthenticationManager authenticationManager)
      : base(userManager, authenticationManager)
    {
    }

    public override Task<ClaimsIdentity> CreateUserIdentityAsync(AppUser user)
    {
      return user.GenerateUserIdentityAsync((AppUserManager)UserManager);
    }

    public static AppSignInManager Create(IdentityFactoryOptions<AppSignInManager> options, IOwinContext context)
    {
      return new AppSignInManager(context.GetUserManager<AppUserManager>(), context.Authentication);
    }
  }
}