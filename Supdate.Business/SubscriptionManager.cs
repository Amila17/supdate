using System;
using System.Collections.Generic;
using System.Linq;
using Supdate.Data;
using Supdate.Model;
using Supdate.Util;
using Stripe;
using Supdate.Model.Exceptions;

namespace Supdate.Business
{
  public class SubscriptionManager : Manager<Subscription>, ISubscriptionManager
  {
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICompanyManager _companyManager;

    public SubscriptionManager(ISubscriptionRepository subscriptionRepository, ICompanyManager companyManager)
      : base(subscriptionRepository)
    {
      _subscriptionRepository = subscriptionRepository;
      _companyManager = companyManager;
      StripeConfiguration.SetApiKey(ConfigUtil.StripeApiKey);
    }

    public void CreateMissingSubscriptions(int userId)
    {
      var userCompanies = _companyManager.GetUserCompanies(userId, null);

      foreach (var userCompany in userCompanies)
      {
        // Create a trial subscription if it doesn't exist.
        CreateTrialSubscription(userCompany.Id);
      }
    }

    public void CreateTrialSubscription(int companyId)
    {
      // Check if this company has a subscription - type of subscription doesn't matter.
      if (GetSubscription(companyId) == null)
      {
        // Create a trial subscription
        var subscription = new Subscription { Status = SubscriptionStatus.Trialing };
        subscription.AddDays(ConfigUtil.DefaultTrialDuration);
        subscription.CompanyId = companyId;

        Create(subscription);
      }
    }

    public Subscription StartSubscription(int companyId, string stripeTokenId)
    {
      var subscription = GetSubscription(companyId);
      var company = _companyManager.Get(subscription.CompanyId);
      var owner = _companyManager.GetOwner(subscription.CompanyId);
      var taxPercent = 20; // TODO deal with EU customers where VAT is NA
      StripeCustomer stripeCustomer;

      var metaData = new Dictionary<string, string> { { "CompanyId", company.Id.ToString() } };

      // Get details of card from the token
      var stripeTokenService = new StripeTokenService();
      var stripeToken = stripeTokenService.Get(stripeTokenId);
      if (stripeToken.StripeCard.Country == "US")
      {
        metaData.Add("VAT", "US Customer, 0%");
        taxPercent = 0;
      }

      if (subscription.StripeCustomerId == null)
      {
        // Create customer and subscribe to plan
        var stripeCustomerOptions = new StripeCustomerCreateOptions
                                    {
                                      Email = owner.Email,
                                      Description = string.Format("{0} ({1})", company.Name, owner.Email),
                                      SourceToken = stripeTokenId,
                                      PlanId = ConfigUtil.StripePlanId,
                                      TaxPercent = taxPercent,
                                      Quantity = 1,
                                      Metadata = metaData
                                    };

        if (subscription.ExpiryDate >= DateTime.UtcNow)
        {
          stripeCustomerOptions.TrialEnd = subscription.ExpiryDate;
        }

        var customerService = new StripeCustomerService();
        stripeCustomer = customerService.Create(stripeCustomerOptions);

        var stripeSubscription = stripeCustomer.StripeSubscriptionList.Data[0];
        subscription.StripeCustomerId = stripeCustomer.Id;
        subscription.StripeSubscriptionId = stripeSubscription.Id;
      }
      else
      {
        // Customer already exists (probably cancelled previous subscription)

        // Ensure we're not creating a duplicate subscription
        var customerService = new StripeCustomerService();
        stripeCustomer = customerService.Get(subscription.StripeCustomerId);
        var subscriptionCount = stripeCustomer.StripeSubscriptionList.TotalCount;

        if (subscriptionCount > 0)
        {
          throw new BusinessException("An orphaned subscription already exists");
        }

        // Update the stripe customer details.
        var stripeCustomerOptions = new StripeCustomerUpdateOptions { Description = string.Format("{0} ({1})", company.Name, owner.Email), SourceToken = stripeTokenId, Metadata = metaData };
        stripeCustomer = customerService.Update(stripeCustomer.Id, stripeCustomerOptions);

        // Create a new subscription.
        var stripeSubscriptionCreateOptions = new StripeSubscriptionCreateOptions { TaxPercent = taxPercent, Quantity = 1 };

        // Set the subscription trial end date if trial period is not yet expired.
        if (subscription.ExpiryDate >= DateTime.UtcNow)
        {
          stripeSubscriptionCreateOptions.TrialEnd = subscription.ExpiryDate;
        }

        // Create the Subscription
        var subscriptionService = new StripeSubscriptionService();
        var stripeSubscription = subscriptionService.Create(subscription.StripeCustomerId, ConfigUtil.StripePlanId, stripeSubscriptionCreateOptions);

        subscription.StripeSubscriptionId = stripeSubscription.Id;
      }

      Update(subscription);

      return subscription;
    }

