using System;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class UserConfirmation : ModelBase
  {
    public int UserId { get; set; }

    [Editable(false)]
    public Guid UniqueId { get; set; }

    [Editable(false)]
    public string Email { get; set; }

    [Editable(false)]
    public int RemindersSent { get; set; }

    public string Url { get; set; }
  }
}
