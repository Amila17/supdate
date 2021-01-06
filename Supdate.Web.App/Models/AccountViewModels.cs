using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class ExternalLoginConfirmationViewModel
  {
    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; }
  }

  public class ExternalLoginListViewModel
  {
    public string ReturnUrl { get; set; }
  }

  public class SendCodeViewModel
  {
    public string SelectedProvider { get; set; }
    public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    public string ReturnUrl { get; set; }
    public bool RememberMe { get; set; }
  }

  public class VerifyCodeViewModel
  {
    [Required]
    public string Provider { get; set; }

    [Required]
    [Display(Name = "Code")]
    public string Code { get; set; }
    public string ReturnUrl { get; set; }

    [Display(Name = "Remember this browser?")]
    public bool RememberBrowser { get; set; }

    public bool RememberMe { get; set; }
  }

  public class ForgotViewModel
  {
    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; }
  }

  public class LoginViewModel
  {
    [Required(ErrorMessage = "Please enter your email address.")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Please enter your password.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
  }

  public class RegisterViewModel
  {
    [Required(ErrorMessage = "Please enter your email address.")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Please provide a password.")]
    [StringLength(100, ErrorMessage = "This must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Doesn't match Password.")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Enter your company name.")]
    [Display(Name = "Company Name")]
    [StringLength(300, ErrorMessage = "Must be less than 300 characters.")]
    public string CompanyName { get; set; }

    [Display(Name = "Terms and Conditions")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the Terms and Conditions and Privacy Policy")]
    public bool TermsAndConditions { get; set; } = false;

    [Display(Name = "Twitter Handle")]
    [StringLength(100, ErrorMessage = "The {0} must be less than {2} characters long.")]
    public string TwitterHandle { get; set; }

    public CompanyTeamMemberInvite Invite { get; set; }
  }

  public class ResetPasswordViewModel
  {
    [Required(ErrorMessage = "Please enter your email address.")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Please supply a password.")]
    [StringLength(100, ErrorMessage = "This must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Doesn't match Password.")]
    public string ConfirmPassword { get; set; }

    public string Code { get; set; }
  }

  public class ForgotPasswordViewModel
  {
    [Required(ErrorMessage = "Please enter your email address.")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }
  }

  public class CompanyAccessViewModel
  {
    public IEnumerable<Company> LapsedCompanies { get; set; }
    public IEnumerable<UserCompany> OtherCompanies { get; set; }
  }
}
