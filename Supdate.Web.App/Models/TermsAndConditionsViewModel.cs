using System.ComponentModel.DataAnnotations;

namespace Supdate.Web.App.Models
{
  public class TermsAndConditionsViewModel
  {
    public int UserId { get; set; }

    public bool TermsAndConditions { get; set; } = false;
  }
}
