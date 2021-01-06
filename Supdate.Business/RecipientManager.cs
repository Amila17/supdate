using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Supdate.Data;
using Supdate.Model;

namespace Supdate.Business
{
  public class RecipientManager : Manager<Recipient>, IRecipientManager
  {
    private readonly IRecipientRepository _recipientRepository;

    public RecipientManager(IRecipientRepository recipientRepository)
      : base(recipientRepository)
    {
      _recipientRepository = recipientRepository;
    }

    public IEnumerable<Recipient> GetReportRecipients(int companyId, DateTime reportDate)
    {
      return _recipientRepository.GetReportRecipients(companyId, reportDate);
    }

    public Recipient GetRecipient(int companyId, Guid uniqueId)
    {
      return _recipientRepository.GetRecipient(companyId, uniqueId);
    }

    public bool IsDuplicateEmail(Recipient recipient)
    {
      var recipientList = GetList(new { recipient.CompanyId });
      var recipients = recipientList as Recipient[] ?? recipientList.ToArray();

      return (recipients.Any(r => (r.Email == recipient.Email && (recipient.Id == 0 || r.Id != recipient.Id))));
    }

    public override Recipient Create(Recipient model)
    {
      if (model.FirstName == "auto-gen-from-email")
      {
        var emailParts = model.Email.Split('@');
        var textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

        model.FirstName = textInfo.ToTitleCase(emailParts[0]);
      }

      return base.Create(model);
    }

    public void Delete(int companyId, Guid uniqueId)
    {
      _recipientRepository.Delete(companyId, uniqueId);
    }

  }
}
