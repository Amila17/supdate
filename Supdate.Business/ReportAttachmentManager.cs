using System;
using System.IO;
using Supdate.Data;
using Supdate.Data.Base;
using Supdate.Util;
using Supdate.Model;
using System.Linq;

namespace Supdate.Business
{
  public class ReportAttachmentManager : IReportAttachmentManager
  {
    private readonly ICloudStorage _cloudStorage;
    private readonly ICrudRepository<ReportAttachment> _reportAttachmentRepository;
    private readonly IReportRepository _reportRepository;

    public ReportAttachmentManager(ICloudStorage cloudStorage, ICrudRepository<ReportAttachment> reportAttachmentRepository, IReportRepository reportRepository)
    {
      _cloudStorage = cloudStorage;
      _reportAttachmentRepository = reportAttachmentRepository;
      _reportRepository = reportRepository;
    }

    public ReportAttachment Create(ReportAttachment model, Stream fileToBeUploaded)
    {
      var containerName = ConfigUtil.ReportAttachmentStorageContainerName;
      var reportGuid = _reportRepository.GetReportGuid(model.CompanyId, model.ReportId);
      var blobName = (model.AreaId.HasValue) ? string.Format("{0}/{1}/{2}", reportGuid, model.AreaId, model.FileName) : string.Format("{0}/{1}", reportGuid, model.FileName);

      model.FilePath = _cloudStorage.SaveBlob(containerName, blobName, fileToBeUploaded);

      var newReportAttachment = _reportAttachmentRepository.Create(model);

      return newReportAttachment;
    }

    public ReportAttachment GetByUniqueId(int companyId, Guid attachmentId)
    {
      var attachment =
        _reportAttachmentRepository.GetList(new { CompanyId = companyId, UniqueId = attachmentId }).FirstOrDefault();

      return attachment;
    }

    public bool Delete(ReportAttachment model)
    {
      var containerName = ConfigUtil.ReportAttachmentStorageContainerName;
      var reportGuid = _reportRepository.GetReportGuid(model.CompanyId, model.ReportId);
      var blobName = (model.AreaId.HasValue) ? string.Format("{0}/{1}/{2}", reportGuid, model.AreaId, model.FileName) : string.Format("{0}/{1}", reportGuid, model.FileName);

      _cloudStorage.DeleteBlob(containerName, blobName);
      _reportAttachmentRepository.Delete(model.Id);

      return true;
    }

    public bool IsFilenameUnique(ReportAttachment reportAttachment)
    {
      var existingFile = _reportAttachmentRepository.GetList(
          new
          {
            CompanyId = reportAttachment.CompanyId,
            ReportId = reportAttachment.ReportId,
            FileName = reportAttachment.FileName,
            AreaId = reportAttachment.AreaId
          }
        ).FirstOrDefault();

      return existingFile == null;
    }
  }
}
