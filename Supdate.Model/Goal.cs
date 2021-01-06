using System;
using System.ComponentModel;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class Goal : GoalBase
  {
    public int CompanyId { get; set; }

    [ReadOnly(true)]
    public Guid UniqueId { get; set; }

    /// <summary>
    /// Id of the associated reporting area
    /// </summary>
    public int? AreaId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }
  }
}
