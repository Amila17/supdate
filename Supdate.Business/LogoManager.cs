using System;
using System.IO;
using Supdate.Data;
using Supdate.Util;

namespace Supdate.Business
{
  public class LogoManager : ILogoManager
  {
    private readonly ICloudStorage _cloudStorage;

    public LogoManager(ICloudStorage cloudStorage)
    {
      _cloudStorage = cloudStorage;
    }

    public string Save(string fileName, Stream fileToBeUploaded)
    {
      var containerName = ConfigUtil.LogoStorageContainerName;
      var blobName = Guid.NewGuid() + Path.GetExtension(fileName);

      return _cloudStorage.SaveBlob(containerName, blobName, fileToBeUploaded);
    }
  }
}
