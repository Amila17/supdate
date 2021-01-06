using System;
using System.Collections.Generic;
using Supdate.Model;
using Supdate.Model.Admin;

namespace Supdate.Data.Admin
{
  public interface IAdminRepository
  {
    bool IsUserAdmin(int userId);

    IEnumerable<UserEx> GetRegisteredUsers(int page, int records, int sortOption);

    IEnumerable<CompanyEx> GetRegisteredCompanies(int page, int records);

    RegistrationStatistics GetRegistrationStatistics(int windowInDays);

    bool SaveUserConfirmationUrl(int userId, string url);

    bool DeleteUserConfirmationUrl(int userId);

    void IncrementConfirmationReminderCount(int userId);

    IEnumerable<UserConfirmation> GetUnconfirmedUsers(int registeredBeforeHours);

    IEnumerable<LiteUser> GetNewUsers(int registeredBeforeHours);

    UserEx GetUserDetails(Guid uniqueId);

    IEnumerable<ReportEmailDetails> GetRecentReports(int records);

    CompanyEx GetCompanyDetails(Guid uniqueId);

    MarketingData GetMarketingData(int userId);

    void UpdateFeedbackEmailSent(int userId);

    IEnumerable<int> GetModifiedCompanyList();

    void StoreUtmInfo(UtmInfo utmInfo);

    bool DeleteMember(int memberId);

    SearchResults Search(string query);

    bool DeleteCompany(int id, string name);
  }
}
