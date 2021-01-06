using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using LumenWorks.Framework.IO.Csv;
using Supdate.Business;
using Supdate.Model;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("recipients")]
  public class RecipientsController : AuthenticatedControllerBase
  {
    private readonly IRecipientManager _recipientManager;

    public RecipientsController(IRecipientManager recipientManager)
      : base(ownerAccessOnly: true)
    {

      _recipientManager = recipientManager;
    }

    public ActionResult Index()
    {
      var recipientList = _recipientManager.GetList(new { CompanyId });

      return View(recipientList);
    }

    [Route("details/{uniqueId?}")]
    [HttpPost]
    public ActionResult Details(Recipient recipient, Guid uniqueId = default(Guid))
    {
      recipient.UniqueId = uniqueId;
      if (ModelState.IsValid)
      {
        recipient.CompanyId = CompanyId;
        if (_recipientManager.IsDuplicateEmail(recipient))
        {
          return Json(new { success = false, error = "You already have a recipient with this email address" });
        }

        if (recipient.UniqueId == Guid.Empty)
        {
          _recipientManager.Create(recipient);
        }
        else
        {
          _recipientManager.Update(recipient);
        }
        this.SetNotificationMessage(NotificationType.Success, "Recipient successfully saved.");
        return Json(new { success = true });
      }
      return Json(new { success = false });
    }

    [Route("details/{uniqueId}")]
    public ActionResult Details(Guid uniqueId)
    {
      var recipient = new Recipient();
      if (uniqueId != Guid.Empty)
      {
        recipient = _recipientManager.GetRecipient(CompanyId, uniqueId);
      }
      return View("_details", recipient);
    }

    [Route("delete")]
    public ActionResult Delete(Guid recipientId)
    {
      _recipientManager.Delete(CompanyId, recipientId);
      return Json(new { success = true });
    }

    public ActionResult UploadCsv()
    {
      return View("_uploadFile");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UploadCsv(HttpPostedFileBase csvFile)
    {
      if (ModelState.IsValid)
      {
        if (csvFile != null && csvFile.ContentLength > 0)
        {

          if (csvFile.FileName.EndsWith(".csv"))
          {
            var stream = csvFile.InputStream;
            var csvTable = new DataTable();

            using (var csvReader = new CsvReader(new StreamReader(stream), true))
            {
              csvTable.Load(csvReader);
            }

            if (csvTable.Rows.Count < 1)
            {
              ModelState.AddModelError("File", "This file only contains one row of data. There must be more than one row for the importer to be of any use.");

              return View("Upload");
            }

            return View("Upload", csvTable);
          }

          ModelState.AddModelError("File", "This file format is not supported");
          return View("Upload");
        }

        ModelState.AddModelError("File", "Please Upload Your file");
      }

      return View("Upload");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddMultiple(IEnumerable<Recipient> data, bool deleteExisting)
    {
      using (var scope = new TransactionScope())
      {
        // Delete existing recipients
        if (deleteExisting)
        {
          var existingRecipients = _recipientManager.GetList(new { CompanyId });
          foreach (var recipient in existingRecipients)
          {
            _recipientManager.Delete(recipient.Id);
          }
        }

        // Add new recipients
        foreach (var recipient in data)
        {
          recipient.CompanyId = CompanyId;
          _recipientManager.Create(recipient);
        }

        scope.Complete();
        this.SetNotificationMessage(NotificationType.Success, string.Format("{0} recipients successfully imported.", data.Count()));

        return RedirectToAction("Index");
      }
    }

  }
}
