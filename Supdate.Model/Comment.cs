using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class Comment : ModelBase
  {
    public int DiscussionId { get; set; }

    [ReadOnly(true)]
    public Guid UniqueId { get; set; }

    public string Text { get; set; }

    public string AuthorEmail { get; set; }

    public string AuthorName { get; set; }

    [Editable(false)]
    public string AuthorHash { get; set; }

    [Editable(false)]
    public string DisplayDate
    {
      get
      {
        return CreatedDate.ToString("MMMM dd, yyyy");
      }
    }
  }
}
