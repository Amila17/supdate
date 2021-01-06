using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supdate.Data.Base;
using Supdate.Model;
using Supdate.Model.Exceptions;
using Supdate.Util;

namespace Supdate.Data
{
  public class CompanyRepository : CrudRepository<Company>, ICompanyRepository
  {
    public LiteUser GetOwner(int companyId)
    {
      try
      {
        OpenConnection();
        return Connection.Query<LiteUser>("CompanyGetOwner", param: new { CompanyId = companyId }, commandType: CommandType.StoredProcedure).First();
      }
      finally
      {
        CloseConnection();
      }
    }

    public bool AddUser(int companyId, int userId, bool isOwner)
    {
      try
      {
        OpenConnection();

        return Connection.Execute("CompanyAssociateUser", new { companyId, userId, isOwner }, commandType: CommandType.StoredProcedure) > 0;
      }
      finally
      {
        CloseConnection();
      }
    }

    public int AddTeamMember(LiteUser teamMember)
    {
      try
      {
        var areaData = ConversionUtil.IntArrayToDataTable(teamMember.AccessibleAreaIds);

        OpenConnection();
        return Connection.Query<int>("CompanyAddTeamMember", new { CompanyId = teamMember.CompanyId, emailAddress = teamMember.Email, accessibleAreaIds = areaData, canViewReports = teamMember.CanViewReports }, commandType: CommandType.StoredProcedure).First();
      }
      finally
      {
        CloseConnection();
      }
    }

    public LiteUser SaveTeamMember(LiteUser teamMember)
    {
      try
      {
        //only need to update area permissions
        var areaData = ConversionUtil.IntArrayToDataTable(teamMember.AccessibleAreaIds);
        OpenConnection();
        Connection.Execute("CompanyTeamMemberPermissionsUpdate", new { companyId = teamMember.CompanyId, userGuid = teamMember.UniqueId, accessibleAreaIds = areaData, canViewReports = teamMember.CanViewReports }, commandType: CommandType.StoredProcedure);

        return teamMember;
      }
      finally
      {
        CloseConnection();
      }
    }

    public LiteUser GetTeamMember(int companyId, Guid userGuid)
    {
      try
      {
        OpenConnection();
        var results = Connection.QueryMultiple("CompanyTeamMemberGet", new { companyId, userGuid }, commandType: CommandType.StoredProcedure);
        var teamMember = results.Read<LiteUser>().FirstOrDefault();
        if (teamMember != null)
        {
          teamMember.AccessibleAreaIds = results.Read<int>().ToArray();
        }

        return teamMember;
      }
      finally
      {
        CloseConnection();
      }
    }

    public void RemoveTeamMember(int companyId, Guid userGuid)
    {
      try
      {
        OpenConnection();
        Connection.Execute("CompanyTeamMemberRemove", new { userGuid, companyId }, commandType: CommandType.StoredProcedure);
      }
      finally
      {
        CloseConnection();
      }
    }

    public bool RemoveUser(int companyId, int userId)
    {
      try
      {
        OpenConnection();

        return Connection.Execute("CompanyDisassociateUser", new { companyId, userId }, commandType: CommandType.StoredProcedure) > 0;
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<UserCompany> GetUserCompanies(int userId, bool? ownedCompanies)
    {
      try
      {
        int? isOwner = null;

        if (ownedCompanies.HasValue)
        {
          isOwner = Convert.ToInt32(ownedCompanies);
        }

        OpenConnection();
        var results = Connection.Query<UserCompany>("CompanyGetListForUser", new { Userid = userId, isOwner }, commandType: CommandType.StoredProcedure);
        return results;
      }
      finally
      {
        CloseConnection();
      }
    }
    public IEnumerable<Company> GetUserLapsedCompanies(int userId)
    {
      try
      {
        OpenConnection();
        var results = Connection.Query<Company>("CompanyGetLapsedListForUser", new { Userid = userId }, commandType: CommandType.StoredProcedure);
        return results;
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<LiteUser> GetCompanyTeamMembers(int companyId)
    {
      try
      {
        OpenConnection();
        var results = Connection.QueryMultiple("CompanyListTeamMembers", new { CompanyId = companyId }, commandType: CommandType.StoredProcedure);
        var teamMembers = results.Read<LiteUser>();
        var permissionsArray = results.Read<int, int, Tuple<int, int>>(Tuple.Create, splitOn: "*").ToList();

        foreach (var teamMember in teamMembers)
        {
          teamMember.AccessibleAreaIds = permissionsArray.Where(p => p.Item1 == teamMember.Id).Select(p => p.Item2).ToArray();
        }

        return teamMembers;
      }
      finally
      {
        CloseConnection();
      }
    }

    public CompanyTeamMemberInvite AddTeamMemberInvite(LiteUser teamMember)
    {
      try
      {
        var areaData = ConversionUtil.IntArrayToDataTable(teamMember.AccessibleAreaIds);
        OpenConnection();

        var invite = Connection.Query<CompanyTeamMemberInvite>("CompanyInviteUser", param: new { EmailAddress = teamMember.Email, CompanyId = teamMember.CompanyId, accessibleAreaIds = areaData, canViewReports = teamMember.CanViewReports }, commandType: CommandType.StoredProcedure).First();
        if (invite == null)
        {
          throw new BusinessException("That email address is already registered");
        }

        return invite;
      }
      finally
      {
        CloseConnection();
      }
    }

    public CompanyTeamMemberInvite GetTeamMemberInvite(Guid inviteGuid)
    {
      try
      {
        OpenConnection();

        return Connection.Query<CompanyTeamMemberInvite>("CompanyGetTeamMemberInvite", new { inviteGuid }, commandType: CommandType.StoredProcedure).FirstOrDefault();
      }
      catch
      {
        return null;
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<CompanyTeamMemberInvite> GetCompanyTeamMemberInvites(int companyId)
    {
      try
      {
        OpenConnection();
        var results = Connection.QueryMultiple("CompanyListTeamMemberInvites", new { CompanyId = companyId }, commandType: CommandType.StoredProcedure);
        var invites = results.Read<CompanyTeamMemberInvite>();
        var permissionsArray = results.Read<int, int, Tuple<int, int>>(Tuple.Create, splitOn: "*").ToList();

        foreach (var invite in invites)
        {
          invite.AccessibleAreaIds = permissionsArray.Where(p => p.Item1 == invite.Id).Select(p => p.Item2).ToArray();
        }

        return invites;
      }
      finally
      {
        CloseConnection();
      }
    }

    public void AcceptTeamMemberInvite(int userId, string inviteGuid)
    {
      try
      {
        OpenConnection();
        Connection.Execute("CompanyTeamMemberAcceptInvite", new { inviteGuid, userId }, commandType: CommandType.StoredProcedure);
      }
      finally
      {
        CloseConnection();
      }
    }

    public bool DeleteInvite(int companyId, int? inviteId = null, Guid? inviteGuid = null)
    {
      try
      {
        OpenConnection();
        var result = Connection.Execute("CompanyTeamMemberInviteDelete", new { companyId, inviteId, inviteGuid }, commandType: CommandType.StoredProcedure);

        return result > 0;
      }
      finally
      {
        CloseConnection();
      }
    }

    public void UpdateStats(int companyId)
    {
      try
      {
        OpenConnection();
        Connection.Execute("CompanyUpdateStats", new { companyId }, commandType: CommandType.StoredProcedure);
      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
