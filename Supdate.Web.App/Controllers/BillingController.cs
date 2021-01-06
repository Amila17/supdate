using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using StackExchange.Exceptional;
using Supdate.Business;
using Supdate.Model;
using Supdate.Util;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("billing")]
  public class BillingController : AuthenticatedControllerBase
  {
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly ICompanyManager _companyManager;

    public BillingController(ISubscriptionManager subscriptionManager, ICompanyManager companyManager)
      : base(ownerAccessOnly: true)
    {
      _subscriptionManager = subscriptionManager;
      _companyManager = companyManager;
    }

    public ActionResult Index()
    {
      var subscription = _subscriptionManager.GetList(new { CompanyId }).FirstOrDefault();
      var company = _companyManager.Get(CompanyId);
      var owner = _companyManager.GetOwner(CompanyId);

      if (subscription == null)
      {
        // Create a trial subscription
        subscription = new Subscription { Status = SubscriptionStatus.Trialing };
        subscription.AddDays(ConfigUtil.DefaultTrialDuration);
        subscription.CompanyId = company.Id;
        _subscriptionManager.Create(subscription);
      }

      string stripeCardLabel = "Start Subscription";
      string stripeCardPostTarget = Url.Action("StartSubcription");

      if (subscription.StripeSubscriptionId != null)
      {
        stripeCardLabel = "Update Card Details";
        stripeCardPostTarget = Url.Action("UpdateCardDetails");
      }

      return View(new BillingViewModel
      {
        Subscription = subscription,
        Company = company,
        Owner = owner,
        StripeCardLabel = stripeCardLabel,
        StripeCardPostTarget = stripeCardPostTarget,
        HasValidSubscription = HasValidSubscription
      });
    }

    [Route("address")]
    public ActionResult BillingAddress()
    {
      var company = _companyManager.Get(CompanyId);
      if (string.IsNullOrWhiteSpace(company.BillingName))
      {
        company.BillingName = company.Name;
      }
      return View("_Billing_Address", company);
    }

    [HttpPost]
    [Route("address")]
    public ActionResult UpdateBillingAddress(Company model)
    {
      var company = _companyManager.Get(CompanyId);
      if (company.UniqueId != model.UniqueId)
      {
        this.SetNotificationMessage(NotificationType.Error, "Company mismatch. Try again.");
        return RedirectToAction("BillingAddress");
      }

      company.BillingName = model.BillingName;
      company.BillingAddress1 = model.BillingAddress1;
      company.BillingAddress2 = model.BillingAddress2;
      company.BillingCity = model.BillingCity;
      company.BillingPostCode = model.BillingPostCode;
      company.BillingCountry = model.BillingCountry;

      _companyManager.Update(company);

      // TODO update stripe
      this.SetNotificationMessage(NotificationType.Success, "Billing Address Updated.");

      return RedirectToAction("Index");
    }

    [Route("invoices")]
    public ActionResult ListInvoices()
    {
      var invoices = _subscriptionManager.ListInvoices(CompanyId);

      return View("_Billing_Invoices", invoices);
    }

    [Route("invoices/{invoiceId}")]
    public ActionResult ViewInvoice(string invoiceId)
    {
      var model = new InvoiceViewModel
      {
        Company = _companyManager.Get(CompanyId),
        StripeInvoice = _subscriptionManager.GetInvoice(invoiceId)
      };

      return View("invoice", model);
    }

    [HttpPost]
    [Route("start-subscription")]
    public ActionResult StartSubcription(string stripeToken)
    {
      try
      {
        var subscription = _subscriptionManager.StartSubscription(CompanyId, stripeToken);
        var company = _companyManager.Get(CompanyId);

        company.EnableCommenting = true;
        _companyManager.Update(company);

        return View("Billing_Started", subscription);
      }
      catch (Exception e)
      {
        this.SetNotificationMessage(NotificationType.Error, string.Format("An error occurred\n{0}\nPlease contact support.", e.Message));
        ErrorStore.LogException(e, System.Web.HttpContext.Current);

        return Redirect(Url.Action("Index"));
      }
    }

    [HttpPost]
    [Route("update-cards")]
    public ActionResult UpdateCardDetails(string stripeToken)
    {
      _subscriptionManager.UpdateCardDetails(CompanyId, stripeToken);
      this.SetNotificationMessage(NotificationType.Success, "Your card details have been updated.");

      return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("cancel-subscription")]
    public ActionResult CancelSubscription(string stripeToken)
    {
      var subscription = _subscriptionManager.CancelSubscription(CompanyId);

      return View("Billing_Cancelled", subscription);
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("stripe-webhook")]
    public ActionResult StripeWebhook()
    {
      var json = new StreamReader(Request.InputStream).ReadToEnd();

      return _subscriptionManager.ProcessWebhook(json)
        ? new HttpStatusCodeResult(HttpStatusCode.OK)
        : new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }


  }
}
