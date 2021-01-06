using System;
using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Business
{
  public interface ICompanyManager : IManager<Company>
  {
    LiteUser GetOwner(int companyId);

    void AddUser(int companyId, int userId, bool isOwner);
    void AddTeamMember(LiteUser teamMember, string welcomeMessage, string addedByEmail, string registrationUrl);
    CompanyTeamMemberInvite GetTeamMemberInvite(Guid inviteGuid);
    LiteUser GetTeamMember(int companyId, Guid userGuid);
    LiteUser SaveTeamMember(LiteUser teamMember);
    void AcceptTeamMemberInvite(CompanyTeamMemberInvite invite);
    void RemoveTeamMember(int companyId, Guid userGuid);
    void RemoveUser(int companyId, int userId);

    IEnumerable<UserCompany> GetUserCompanies(int userId, bool? ownedCompanies);
    IEnumerable<Company> GetUserLapsedCompanies(int userId);
    IEnumerable<LiteUser> GetCompanyTeamMembers(int companyId);
    IEnumerable<CompanyTeamMemberInvite> GetCompanyTeamMemberInvites(int companyId);
    bool DeleteInvite(int companyId, int? inviteId = null, Guid? uniqueId = null);

    void UpdateStats(int companyId);
  }
}
