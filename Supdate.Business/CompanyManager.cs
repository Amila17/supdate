using System;
using System.Collections.Generic;
using System.Linq;
using Supdate.Data;
using Supdate.Model;
using Supdate.Model.Exceptions;
using Supdate.Util;

namespace Supdate.Business
{
  public class CompanyManager : Manager<Company>, ICompanyManager
  {
    private const string InviteCode = "INVITE_CODE";
    private const string InviteEmail = "INVITE_EMAIL";
    private const string SelectAtLeastOneReportingArea = "You must select at least one Reporting Area";
    private readonly ICompanyRepository _companyRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IGenericEmailManager _genericEmailManager;

    public CompanyManager(ICompanyRepository companyRepository, IReportRepository reportRepository, IGenericEmailManager genericEmailManager)
      : base(companyRepository)
    {
      _companyRepository = companyRepository;
      _reportRepository = reportRepository;
      _genericEmailManager = genericEmailManager;
    }

    public LiteUser GetOwner(int companyId)
    {
      return _companyRepository.GetOwner(companyId);
    }

    public void AddUser(int companyId, int userId, bool isOwner)
    {
      _companyRepository.AddUser(companyId, userId, isOwner);
    }

    public void AddTeamMember(LiteUser teamMember, string welcomeMessage, string addedByEmail, string registerUrl)
    {
      if (teamMember.AccessibleAreaIds == null || teamMember.AccessibleAreaIds.Count() < 1)
      {
        throw new BusinessException(SelectAtLeastOneReportingArea);
      }
      var status = _companyRepository.AddTeamMember(teamMember);
      var thisCompany = _companyRepository.Get(teamMember.CompanyId);
      TextReplacements textReplacements;
      string subject;

      switch (status)
      {
        case 0:
          throw new BusinessException("That email address is already in use by your team");

        case 1:
          // existing user added, notify
          subject = string.Format("{0} - Access Granted", thisCompany.Name);

          textReplacements = new TextReplacements
                                 {
                                   Subject = subject,
                                   CompanyName = thisCompany.Name,
                                   WelcomeMessage = welcomeMessage,
                                   OwnerEmail = addedByEmail,
                                   ToEmail = teamMember.Email,
                                   RecipientEmail = teamMember.Email
                                 };

          _genericEmailManager.SendFromTemplate(teamMember.Email, subject, TextTemplate.TeamGrantAccessEmail, textReplacements);
          break;

        case 2:
          // new user: create invite and send registration link
          var invite = _companyRepository.AddTeamMemberInvite(teamMember);
          var registerLink = registerUrl.Replace(InviteCode, Uri.EscapeDataString(invite.UniqueId.ToString()));
          registerLink = registerLink.Replace(InviteEmail, Uri.EscapeDataString(teamMember.Email));
          subject = string.Format("{0} - Invitation", thisCompany.Name);

          textReplacements = new TextReplacements
                             {
                               Subject = subject,
                               CompanyName = thisCompany.Name,
                               WelcomeMessage = welcomeMessage,
                               OwnerEmail = addedByEmail,
                               ToEmail = teamMember.Email,
                               RecipientEmail = teamMember.Email,
                               RegisterLink = new Uri(new Uri(ConfigUtil.BaseAppUrl), registerLink).AbsoluteUri
                             };
          _genericEmailManager.SendFromTemplate(teamMember.Email, subject, TextTemplate.TeamInvitationEmail, textReplacements);
          break;
      }
    }

    public CompanyTeamMemberInvite GetTeamMemberInvite(Guid inviteGuid)
    {
      return _companyRepository.GetTeamMemberInvite(inviteGuid);
    }

    public LiteUser GetTeamMember(int companyId, Guid userGuid)
    {
      return _companyRepository.GetTeamMember(companyId, userGuid);
    }

    public LiteUser SaveTeamMember(LiteUser teamMember)
    {
      if (teamMember.AccessibleAreaIds == null || teamMember.AccessibleAreaIds.Count() < 1)
      {
        throw new BusinessException(SelectAtLeastOneReportingArea);
      }
      return _companyRepository.SaveTeamMember(teamMember);
    }

    public void AcceptTeamMemberInvite(CompanyTeamMemberInvite invite)
    {
      if (invite.ResultantUserId != null)
      {
        _companyRepository.AcceptTeamMemberInvite(invite.ResultantUserId.Value, invite.UniqueId.ToString());
      }
    }

    public void RemoveTeamMember(int companyId, Guid userGuid)
    {
      _companyRepository.RemoveTeamMember(companyId, userGuid);
    }

    public void RemoveUser(int companyId, int userId)
    {
      _companyRepository.RemoveUser(companyId, userId);
    }

    public IEnumerable<UserCompany> GetUserCompanies(int userId, bool? ownedCompanies)
    {
      var companies = _companyRepository.GetUserCompanies(userId, ownedCompanies).ToArray();

      foreach (var c in companies)
      {
        if (c.CanViewReports)
        {
          c.Permalinks = _reportRepository.GetReportPermalinks(c.Id);
        }
        else
        {
          c.Permalinks = new List<ReportPermalink>();
        }
      }

      return companies;
    }

    public IEnumerable<Company> GetUserLapsedCompanies(int userId)
    {
      return _companyRepository.GetUserLapsedCompanies(userId);
    }
    public IEnumerable<LiteUser> GetCompanyTeamMembers(int companyId)
    {
      return _companyRepository.GetCompanyTeamMembers(companyId);
    }

    public IEnumerable<CompanyTeamMemberInvite> GetCompanyTeamMemberInvites(int companyId)
    {
      return _companyRepository.GetCompanyTeamMemberInvites(companyId);
    }

    public bool DeleteInvite(int companyId, int? inviteId = null, Guid? uniqueId = null)
    {
      return _companyRepository.DeleteInvite(companyId, inviteId, uniqueId);
    }

    public void UpdateStats(int companyId)
    {
      _companyRepository.UpdateStats(companyId);
    }
  }
}
