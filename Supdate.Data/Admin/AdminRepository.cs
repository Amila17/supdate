using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using StackExchange.Profiling.Helpers.Dapper;
using Supdate.Data.Base;
using Supdate.Model;
using Supdate.Model.Admin;

namespace Supdate.Data.Admin
{
  public class AdminRepository : RepositoryBase, IAdminRepository
  {
    private readonly ICrudRepository<UserConfirmation> _userConfirmationRepository;
    private readonly ICrudRepository<UtmInfo> _utmInfoRepository;

    public AdminRepository(ICrudRepository<UserConfirmation> userConfirmationRepository, ICrudRepository<UtmInfo> utmInfoRepository)
    {
      _userConfirmationRepository = userConfirmationRepository;
      _utmInfoRepository = utmInfoRepository;
    }

    public bool IsUserAdmin(int userId)
    {
      try
      {
        OpenConnection();

        // Gets all the recipients for the report along with the recipients to whom already an email sent
        var result = Connection.Query<int>("UserGetIsAdmin", new { userId }, commandType: CommandType.StoredProcedure);

        if (result != null)
        {
          return result.Any();
        }

        return false;
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<UserEx> GetRegisteredUsers(int page, int records, int sortOption)
    {
      try
      {
        OpenConnection();

        // Gets all the registered (by filtering on the server side).
        var results = Connection.Query<UserEx>("UsersGetRegistered", new { desiredPageNumber = page, numberOfRows = records, sortOption = sortOption }, commandType: CommandType.StoredProcedure);

        return results;
      }
      finally
      {
        CloseConnection();
      }
    }
    public IEnumerable<CompanyEx> GetRegisteredCompanies(int page, int records)
    {
      try
      {
        OpenConnection();

        // Gets all the registered (by filtering on the server side).
        var results = Connection.Query<CompanyEx>("CompaniesGetRegistered", new { desiredPageNumber = page, numberOfRows = records }, commandType: CommandType.StoredProcedure);
        return results;
      }
      finally
      {
        CloseConnection();
      }
    }

    public RegistrationStatistics GetRegistrationStatistics(int windowInDays)
    {
      try
      {
        OpenConnection();

        // Gets all the registration statistics for the number of days mentioned.
        var results = Connection.Query<RegistrationStatistics>("UsersGetRegistrationStats", commandType: CommandType.StoredProcedure, param: new { windowInDays });
        var stats = results.FirstOrDefault();

        if (stats != null)
        {
          stats.WindowInDays = windowInDays;
        }

        return stats;
      }
      finally
      {
        CloseConnection();
      }
    }

    public bool SaveUserConfirmationUrl(int userId, string url)
    {
      // Make sure there are no confirmation URLs already.
      if (!_userConfirmationRepository.GetList(new { userId }).Any())
      {
        // Save the confirmation url.
        var newModel = _userConfirmationRepository.Create(new UserConfirmation { UserId = userId, Url = url });
        return newModel.Id > 0;
      }

      return true;
    }

    public bool DeleteUserConfirmationUrl(int userId)
    {
      var unconfirmedUsers = _userConfirmationRepository.GetList(new { userId });

      // Delete all the instances of user confirmation records - ideally there should only be one record per user.
      return unconfirmedUsers.Aggregate(true, (current, user) => current & _userConfirmationRepository.Delete(user.Id));
    }

    public void IncrementConfirmationReminderCount(int userId)
    {
      try
      {
        OpenConnection();

        // Increments the reminder sent count
        Connection.Execute("UserIncrementConfirmationReminder", commandType: CommandType.StoredProcedure, param: new { userId });
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<UserConfirmation> GetUnconfirmedUsers(int registeredBeforeHours)
    {
      try
      {
        OpenConnection();

        // Gets all the unconfirmed users (users that have not confirmed their emails address)
        var results = Connection.Query<UserConfirmation>("UsersGetUnconfirmed", commandType: CommandType.StoredProcedure, param: new { registeredBeforeHours });

        return results;
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<LiteUser> GetNewUsers(int registeredBeforeHours)
    {
      try
      {
        OpenConnection();

        // Gets all the unconfirmed users (users that have not confirmed their emails address)
        var results = Connection.Query<LiteUser>("UsersGetNew", commandType: CommandType.StoredProcedure, param: new { registeredBeforeHours });

        return results;
      }
      finally
      {
        CloseConnection();
      }
    }

    public UserEx GetUserDetails(Guid uniqueId)
    {
      try
      {
        OpenConnection();

        // Gets the user specified by the email address
        var results = Connection.Query<UserEx>("UserGetDetailsByUniqueId", commandType: CommandType.StoredProcedure, param: new { uniqueId });
        var u = results.FirstOrDefault();

        return u;
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<ReportEmailDetails> GetRecentReports(int records)
    {
      try
      {
        OpenConnection();

        // Gets all the recent reports that have been emailed by users.
        var results = Connection.Query<ReportEmailDetails>("UsersGetRecentReports", new { recordCount = records }, commandType: CommandType.StoredProcedure);

        return results;
      }
      finally
      {
        CloseConnection();
      }
    }

    public CompanyEx GetCompanyDetails(Guid uniqueId)
    {
      try
      {
        OpenConnection();
        var results = Connection.QueryMultiple("CompanyGetForAdmin", new { guid = uniqueId }, commandType: CommandType.StoredProcedure);
        var companyEx = results.Read<CompanyEx>().FirstOrDefault();

        if (companyEx != null)
        {
          companyEx.TeamMembers = results.Read<UserEx>().ToList();
          companyEx.PendingInvites = results.Read<CompanyTeamMemberInvite>().ToList();

          return companyEx;
        }
      }
      finally
      {
        CloseConnection();
      }

      return null;
    }

    public MarketingData GetMarketingData(int userId)
    {
      try
      {
        OpenConnection();
        return Connection.Query<MarketingData>("UserGetMarketingData", new { uid = userId }, commandType: CommandType.StoredProcedure).SingleOrDefault();
      }
      finally
      {
        CloseConnection();
      }
    }

    public void UpdateFeedbackEmailSent(int userId)
    {
      try
      {
        OpenConnection();

        // Increments the reminder sent count
        Connection.Execute("UserUpdateFeedbackEmailSent", commandType: CommandType.StoredProcedure, param: new { userId });
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<int> GetModifiedCompanyList()
    {
      try
      {
        OpenConnection();
        var modifiedCompanyIdList = Connection.Query<int>("CompanyListGetModified", commandType: CommandType.StoredProcedure);

        return modifiedCompanyIdList;
      }
      finally
      {
        CloseConnection();
      }
    }

    public void StoreUtmInfo(UtmInfo utmInfo)
    {
      try
      {
        OpenConnection();
        _utmInfoRepository.Create(utmInfo);
      }
      finally
      {
        CloseConnection();
      }

    }

    public bool DeleteMember(int memberId)
    {
      try
      {
        OpenConnection();
        var retValue = Connection.Execute("UserDeleteById", commandType: CommandType.StoredProcedure, param: new { userId = memberId });

        return retValue > 0;
      }
      finally
      {
        CloseConnection();
      }
    }

    public SearchResults Search(string query)
    {
      var searchResults = new SearchResults();
      try
      {
        OpenConnection();
        var results = Connection.QueryMultiple("AdminSearch", commandType: CommandType.StoredProcedure, param: new { query });
        searchResults.Users = results.Read<LiteUser>().ToList();
        searchResults.Companies = results.Read<Company>().ToList();
        return searchResults;
      }
      finally
      {
        CloseConnection();
      }
    }

    public bool DeleteCompany(int id, string name) 
    {
   
      try
      {
        OpenConnection();
        var result =  Connection.Query<int>("CompanyDelete", commandType: CommandType.StoredProcedure, param: new { id, name }).FirstOrDefault();
        return result > 0;
      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
