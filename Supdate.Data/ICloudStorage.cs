using System.IO;

namespace Supdate.Data
{
  public interface ICloudStorage
  {
    string SaveBlob(string containerName, string blobName, Stream fileToBeUploaded);

    void DeleteBlob(string containerName, string blobName);

    void Enqueue(string queueName, string content);
  }
}
