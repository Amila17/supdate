namespace Supdate.Business.DataSources
{
  public interface IOAuthClientManager
  {
    string Authorize(string returnUrl);

    void GetAccessToken(int companyId, string code, string returnUrl);
  }
}
