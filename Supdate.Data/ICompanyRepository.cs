using System;
using System.Collections.Generic;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public interface ICompanyRepository : ICrudRepository<Company>
  {
    LiteUser GetOwner(int companyId);
    bool AddUser(int companyId, int userId, bool isOwner);

    int AddTeamMember(LiteUser teamMember);
    void RemoveTeamMember(int companyId, Guid userGuid);
    LiteUser GetTeamMember(int companyId, Guid userGuid);
    LiteUser SaveTeamMember(LiteUser teamMember);
    bool RemoveUser(int companyId, int userId);

    IEnumerable<UserCompany> GetUserCompanies(int userId, bool? ownedCompanies);
    IEnumerable<Company> GetUserLapsedCompanies(int userId);

    IEnumerable<LiteUser> GetCompanyTeamMembers(int companyId);
    IEnumerable<CompanyTeamMemberInvite> GetCompanyTeamMemberInvites(int companyId);
    CompanyTeamMemberInvite AddTeamMemberInvite(LiteUser teamMember);
    CompanyTeamMemberInvite GetTeamMemberInvite(Guid inviteGuid);
    void AcceptTeamMemberInvite(int userId, string inviteGuid);
    bool DeleteInvite(int companyId, int? inviteId = null, Guid? inviteGuid = null);

    void UpdateStats(int companyId);
  }
}
