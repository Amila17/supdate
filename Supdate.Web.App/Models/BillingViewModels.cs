using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Stripe;
using Supdate.Model;

namespace Supdate.Web.App.Models
{

  public class BillingViewModel
  {
    public Subscription Subscription { get; set; }
    public Company Company { get; set; }
    public LiteUser Owner { get; set; }

    public string StripeCardLabel { get; set; }
    public string StripeCardPostTarget { get; set; }
    public bool HasValidSubscription { get; set; }
  }

  public class InvoiceViewModel
  {
    public Company Company { get; set; }
    public StripeInvoice StripeInvoice { get; set; }
  }
}
