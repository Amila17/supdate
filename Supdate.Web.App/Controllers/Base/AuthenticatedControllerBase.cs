using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Supdate.Model;
using Supdate.Model.Exceptions;
using Supdate.Util;
using Supdate.Web.App.Filters;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers.Base
{
  [UnhandledExceptionFilterAttribute(View = "Error")]
  [Authorize]
  [ContextInitializerFilter]
  public abstract class AuthenticatedControllerBase : Controller
  {
    public bool IsSubscriptionActive { get; set; }

    protected string CompanyName;
    protected string LogoPath;

    /// <summary>
    /// This indicates user is a Supdate super-admin
    /// </summary>
    protected bool IsUserAdmin;

    /// <summary>
    /// Indicates the user is the owner.
    /// </summary>
    protected bool IsCompanyAdmin;

    protected SubscriptionStatus SubscriptionStatus;
    protected DateTime SubscriptionExpiryDate;
    protected bool HasValidSubscription;
    protected bool UnconvertedTrial;

    protected IList<Company> OtherCompanies;
    protected List<Area> MasterAreas;
    protected List<Goal> MasterGoals;
    protected List<Metric> MasterMetrics;
    protected CompanyMetadata CompanyMetadata;
    protected ListHelper ListHelper;
    protected LiteUser CurrentUser;

    private readonly bool _adminAccessOnly;
    private readonly bool _ownerAccessOnly;
    private int _companyId;
    private AppUserManager _userManager;

    protected AuthenticatedControllerBase(bool adminAccessOnly = false, bool ownerAccessOnly = false)
    {
      _adminAccessOnly = adminAccessOnly;
      _ownerAccessOnly = ownerAccessOnly;
      if (User == null)
      {
        return;
      }

      InitializeContext();
    }

    public bool InitializeContext()
    {
      if (_companyId > 0)
      {
        // Already initialised.
        return true;
      }

      // Get the user id from the identity and get the user attributes from the database.
      var userId = User.Identity.GetUserId<int>();
      CurrentUser = UserManager.GetUserAttributes(HttpContext.GetOwinContext(), userId);

      // Exit if the user is not found.
      if (CurrentUser == null)
      {
        return false;
      }

      // Initialize the fields.
      _companyId = CurrentUser.CompanyId;
      CompanyName = CurrentUser.Company;
      LogoPath = string.IsNullOrEmpty(CurrentUser.LogoPath) ? ConfigUtil.DefaultLogoUrl : CurrentUser.LogoPath;
      IsCompanyAdmin = CurrentUser.IsCompanyAdmin;
      IsUserAdmin = CurrentUser.IsAdmin;
      SubscriptionStatus = CurrentUser.SubscriptionStatus;
      SubscriptionExpiryDate = CurrentUser.SubscriptionExpiryDate;
      IsSubscriptionActive = ConfigUtil.FreeAccessToPremiumFeatures || CurrentUser.SubscriptionExpiryDate >= DateTime.UtcNow.Date;
      HasValidSubscription = CurrentUser.HasValidSubscription;
      OtherCompanies = CurrentUser.OtherCompanies;

      // Show trial banners if trial/cancelled and has no subscription set up
      UnconvertedTrial = ConfigUtil.FreeAccessToPremiumFeatures ? false :
                          ((SubscriptionStatus == SubscriptionStatus.Trialing || SubscriptionStatus == SubscriptionStatus.Cancelled)
                          && !HasValidSubscription  // No Stripe subscription Id
                          && IsSubscriptionActive);  // Expiry date not yet reached


      // Throw a 404 if admin access is required and user is not an admin.
      if (_adminAccessOnly && !IsUserAdmin)
      {
        throw new HttpException(404, "Invalid URL!");
      }

      // Throw a 403 if owner access is required and user is not an owner.
      if (_ownerAccessOnly && !IsCompanyAdmin)
      {
        throw new HttpException(403, "Forbidden");
      }

      ViewBag.CompanyName = CompanyName;
      ViewBag.CompanyId = _companyId;
      ViewBag.LogoPath = LogoPath;
      ViewBag.IsCompanyAdmin = IsCompanyAdmin;
      ViewBag.IsAdmin = IsUserAdmin;
      ViewBag.OtherCompanies = OtherCompanies;
      ViewBag.IsSubscriptionActive = IsSubscriptionActive;
      ViewBag.HasValidSubscription = HasValidSubscription;
      ViewBag.UnconvertedTrial = UnconvertedTrial;
      ViewBag.AcceptedLatestTerms = CurrentUser.AcceptedLatestTerms;

      return true;
    }

    protected AppUserManager UserManager
    {
      get
      {
        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
      }
      private set
      {
        _userManager = value;
      }
    }

    protected int CompanyId
    {
      get
      {
        if (_companyId == 0)
        {
          InitializeContext();
        }

        if (_companyId > 0)
        {
          return _companyId;
        }

        throw new BusinessException(MessageConstants.InvalidCompanyId);
      }
    }

    protected void InitializeMasterLists(LiteUser currentUser)
    {
      ListHelper.InitializeAreas(currentUser);
      ListHelper.InitializeGoals(currentUser);
      ListHelper.InitializeMetrics(currentUser);

      MasterAreas = ListHelper.GetAreas().ToList();
      MasterGoals = ListHelper.GetGoals().ToList();
      MasterMetrics = ListHelper.GetMetrics().ToList();

      CompanyMetadata = new CompanyMetadata
      {
        AreaCount = MasterAreas.Count,
        GoalCount = MasterGoals.Count,
        MetricCount = MasterMetrics.Count
      };

    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && UserManager != null)
      {
        UserManager.Dispose();
        UserManager = null;
      }

      base.Dispose(disposing);
    }
  }
}
