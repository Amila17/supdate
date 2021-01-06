namespace Supdate.Data
{
  public interface ITermsAndConditionsRepository
  {
    void AcceptTermsAndConditions(int userId);

    bool HasPendingTermsAndConditions(int userId);
  }
}
