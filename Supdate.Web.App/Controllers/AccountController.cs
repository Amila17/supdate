using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RazorEngine;
using RazorEngine.Templating;
using StackExchange.Exceptional;
using Supdate.Business;
using Supdate.Business.Admin;
using Supdate.Model;
using Supdate.Model.Admin;
using Supdate.Util;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("account")]
  [UnhandledExceptionFilter(View = "Error")]
  [Authorize]
  public class AccountController : Controller
  {
    private readonly ICompanyManager _companyManager;
    private readonly IAdminManager _adminManager;
    private AppSignInManager _signInManager;
    private AppUserManager _userManager;
    private readonly IGenericEmailManager _genericEmailManager;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly ITermsAndConditionsManager _termsAndConditionsManager;
    private const string DefaultReportTitle = "Shareholder Update";

    public AccountController(ICompanyManager companyManager, IAdminManager adminManager, IGenericEmailManager genericEmailManager, ISubscriptionManager subscriptionManager, ITermsAndConditionsManager termsAndConditionsManager)
    {
      _companyManager = companyManager;
      _adminManager = adminManager;
      _genericEmailManager = genericEmailManager;
      _subscriptionManager = subscriptionManager;
      _termsAndConditionsManager = termsAndConditionsManager;
    }

    public AppSignInManager SignInManager
    {
      get
      {
        return _signInManager ?? HttpContext.GetOwinContext().Get<AppSignInManager>();
      }
    }

    public AppUserManager UserManager
    {
      get
      {
        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
      }
    }

    // GET: /Account/Login
    [AllowAnonymous]
    public ActionResult Login(string returnUrl, string email = "")
    {
      ViewBag.ReturnUrl = returnUrl;
      LoginViewModel model = new LoginViewModel
      {
        Email = email
      };

      return View(model);
    }

    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
    {
      ViewBag.ReturnUrl = returnUrl;
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      if (Request.IsAuthenticated)
      {
        return RedirectToLocal(returnUrl);
      }

      // Require the user to have a confirmed email before they can log on.
      var allowPassOnEmailVerfication = false;
      var user = await UserManager.FindByNameAsync(model.Email);

      if (user != null)
      {
        if (!string.IsNullOrWhiteSpace(user.UnConfirmedEmail))
        {
          allowPassOnEmailVerfication = true;
        }

        if (!await UserManager.IsEmailConfirmedAsync(user.Id) && !allowPassOnEmailVerfication)
        {
          string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, user.UniqueId, "Confirm your account - Resent");

          this.SetNotificationMessage(NotificationType.Error, string.Format("You must have a confirmed email address to log in.{0}{0}We've resent you an email to confirm your address.", Environment.NewLine));
          return View();
        }
      }

      // This doesn't count login failures towards account lockout
      // To enable password failures to trigger account lockout, change to shouldLockout: true
      var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
      switch (result)
      {
        case SignInStatus.Success:
          if (user != null)
          {
            user.LoginCount = user.LoginCount + 1;
            user.LastLogin = DateTime.UtcNow;
            UserManager.Update(user);

            // Create a trial subscription if one doesn't exist for all companies of this user. This is necessary for creating subscriptions for all the users that
            // were in the system before subscriptions were introduced.
            _subscriptionManager.CreateMissingSubscriptions(user.Id);
          }

          return RedirectToLocal(returnUrl);

        case SignInStatus.LockedOut:
          return View("Lockout");

        case SignInStatus.RequiresVerification:
          // allow user to log in if its the new email address that's awaiting verification
          return allowPassOnEmailVerfication ? RedirectToLocal(returnUrl) : RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        case SignInStatus.Failure:
        default:
          this.SetNotificationMessage(NotificationType.Error, "Invalid login attempt.");
          return View(model);
      }
    }

    [HttpPost]
    public ActionResult ChangeCompany(Guid companyUniqueId)
    {
      UserManager.ChangeDefaultCompany(HttpContext.GetOwinContext(), companyUniqueId, User.Identity.GetUserId<int>());
      return Json(new { success = true });
    }

    //
    // GET: /Account/VerifyCode
    [AllowAnonymous]
    public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
    {
      // Require that the user has already logged in via username/password or external login
      if (!await SignInManager.HasBeenVerifiedAsync())
      {
        return View("Error");
      }

      return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
    }

    // POST: /Account/VerifyCode
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // The following code protects for brute force attacks against the two factor codes.
      // If a user enters incorrect codes for a specified amount of time then the user account
      // will be locked out for a specified amount of time.
      // You can configure the account lockout settings in IdentityConfig
      var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
      switch (result)
      {
        case SignInStatus.Success:
          return RedirectToLocal(model.ReturnUrl);

        case SignInStatus.LockedOut:
          return View("Lockout");

        case SignInStatus.Failure:
        default:
          this.SetNotificationMessage(NotificationType.Error, "Invalid code.");
          return View(model);
      }
    }

    // GET: /Account/Register
    [AllowAnonymous]
    public ActionResult Register(Guid? inviteCode, string email)
    {
      if (!inviteCode.HasValue || inviteCode == Guid.Empty)
      {
        return View();
      }

      var invite = _companyManager.GetTeamMemberInvite(inviteCode.Value);
      if (invite == null)
      {
        return View("InviteUnavailable");
      }

      if (invite.ResultantUserId.HasValue)
      {
        return View("InviteUnavailable");
      }

      var m = new RegisterViewModel { Invite = invite };
      m.CompanyName = m.Invite.CompanyName;

      return View(m);
    }

    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        var lastMonth = DateTime.Now.AddMonths(-1);
        var existingUser = await UserManager.FindByEmailAsync(model.Email);
        CompanyTeamMemberInvite invite = null;
        bool hasInvite = false;

        if (existingUser != null)
        {
          this.SetNotificationMessage(NotificationType.Error, string.Format("Email {0} is already taken.", model.Email));
          return View(model);
        }

        if (!model.TermsAndConditions)
        {
          this.SetNotificationMessage(NotificationType.Error, "You must accept the Terms and Conditions and Privacy Policy.");
          return View(model);
        }

        // Check for invite
        if (model.Invite != null)
        {
          invite = _companyManager.GetTeamMemberInvite(model.Invite.UniqueId);

          if (invite == null)
          {
            return View("InviteUnavailable");
          }

          if (invite.ResultantUserId.HasValue)
          {
            this.SetNotificationMessage(NotificationType.Error, "There is already an account creating using this invitation. Please login to start using the application.");
            return View(model);
          }

          hasInvite = true;
        }

        IdentityResult result;
        AppUser registeredUser;

        using (var scope = new TransactionScope())
        {
          // Create new user.
          registeredUser = new AppUser
          {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName
          };
          result = await UserManager.CreateAsync(registeredUser, model.Password);

          if (result.Succeeded)
          {
            // Mark the latest terms and conditions as accepted
            _termsAndConditionsManager.AcceptTermsAndConditions(registeredUser.Id);

            // Check if the user has been invited.
            if (hasInvite)
            {
              invite.ResultantUserId = registeredUser.Id;
              _companyManager.AcceptTeamMemberInvite(invite);
            }
            else
            {
              // Create a new company.
              var newCompany = _companyManager.Create(new Company
              {
                Name = model.CompanyName,
                StartMonth = lastMonth,
                ReportTitle = DefaultReportTitle,
                TwitterHandle = model.TwitterHandle
              });

              // Associate the user to the company.
              _companyManager.AddUser(newCompany.Id, registeredUser.Id, true);

              // Create a trial subscription
              _subscriptionManager.CreateTrialSubscription(newCompany.Id);
            }
          }

          // Complete the scope.
          scope.Complete();
        }

        // Send the email confirmation email.
        if (result.Succeeded)
        {
          // store UTM info
          var utmInfo = GetUtmInfo();
          if (utmInfo != null)
          {
            utmInfo.UserId = registeredUser.Id;
            _adminManager.StoreUtmInfo(utmInfo);
          }

          var callbackUrl = await SendEmailConfirmationTokenAsync(registeredUser.Id, registeredUser.UniqueId, "Confirm your account");
          ViewBag.Link = callbackUrl;

          return Redirect("http://supdate.com/confirm-email");

        }

        AddErrors(result);
      }

      // If we got this far, something failed, redisplay form

      return View(model);
    }

    [HttpGet]
    public ActionResult TermsAndConditions()
    {
      var userId = User.Identity.GetUserId<int>();
      var acceptedTermsAndConditions =_termsAndConditionsManager.HasPendingTermsAndConditions(userId);

      return View("_TermsAndConditions", new TermsAndConditionsViewModel { UserId = userId, TermsAndConditions = acceptedTermsAndConditions });
    }

    [HttpPost]
    public ActionResult TermsAndConditions(int userId, bool termsAndConditions)
    {
      if (termsAndConditions)
      {
        _termsAndConditionsManager.AcceptTermsAndConditions(userId);

        return Json(new { success = true });
      }

      return Json(new { success = false });
    }

    private UtmInfo GetUtmInfo()
    {
      var utmStrings = new[] { "Source", "Medium", "Term", "Content", "Campaign" };
      var hasData = false;
      var utmInfo = new UtmInfo();

      foreach (var s in utmStrings)
      {
        var httpCookie = Request.Cookies.Get(string.Format("utm_{0}", s.ToLower()));
        if (httpCookie != null)
        {
          var propertyInfo = utmInfo.GetType().GetProperty(s);
          propertyInfo.SetValue(utmInfo, httpCookie.Value);
          hasData = true;
        }
      }

      return hasData ? utmInfo : null;
    }

    // GET: /Account/ConfirmEmail
    [AllowAnonymous]
    public async Task<ActionResult> ConfirmEmail(Guid u, string code)
    {
      if (u == Guid.Empty || code == null)
      {
        return View("Error");
      }

      var user = await UserManager.GetUserFromUniqueId(HttpContext.GetOwinContext(), u);

      var result = await UserManager.ConfirmEmailAsync(user.Id, code);
      if (result.Succeeded)
      {
        try
        {
          // Remove the confirmation url from the database.
          _adminManager.DeleteUserConfirmationUrl(user.Id);

          // We have an unconfirmed email address implying the email address was changed.
          if (!string.IsNullOrWhiteSpace(user.UnConfirmedEmail))
          {
            // it's an email change
            var appUser = UserManager.FindById(user.Id);
            var oldEmail = appUser.Email;
            appUser.Email = user.UnConfirmedEmail;
            appUser.UserName = user.UnConfirmedEmail;
            appUser.UnConfirmedEmail = string.Empty;

            // Update the database.
            UserManager.Update(appUser);
            _adminManager.PushToMailChimp(user.Id, oldEmail);

            this.SetNotificationMessage(NotificationType.StickySuccess, string.Format("Thanks, you new email address has been confirmed.{0}You should use this address when you next log in.", Environment.NewLine));
            return RedirectToAction("Index", "Home", new { email = user.Email });
          }
          else
          {
            if (user.IsCompanyAdmin)
            {
              // Push the user to MailChimp and send welcome email
              _adminManager.PushToMailChimp(user.Id);
              _adminManager.SendWelcomeEmail(user.Email);
            }

            this.SetNotificationMessage(NotificationType.StickySuccess, string.Format("Yay! Email confirmed.{0}Now you can log in and get started.", Environment.NewLine));
          }

        }
        catch (Exception exception)
        {
          ErrorStore.LogException(exception, System.Web.HttpContext.Current);
        }

      }
      else
      {
        await SendEmailConfirmationTokenAsync(user.Id, u, "Confirm your account - Resent");
        this.SetNotificationMessage(NotificationType.Warning, string.Format("Your activation link has expired.{0}{0}We've resent an email to confirm your address.", Environment.NewLine));
      }

      return RedirectToAction("Login", "Account", new { email = user.Email });
    }

    // GET: /Account/ForgotPassword
    [AllowAnonymous]
    public ActionResult ForgotPassword()
    {
      return View(new ForgotPasswordViewModel { Email = Request.QueryString["Email"] });
    }

    // POST: /Account/ForgotPassword
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await UserManager.FindByNameAsync(model.Email);
        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        {
          // Don't reveal that the user does not exist or is not confirmed
          this.SetNotificationMessage(NotificationType.StickySuccess, "We've sent you an email so that you can reset your password.");
          return RedirectToAction("Login", "Account");
        }

        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        // Send an email with this link
        var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        var callbackUrl = Url.Action("ResetPassword", "Account", new { code = code }, protocol: Request.Url.Scheme);
        callbackUrl = callbackUrl + "&email=" + Uri.EscapeDataString(user.Email);
        var textReplacement = new TextReplacements
        {
          BaseUrl = ConfigUtil.BaseAppUrl,
          RecipientEmail = user.Email,
          ResetPasswordLink = callbackUrl
        };

        _genericEmailManager.SendFromTemplate(user.Email, "Supdate Password Reset", TextTemplate.ForgotPasswordEmail, textReplacement);

        this.SetNotificationMessage(NotificationType.StickySuccess, "We've sent you an email so that you can reset your password.");
        return RedirectToAction("Login", "Account");
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    // GET: /Account/ResetPassword
    [AllowAnonymous]
    public ActionResult ResetPassword(string code)
    {
      ViewBag.Email = Request.QueryString["Email"];
      return code == null ? View("Error") : View();
    }

    // POST: /Account/ResetPassword
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      var user = await UserManager.FindByNameAsync(model.Email);
      if (user == null)
      {
        // Don't reveal that the user does not exist
        this.SetNotificationMessage(NotificationType.StickySuccess, "Your password has been reset. Please login now.");
        return RedirectToAction("Login", "Account");
      }
      var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
      if (result.Succeeded)
      {
        this.SetNotificationMessage(NotificationType.StickySuccess, "Your password has been reset. Please login now.");
        return RedirectToAction("Login", "Account");
      }

      AddErrors(result);
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> ChangeEmail(ChangeEmailModel model)
    {
      if (!ModelState.IsValid)
      {
        return RedirectToAction("EditProfile", "Manage");
      }

      var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
      if (user != null)
      {
        //doing a quick swap so we can send the appropriate confirmation email
        user.UnConfirmedEmail = user.Email;
        user.Email = model.EmailAddress;
        user.EmailConfirmed = false;
        var result = await UserManager.UpdateAsync(user);

        if (result.Succeeded)
        {

          string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, user.UniqueId, "Confirm your new email address", true);

          var tempUnconfirmed = user.Email;
          user.Email = user.UnConfirmedEmail;
          user.UnConfirmedEmail = tempUnconfirmed;
          result = await UserManager.UpdateAsync(user);

          return Json(new { success = true, email = model.EmailAddress });
        }
        else
        {
          return Json(new { success = false, error = "That email address is already in use" });
        }
      }

      return Json(new { success = false });
    }


    // POST: /Account/ExternalLogin
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult ExternalLogin(string provider, string returnUrl)
    {
      // Request a redirect to the external login provider
      return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
    }

    // GET: /Account/SendCode
    [AllowAnonymous]
    public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
    {
      var userId = await SignInManager.GetVerifiedUserIdAsync();
      if (userId == 0)
      {
        return View("Error");
      }
      var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
      var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
      return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
    }

    // POST: /Account/SendCode
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SendCode(SendCodeViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      // Generate the token and send it
      if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
      {
        return View("Error");
      }
      return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
    }

    // GET: /Account/ExternalLoginCallback
    [AllowAnonymous]
    public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
    {
      var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
      if (loginInfo == null)
      {
        return RedirectToAction("Login");
      }

      // Sign in the user with this external login provider if the user already has a login
      var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
      switch (result)
      {
        case SignInStatus.Success:
          return RedirectToLocal(returnUrl);

        case SignInStatus.LockedOut:
          return View("Lockout");

        case SignInStatus.RequiresVerification:
          return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });

        case SignInStatus.Failure:
        default:
          // If the user does not have an account, then prompt the user to create an account
          ViewBag.ReturnUrl = returnUrl;
          ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
          return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
      }
    }

    // POST: /Account/ExternalLoginConfirmation
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
    {
      if (User.Identity.IsAuthenticated)
      {
        return RedirectToAction("Index", "Manage");
      }

      if (ModelState.IsValid)
      {
        // Get the information about the user from the external login provider
        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
          return View("ExternalLoginFailure");
        }
        var user = new AppUser { UserName = model.Email, Email = model.Email };
        var result = await UserManager.CreateAsync(user);
        if (result.Succeeded)
        {
          result = await UserManager.AddLoginAsync(user.Id, info.Login);
          if (result.Succeeded)
          {
            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            return RedirectToLocal(returnUrl);
          }
        }
        AddErrors(result);
      }

      ViewBag.ReturnUrl = returnUrl;
      return View(model);
    }

    // POST: /Account/LogOff
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult LogOff()
    {
      Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

      return RedirectToAction("Index", "Home");
    }

    //
    // GET: /Account/ExternalLoginFailure
    [AllowAnonymous]
    public ActionResult ExternalLoginFailure()
    {
      return View();
    }

    public ActionResult NoCompanyToAccess()
    {
      int userId = User.Identity.GetUserId<int>();
      var lapsedCompanies = _companyManager.GetUserLapsedCompanies(userId);

      if (!lapsedCompanies.Any())
      {
        // There are no lapsed companies, ask the company to create a new company.
        return RedirectToAction("CreateCompany");
      }

      return View("CompanyAccess", new CompanyAccessViewModel
      {
        LapsedCompanies = lapsedCompanies,
        OtherCompanies = _companyManager.GetUserCompanies(userId, null)
      });
    }

    public ActionResult CreateCompany()
    {
      return View();
    }

    [HttpPost]
    public ActionResult CreateCompany(string companyName)
    {
      if (string.IsNullOrWhiteSpace(companyName))
      {
        return View();
      }

      using (var scope = new TransactionScope())
      {
        // Create a new company.
        var newCompany = _companyManager.Create(new Company
        {
          Name = companyName,
          StartMonth = DateTime.Now.AddMonths(-1),
          TwitterHandle = string.Empty
        });

        // Associate the user to the company.
        _companyManager.AddUser(newCompany.Id, User.Identity.GetUserId<int>(), true);

        // Complete the scope.
        scope.Complete();
      }
      this.SetNotificationMessage(NotificationType.Success, "Company successfully created!");
      return RedirectToAction("Index", "Settings");
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_userManager != null)
        {
          _userManager.Dispose();
          _userManager = null;
        }

        if (_signInManager != null)
        {
          _signInManager.Dispose();
          _signInManager = null;
        }
      }

      base.Dispose(disposing);
    }

    #region Helpers
    // Used for XSRF protection when adding external logins
    private const string XsrfKey = "XsrfId";

    private IAuthenticationManager AuthenticationManager
    {
      get
      {
        return HttpContext.GetOwinContext().Authentication;
      }
    }

    private void AddErrors(IdentityResult result)
    {
      var errorMessage = result.Errors.Aggregate(string.Empty, (current, error) => current + (error));
      this.SetNotificationMessage(NotificationType.Error, errorMessage);
    }

    private ActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }

      return RedirectToAction("Index", "Home");
    }

    private async Task<string> SendEmailConfirmationTokenAsync(int userId, Guid uniqueId, string subject, bool isEmailChange = false)
    {
      // Generate the confirmation token.
      var code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);

      // Form the confirmation url and generate the email body.
      var callbackUrl = Url.Action("ConfirmEmail", "Account", new { u = uniqueId, code = code }, protocol: Request.Url.Scheme);
      var emailBody = isEmailChange ? GetEmailBodyForConfirmEmailChange(callbackUrl) : GetEmailBodyForConfirmEmail(callbackUrl);

      // Store the confirmation url in the database (to help sending reminder emails - using a job).
      _adminManager.SaveUserConfirmationUrl(userId, callbackUrl);

      // Send the confirmation email.
      await UserManager.SendEmailAsync(userId, subject, emailBody);

      return callbackUrl;
    }

    private async Task<string> StoreEmailConfirmationLink(int userId, Guid uniqueId)
    {
      // Generate the confirmation token.
      var code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);

      // Form the confirmation url and generate the email body.
      var callbackUrl = Url.Action("ConfirmEmail", "Account", new { u = uniqueId, code = code }, protocol: Request.Url.Scheme);

      // Store the confirmation url in the database (to help sending reminder emails - using a job).
      _adminManager.SaveUserConfirmationUrl(userId, callbackUrl);

      return callbackUrl;
    }

    private string GetEmailBodyForConfirmEmail(string confirmationUrl)
    {
      var templatePath = Server.MapPath("~/Views/Templates/AccountConfirmation.cshtml");
      var template = System.IO.File.ReadAllText(templatePath);

      var model = new { ConfirmationUrl = confirmationUrl };
      var body = Engine.Razor.RunCompile(template, "confirmationEmailTemplate", null, model);

      return body;
    }

    private string GetEmailBodyForConfirmEmailChange(string confirmationUrl)
    {
      var templatePath = Server.MapPath("~/Views/Templates/EmailChangeConfirmation.cshtml");
      var template = System.IO.File.ReadAllText(templatePath);

      var model = new { ConfirmationUrl = confirmationUrl };
      var body = Engine.Razor.RunCompile(template, "EmailChangeConfirmation", null, model);

      return body;
    }

    internal class ChallengeResult : HttpUnauthorizedResult
    {
      public ChallengeResult(string provider, string redirectUri)
        : this(provider, redirectUri, 0)
      {
      }

      public ChallengeResult(string provider, string redirectUri, int userId)
      {
        LoginProvider = provider;
        RedirectUri = redirectUri;
        UserId = userId;
      }

      public string LoginProvider { get; set; }
      public string RedirectUri { get; set; }
      public int UserId { get; set; }

      public override void ExecuteResult(ControllerContext context)
      {
        var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
        if (UserId != 0)
        {
          properties.Dictionary[XsrfKey] = UserId.ToString();
        }
        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
      }
    }
    #endregion
  }
}
