using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Supdate.Business;
using Supdate.Model;
using Supdate.Util;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("companies")]
  public class CompanyController : AuthenticatedControllerBase
  {
    private readonly ICompanyManager _companyManager;
    private readonly ISubscriptionManager _subscriptionManager;

    public CompanyController(ICompanyManager companyManager, ISubscriptionManager subscriptionManager)
    {
      _companyManager = companyManager;
      _subscriptionManager = subscriptionManager;
    }

    // GET: Companies
    public ActionResult Index()
    {
      var ownCompanies = _companyManager.GetUserCompanies(User.Identity.GetUserId<int>(), true).ToArray();

      // remove all reports not yet completed
      foreach (var reportPermalinks in ownCompanies.Select(c => c.Permalinks as IList<ReportPermalink> ?? c.Permalinks.ToList()))
      {
        for (var i = reportPermalinks.Count - 1; i >= 0; i--)
        {
          if (reportPermalinks[i].Status != ReportStatus.Completed)
          {
            reportPermalinks.RemoveAt(i);
          }
        }
      }

      var m = new CompaniesViewModel
      {
        OwnCompanies = ownCompanies,
        OtherCompanies = _companyManager.GetUserCompanies(User.Identity.GetUserId<int>(), false).ToArray()
      };

      return View(m);
    }

    [HttpPost]
    public ActionResult AddCompany(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return Json(new { success = false });
      }

      using (var scope = new TransactionScope())
      {
        var lastMonth = DateTime.Now.AddMonths(-1);

        // Create a new company.
        var newCompany = _companyManager.Create(new Company
        {
          Name = name,
          StartMonth = lastMonth,
          TwitterHandle = string.Empty
        });

        // Associate the user to the company.
        _companyManager.AddUser(newCompany.Id, User.Identity.GetUserId<int>(), true);

        // Create a trial subscription
        var subscription = new Subscription { Status = SubscriptionStatus.Trialing };
        subscription.AddDays(ConfigUtil.DefaultTrialDuration);
        subscription.CompanyId = newCompany.Id;

        _subscriptionManager.Create(subscription);

        // Complete the scope.
        scope.Complete();
      }

      return Json(new { success = true });
    }
  }
}
