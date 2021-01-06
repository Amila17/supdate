using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class Area : ModelBase
  {
    public int CompanyId { get; set; }

    [ReadOnly(true)]
    public Guid UniqueId { get; set; }

    [Required(ErrorMessage = "Please enter a name for this Reporting Area.")]
    [StringLength(200, ErrorMessage = "Reporting Area name cannot be longer than 200 characters.")]
    public string Name { get; set; }

    public int DisplayOrder { get; set; }
  }
}
