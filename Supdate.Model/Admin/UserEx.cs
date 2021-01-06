using System;
using System.Collections.Generic;
using Supdate.Model.Identity;

namespace Supdate.Model.Admin
{
  public class UserEx : IdentityUser
  {
    public int CompanyId { get; set; }

    public string CompanyName { get; set; }

    public Guid CompanyUniqueId { get; set; }

    public int CompanyCount { get; set; }

    public bool IsCompanyAdmin { get; set; }

    public IEnumerable<Company> OwnCompanies;

    public IEnumerable<Company> OtherCompanies;
  }
}
