using System;
using System.Collections.Generic;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class ReportAttachmentList
  {
    public DateTime ReportDate { get; set; }
    public Guid? AreaUniqueId { get; set; }
    public IEnumerable<ReportAttachment> AttachmentList { get; set; }

  }
}
