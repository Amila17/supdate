using System.Collections.Generic;
using Supdate.Model;
using Stripe;

namespace Supdate.Business
{
  public interface ISubscriptionManager : IManager<Subscription>
  {
    /// <summary>
    /// This method takes care of creating missing subscriptions that were in the system before subscriptions were introduced.
    /// </summary>
    /// <param name="userId">Subscription will be created for all the companies of this user</param>
    void CreateMissingSubscriptions(int userId);

    void CreateTrialSubscription(int companyId);

    Subscription StartSubscription(int companyId, string stripeTokenId);

    Subscription CancelSubscription(int companyId);

    void UpdateCardDetails(int companyId, string stripeTokenId);

    IEnumerable<StripeInvoice> ListInvoices(int companyId);

    StripeInvoice GetInvoice(string invoiceId);

    Subscription GetSubscription(int companyId);

    bool ProcessWebhook(string json);
  }
}
