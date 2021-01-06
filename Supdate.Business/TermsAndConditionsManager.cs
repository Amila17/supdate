using Supdate.Data;

namespace Supdate.Business
{
  public class TermsAndConditionsManager : ITermsAndConditionsManager
  {
    private readonly ITermsAndConditionsRepository _termsAndConditionsRepository;

    public TermsAndConditionsManager(ITermsAndConditionsRepository termsAndConditionsRepository)
    {
      _termsAndConditionsRepository = termsAndConditionsRepository;
    }

    public void AcceptTermsAndConditions(int userId)
    {
      _termsAndConditionsRepository.AcceptTermsAndConditions(userId);
    }

    public bool HasPendingTermsAndConditions(int userId)
    {
      return _termsAndConditionsRepository.HasPendingTermsAndConditions(userId);
    }
  }
}
