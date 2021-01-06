using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class Discussion : ModelBase
  {
    public Discussion()
    {
      Comments = new List<Comment>();
    }
    [Editable(false)]
    public string Title { get; set; }

    [Editable(false)]
    public string DiscussionName
    {
      get
      {
        return string.Format("discussion-{0}-{1}-{2}", ReportGuId, TargetType, Target);
      }
    }

    [ReadOnly(true)]
    public Guid UniqueId { get; set; }

    public int CompanyId { get; set; }

    public Guid ReportGuId { get; set; }

    [Editable(false)]
    public DateTime ReportDate { get; set; }

    [Editable(false)]
    public int ReportId { get; set; }

    public DiscussionTargetType TargetType { get; set; }

    public Guid Target { get; set; }

    [Editable(false)]
    public int CommentCount
    {
      get { return Comments.Count; }
    }

    public IList<Comment> Comments { get; set; }

  }
}
