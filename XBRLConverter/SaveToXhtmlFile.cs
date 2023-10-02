using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XBRLConverter;

public class SaveToXhtmlFile
{
    public static void Save(XmlDocument document, string outputPath, ILogger log)
    {
        try
        {

            // Create a new XmlTextWriter to write the XHTML content to a file
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment
            };

            using (XmlWriter xhtmlWriter = XmlWriter.Create(outputPath, settings))
            {
                document.WriteContentTo(xhtmlWriter);
            }

            log.LogInformation("XML saved as XHTML successfully.");
        }
        catch (Exception ex)
        {
            log.LogError("Cannot save document", ex);
        }
    }
}
