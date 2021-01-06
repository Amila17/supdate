using System;
using System.ComponentModel;

namespace Supdate.Model.Base
{
  public abstract class ModelBase
  {
    public int Id { get; set; }

    [ReadOnly(true)]
    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate
    {
      get { return DateTime.UtcNow; }
    }
  }
}