using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.IO;

namespace XBRLConverter;
public class TemplateOperation
{
    public static XmlDocument GetTemplate(ExecutionContext context)
    {
        var doc = new XmlDocument();
        string fileName = "xbrlTemplate";
        var templatePath = Path.Combine(context.FunctionAppDirectory, "Templates", $"{fileName}.xhtml");
        doc.Load(templatePath);
        return doc;
    }

    public static XmlNode ReplaceTemplateData(XmlDocument requestXml, XmlDocument templateDoc, ILogger log)
    {
        foreach (XmlNode item in requestXml.ChildNodes[0].ChildNodes)
        {
            switch (item.Name)
            {
                case "Firm":
                    ReplaceTemplateItem(templateDoc, "FIRM_PLACEHOLDER", item.InnerText, log);
                    break;
                case "OrganizationNumber":
                    ReplaceTemplateItem(templateDoc, "ORGANIZATION_PLACEHOLDER", item.InnerText, log);
                    break;
                default:
                    log.LogInformation($"Keyword {item.Name} in input data  has not been mapped in template");
                    break;
            }
        }
        return null;
    }
    public static void ReplaceTemplateItem(XmlDocument templateDoc, string searchString, string updatedValue, ILogger log)
    {
        log.LogInformation($"Replacing value {searchString} with value {updatedValue}");
        var item = GetTemplateNode(templateDoc, searchString);
        if (item != null)
        {
            item.Value = updatedValue;
        }
    }

    public static XmlNode GetTemplateNode(XmlNode documents, string searchString)
    {
        foreach (XmlNode item in documents.ChildNodes)
        {
            if (item.Value != null && item.Value == searchString)
            {
                return item;
            }
            if (item.ChildNodes.Count > 0)
            {
                var foundItem = GetTemplateNode(item, searchString);
                if (foundItem != null)
                {
                    return foundItem;
                }
            }
        }
        return null;
    }
}
