using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class CompaniesViewModel
  {
    public IEnumerable<Company> OwnCompanies;
    public IEnumerable<Company> OtherCompanies;
  }
}
