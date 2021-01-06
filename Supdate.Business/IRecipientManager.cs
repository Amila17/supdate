using System;
using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Business
{
  public interface IRecipientManager : IManager<Recipient>
  {
    IEnumerable<Recipient> GetReportRecipients(int companyId, DateTime reportDate);

    Recipient GetRecipient(int companyId, Guid uniqueId);

    bool IsDuplicateEmail(Recipient recipient);

    void Delete(int companyId, Guid uniqueId);
  }
}
