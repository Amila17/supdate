using System;
using System.Collections.Generic;
using Supdate.Model;
using Supdate.Model.Admin;

namespace Supdate.Business.Admin
{
  public interface IAdminManager
  {
    void PushToMailChimp(int userId, string oldEmail = null);

    void SendWelcomeEmail(string emailAddress);

    bool IsUserAdmin(int userId);

    IEnumerable<UserEx> GetRegisteredUsers(int page, int records, int sortOption);

    IEnumerable<CompanyEx> GetRegisteredCompanies(int page, int records);

    RegistrationStatistics GetRegistrationStatistics(int windowInDays);

    UserEx GetUserDetails(Guid uniqueId, bool populateCompanies = false);

    IEnumerable<ReportEmailDetails> GetRecentReports(int records);

    bool SaveUserConfirmationUrl(int userId, string url);

    bool DeleteUserConfirmationUrl(int userId);

    void IncrementConfirmationReminderCount(int userId);

    /// <summary>
    /// Gets the list of registered users that have not activated their emails.
    /// </summary>
    IEnumerable<UserConfirmation> GetUnconfirmedUsers(int registeredBeforeHours);

    CompanyEx GetCompanyDetails(Guid uniqueId);

    IEnumerable<LiteUser> GetNewUsers(int registeredBeforeHours);

    IEnumerable<int> GetModifiedCompanyList();

    void UpdateFeedbackEmailSent(int userId);

    void StoreUtmInfo(UtmInfo utmInfo);

    bool DeleteMemeber(int memberId);

    SearchResults Search(string query);

    bool DeleteCompany(int id, string name);
  }
}
