using System;
using System.Collections.Generic;

namespace Supdate.Model.Admin
{
  public class SearchResults
  {
    
    public IEnumerable<LiteUser> Users { get; set; }

    public IEnumerable<Company> Companies;
  }
}
