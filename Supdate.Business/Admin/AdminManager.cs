using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Supdate.Data;
using Supdate.Data.Admin;
using Supdate.Model;
using Supdate.Model.Admin;
using Supdate.Util;

namespace Supdate.Business.Admin
{
  public class AdminManager : IAdminManager
  {
    private readonly ICloudStorage _cloudStorage;
    private readonly IAdminRepository _adminRepository;
    private readonly ICompanyManager _companyManager;
    private readonly IGenericEmailManager _genericEmailManager;

    public AdminManager(ICloudStorage cloudStorage, IAdminRepository adminRepository,
      ICompanyManager companyManager, IGenericEmailManager genericEmailManager)
    {
      _cloudStorage = cloudStorage;
      _adminRepository = adminRepository;
      _companyManager = companyManager;
      _genericEmailManager = genericEmailManager;
    }

    public void PushToMailChimp(int userId, string oldEmail = null)
    {
      var marketingData = _adminRepository.GetMarketingData(userId);
      marketingData.OldEmail = oldEmail;

      // Get JSON string.
      var message = JsonConvert.SerializeObject(marketingData);

      // Queue the request.
      _cloudStorage.Enqueue(ConfigUtil.MailChimpQueueName, message);
    }

    public void SendWelcomeEmail(string emailAddress)
    {
      var textReplacement = new TextReplacements
                            {
                              BaseUrl = ConfigUtil.BaseAppUrl,
                              RecipientEmail = emailAddress
                            };
      _genericEmailManager.SendFromTemplate(emailAddress, "Welcome to Supdate", TextTemplate.WelcomeEmail, textReplacement);
    }

    public bool IsUserAdmin(int userId)
    {
      return _adminRepository.IsUserAdmin(userId);
    }

    public IEnumerable<UserEx> GetRegisteredUsers(int page, int records, int sortOption)
    {
      return _adminRepository.GetRegisteredUsers(page, records, sortOption);
    }

    public IEnumerable<CompanyEx> GetRegisteredCompanies(int page, int records)
    {
      return _adminRepository.GetRegisteredCompanies(page, records);
    }

    public RegistrationStatistics GetRegistrationStatistics(int windowInDays)
    {
      return _adminRepository.GetRegistrationStatistics(windowInDays);
    }

    public UserEx GetUserDetails(Guid uniqueId, bool populateCompanies = false)
    {
      var user = _adminRepository.GetUserDetails(uniqueId);

      if (populateCompanies)
      {
        user.OwnCompanies = _companyManager.GetUserCompanies(user.Id, true).ToArray();
        user.OtherCompanies = _companyManager.GetUserCompanies(user.Id, false).ToArray();
      }

      return user;
    }

    public IEnumerable<ReportEmailDetails> GetRecentReports(int records)
    {
      return _adminRepository.GetRecentReports(records);
    }

    public bool SaveUserConfirmationUrl(int userId, string url)
    {
      return _adminRepository.SaveUserConfirmationUrl(userId, url);
    }

    public bool DeleteUserConfirmationUrl(int userId)
    {
      return _adminRepository.DeleteUserConfirmationUrl(userId);
    }

    public void IncrementConfirmationReminderCount(int userId)
    {
      _adminRepository.IncrementConfirmationReminderCount(userId);
    }

    public IEnumerable<UserConfirmation> GetUnconfirmedUsers(int registeredBeforeHours)
    {
      return _adminRepository.GetUnconfirmedUsers(registeredBeforeHours);
    }

    public CompanyEx GetCompanyDetails(Guid uniqueId)
    {
      var company = _adminRepository.GetCompanyDetails(uniqueId);
      company.Owner = GetUserDetails(company.OwnerUniqueId);

      return company;
    }

    public IEnumerable<LiteUser> GetNewUsers(int registeredBeforeHours)
    {
      return _adminRepository.GetNewUsers(registeredBeforeHours);
    }

    public IEnumerable<int> GetModifiedCompanyList()
    {
      return _adminRepository.GetModifiedCompanyList();
    }

    public void UpdateFeedbackEmailSent(int userId)
    {
      _adminRepository.UpdateFeedbackEmailSent(userId);
    }

    public void StoreUtmInfo(UtmInfo utmInfo)
    {
      _adminRepository.StoreUtmInfo(utmInfo);
    }

    public bool DeleteMemeber(int memberId)
    {
      return _adminRepository.DeleteMember(memberId);
    }

    public SearchResults Search(string query)
    {
      return _adminRepository.Search(query);
    }

    public bool DeleteCompany(int id, string name)
    {
      return _adminRepository.DeleteCompany(id, name);
    }
  }
}
