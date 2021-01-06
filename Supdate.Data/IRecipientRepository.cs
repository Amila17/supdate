using System;
using System.Collections.Generic;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public interface IRecipientRepository : ICrudRepository<Recipient>
  {
    IEnumerable<Recipient> GetReportRecipients(int companyId, DateTime reportDate);
    Recipient GetRecipient(int companyId, Guid recipientGuid);
    void Delete(int companyId, Guid uniqueId);
  }
}
