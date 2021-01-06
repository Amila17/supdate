using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Supdate.Model.Identity;

namespace Supdate.Web.App.Models
{
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class AppUser : IdentityUser
  {
    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(AppUserManager manager)
    {
      // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
      var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

      // Store the company id in a claim.
      // manager.AddClaim(Id, new Claim("CompanyId", CompanyId.ToString()));

      // Add custom user claims here
      return userIdentity;
    }

    [MaxLength(256)]
    public string UnConfirmedEmail { get; set; }
  }
}
