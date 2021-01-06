using System.IO;

namespace Supdate.Business
{
  public interface ILogoManager
  {
    string Save(string fileName, Stream fileToBeUploaded);
  }
}
