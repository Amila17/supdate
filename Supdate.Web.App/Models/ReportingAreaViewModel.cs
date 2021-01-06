using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class ReportingAreaViewModel
  {
   public IEnumerable<Area> Areas { get; set; }
   public ListHelper ListHelper;

  }

}
