using System;
using System.IO;
using Supdate.Model;

namespace Supdate.Business
{
  public interface IReportAttachmentManager
  {
    ReportAttachment Create(ReportAttachment reportAttachment, Stream fileToBeUploaded);

    bool Delete(ReportAttachment reportAttachment);

    ReportAttachment GetByUniqueId(int companyId, Guid attachmentId);

    bool IsFilenameUnique(ReportAttachment reportAttachment);
  }
}
