using System.Configuration;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Supdate.Model.Exceptions;
using Supdate.Util;

namespace Supdate.Data
{
  public class AzureCloudStorage : ICloudStorage
  {
    public string SaveBlob(string containerName, string blobName, Stream fileToBeUploaded)
    {
      // Get the container reference.
      var storageAccount = GetStorageAccount();
      var blobClient = storageAccount.CreateCloudBlobClient();
      var blobContainer = blobClient.GetContainerReference(containerName);

      // Create a new container, if it does not exist
      blobContainer.CreateIfNotExists();

      // Get a page level blob reference.
      var blob = blobContainer.GetBlockBlobReference(blobName);

      // Upload the file.
      blob.UploadFromStream(fileToBeUploaded);

      return blob.Uri.AbsoluteUri;
    }

    public void DeleteBlob(string containerName, string blobName)
    {
      // Get the container reference.
      var storageAccount = GetStorageAccount();
      var blobClient = storageAccount.CreateCloudBlobClient();
      var blobContainer = blobClient.GetContainerReference(containerName);

      // Retrieve reference to a blob named "myblob.txt".
      var blockBlob = blobContainer.GetBlockBlobReference(blobName);

      // Delete the blob.
      blockBlob.Delete();
    }

    public void Enqueue(string queueName, string content)
    {
      // Get the container reference.
      var storageAccount = GetStorageAccount();
      var queueClient = storageAccount.CreateCloudQueueClient();
      var queue = queueClient.GetQueueReference(queueName);

      // Create the queue if it does not exist.
      queue.CreateIfNotExists();

      var message = new CloudQueueMessage(content);
      queue.AddMessage(message);
    }

    private static CloudStorageAccount GetStorageAccount()
    {
      var connectionStringName = ConfigUtil.StorageConnectionStringName;
      var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];

      if (connectionString == null)
      {
        throw new ConfigurationErrorsException(MessageConstants.InvalidConnectionConfiguration);
      }

      var storageAccount = CloudStorageAccount.Parse(connectionString.ConnectionString);
      return storageAccount;
    }
  }
}
