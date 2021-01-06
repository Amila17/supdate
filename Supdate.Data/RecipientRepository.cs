using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public class RecipientRepository : CrudRepository<Recipient>, IRecipientRepository
  {
    public IEnumerable<Recipient> GetReportRecipients(int companyId, DateTime reportDate)
    {
      try
      {
        OpenConnection();

        // Gets all the recipients for the report along with the recipients to whom already an email sent
        var results = Connection.QueryMultiple("RecipientsGetByReportMonth", new { CompanyId = companyId, ReportDate = reportDate }, commandType: CommandType.StoredProcedure);
        var reportSummaryList = results.Read<Recipient>().ToList();

        return reportSummaryList;
      }
      finally
      {
        CloseConnection();
      }
    }

    public Recipient GetRecipient(int companyId, Guid recipientGuid)
    {
      try
      {
        OpenConnection();
        var results = Connection.QueryMultiple("RecipientGet", new { companyId, recipientGuid }, commandType: CommandType.StoredProcedure);
        var recipient = results.Read<Recipient>().FirstOrDefault();
        return recipient;
      }
      finally
      {
        CloseConnection();
      }
    }

    public override Recipient Update(Recipient model)
    {
      var modelCheck = GetRecipient(model.CompanyId, model.UniqueId);
      if (modelCheck.Id == model.Id)
      {
        return base.Update(model);
      }

      return model;
    }

    public void Delete(int companyId, Guid uniqueId)
    {
      var model = GetRecipient(companyId, uniqueId);
      Delete(model.Id);
    }
  }
}
