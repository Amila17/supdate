using System;
using System.CodeDom;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using RazorEngine;
using RazorEngine.Templating;
using Supdate.Model;

namespace Supdate.Business
{
  public class TemplateManager : ITemplateManager
  {
    private static string AssemblyDirectory
    {
      get
      {
        var codeBase = Assembly.GetExecutingAssembly().CodeBase;
        var uri = new UriBuilder(codeBase);
        var path = Uri.UnescapeDataString(uri.Path);

        return Path.GetDirectoryName(path);
      }
    }

    public string GetTemplateText(TextTemplate template)
    {
      var templateFileName = string.Empty;

      // Decide the template file name.
      switch (template)
      {
        case TextTemplate.WelcomeEmail:
          templateFileName = "WelcomeToSupdate.cshtml";
          break;

        case TextTemplate.TeamGrantAccessEmail:
          templateFileName = "TeamGrantAccess.cshtml";
          break;

        case TextTemplate.TeamInvitationEmail:
          templateFileName = "TeamInvitation.cshtml";
          break;

        case TextTemplate.ForgotPasswordEmail:
          templateFileName = "ForgotPassword.cshtml";
          break;

        case TextTemplate.ReportEmail:
          templateFileName = "ReportEmail.cshtml";
          break;

        case TextTemplate.ReportEmailTitleSnippet:
          templateFileName = "_ReportEmail-Title.cshtml";
          break;

        case TextTemplate.ReportEmailCompanyNameSnippet:
          templateFileName = "_ReportEmail-CompanyName.cshtml";
          break;

        case TextTemplate.ReportEmailButtonSnippet:
          templateFileName = "_ReportEmail-Button.cshtml";
          break;

        case TextTemplate.ReportEmailSummarySnippet:
          templateFileName = "_ReportEmail-Summary.cshtml";
          break;

        case TextTemplate.ReportViewedNotificationEmail:
          templateFileName = "ReportViewedNotification.cshtml";
          break;

        case TextTemplate.ReportCommentNotificationEmail:
          templateFileName = "ReportCommentNotification.cshtml";
          break;

        case TextTemplate.ReportEmailDiscussion:
          templateFileName = "_ReportEmail-Discussion.cshtml";
          break;

        case TextTemplate.ReportEmailPreviewBanner:
          templateFileName = "_ReportEmail-PreviewBanner.cshtml";
          break;
      }

      // Form the absolute path of the template file.
      var templatePath = Path.Combine(AssemblyDirectory, "Templates", templateFileName);

      // Read the template text and compile the template and get the output text.
      return File.ReadAllText(templatePath);
    }

    public string Compile(TextTemplate template, TextReplacements replacements)
    {
      var templateText = GetTemplateText(template);

      return Compile(templateText, replacements);
    }

    public string Compile(string templateText, TextReplacements replacements)
    {
      var body = Engine.Razor.RunCompile(templateText, GetMd5Hash(templateText), null, replacements);

      return body;
    }

    private static string GetMd5Hash(string input)
    {
      var md5 = MD5.Create();
      var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
      var hash = md5.ComputeHash(inputBytes);
      var sb = new StringBuilder();
      foreach (byte t in hash)
      {
        sb.Append(t.ToString("X2"));
      }

      return sb.ToString();
    }
  }
}
