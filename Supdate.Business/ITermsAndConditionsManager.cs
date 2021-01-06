namespace Supdate.Business
{
  public interface ITermsAndConditionsManager
  {
    void AcceptTermsAndConditions(int userId);

    bool HasPendingTermsAndConditions(int userId);
  }
}