    public Subscription CancelSubscription(int companyId)
    {
      var subscription = GetSubscription(companyId);
      var subscriptionService = new StripeSubscriptionService();

      subscriptionService.Cancel(subscription.StripeCustomerId, subscription.StripeSubscriptionId); // optional cancelAtPeriodEnd flag

      return subscription;
    }

    public void UpdateCardDetails(int companyId, string stripeTokenId)
    {
      var subscription = GetSubscription(companyId);
      var customerService = new StripeCustomerService();
      var stripeCustomerUpdateOptions = new StripeCustomerUpdateOptions { SourceToken = stripeTokenId };

      customerService.Update(subscription.StripeCustomerId, stripeCustomerUpdateOptions);
    }

    public IEnumerable<StripeInvoice> ListInvoices(int companyId)
    {
      var subscription = GetSubscription(companyId);
      if (subscription.StripeCustomerId != null)
      {
        var invoiceService = new StripeInvoiceService();
        var stripeInvoiceListOptions = new StripeInvoiceListOptions { CustomerId = subscription.StripeCustomerId };

        return invoiceService.List(stripeInvoiceListOptions);
      }
      return new List<StripeInvoice>();
    }

    public StripeInvoice GetInvoice(string invoiceId)
    {
      var invoiceService = new StripeInvoiceService();
      return invoiceService.Get(invoiceId);
    }

    public bool ProcessWebhook(string json)
    {
      var stripeEvent = StripeEventUtility.ParseEvent(json);

      // All of the types available are listed in StripeEvents
      switch (stripeEvent.Type)
      {
        case StripeEvents.CustomerSubscriptionUpdated:
          StripeSubscription stripeSubscription = Mapper<StripeSubscription>.MapFromJson(stripeEvent.Data.Object.ToString());
          var subscription = GetSubscriptionFromStripeCustomerId(stripeSubscription.CustomerId);

          // Update expiry date
          if (stripeSubscription.CurrentPeriodEnd.HasValue)
          {
            subscription.ExpiryDate = stripeSubscription.CurrentPeriodEnd.Value;
          }

          // Update Subscription status
          subscription.Status = ToSubscriptionStatus(stripeSubscription.Status);

          Update(subscription);
          break;

        case StripeEvents.InvoicePaymentSucceeded:
          // An invoice was paid

          // Get the subscription id from the invoice
          var stripeInvoice = Mapper<StripeInvoice>.MapFromJson(stripeEvent.Data.Object.ToString());
          subscription = GetSubscriptionFromStripeCustomerId(stripeInvoice.CustomerId);

          // Get the subscription from Stripe
          StripeSubscriptionService stripeSubscriptionService = new StripeSubscriptionService();
          stripeSubscription = stripeSubscriptionService.Get(subscription.StripeCustomerId, subscription.StripeSubscriptionId);

          // Update expiry date
          if (stripeSubscription.CurrentPeriodEnd.HasValue)
          {
            subscription.ExpiryDate = stripeSubscription.CurrentPeriodEnd.Value;
          }

          // Update Subscription status
          subscription.Status = ToSubscriptionStatus(stripeSubscription.Status);

          Update(subscription);
          break;

        case StripeEvents.CustomerSubscriptionDeleted:
          stripeSubscription = Mapper<StripeSubscription>.MapFromJson(stripeEvent.Data.Object.ToString());
          subscription = GetSubscriptionFromStripeCustomerId(stripeSubscription.CustomerId);
          if (subscription != null)
          {
            subscription.Status = ToSubscriptionStatus(stripeSubscription.Status);
            subscription.StripeSubscriptionId = null;
            Update(subscription);
          }
          break;

        case StripeEvents.CustomerDeleted:
          StripeCustomer stripeCustomer = Mapper<StripeCustomer>.MapFromJson((stripeEvent.Data.Object.ToString()));
          subscription = GetSubscriptionFromStripeCustomerId(stripeCustomer.Id);
          if (subscription != null)
          {
            subscription.StripeCustomerId = null;
            Update(subscription);
          }
          break;
      }

      return true;
    }

    public Subscription GetSubscription(int companyId)
    {
      return _subscriptionRepository.GetList(new { companyId }).FirstOrDefault();
    }

    private Subscription GetSubscriptionFromStripeCustomerId(string stripeCustomerId)
    {
      return _subscriptionRepository.GetList(new { StripeCustomerId = stripeCustomerId }).FirstOrDefault();
    }

    private static SubscriptionStatus ToSubscriptionStatus(string stripeSubscriptionStatus)
    {
      switch (stripeSubscriptionStatus)
      {
        case StripeSubscriptionStatuses.Trialing:
          return SubscriptionStatus.Trialing;

        case StripeSubscriptionStatuses.Active:
          return SubscriptionStatus.Active;

        case StripeSubscriptionStatuses.Canceled:
          return SubscriptionStatus.Cancelled;

        case StripeSubscriptionStatuses.PastDue:
          return SubscriptionStatus.Active;

        case StripeSubscriptionStatuses.Unpaid:
          return SubscriptionStatus.Cancelled;
      }

      return SubscriptionStatus.Unknown;
    }
  }
}
