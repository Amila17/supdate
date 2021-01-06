using Supdate.Model;

namespace Supdate.Business
{
  public interface ITemplateManager
  {
    string GetTemplateText(TextTemplate template);

    string Compile(TextTemplate template, TextReplacements replacements);

    string Compile(string templateText, TextReplacements replacements);
  }
}
