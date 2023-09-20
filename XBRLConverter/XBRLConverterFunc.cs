using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace XBRLConverter
{
    public static class XBRLConverterFunc
    {
        [FunctionName("XBRLConverterFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "{templateId}")] HttpRequest req, string templateId,
            ILogger log)
        {
            // the request body needs to contains all the key similar to the tag in the xml template
            log.LogInformation("Getting the template");
            var template = GetTemplate(templateId);
            if (template == null)
            {
                return new BadRequestObjectResult($"Template {templateId} is not supported");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (requestBody == null)
            {
                return new BadRequestObjectResult("Request body cannot be empty");
            }
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestBody);

            foreach (XmlElement item in template.ChildNodes[0].ChildNodes)
            {
                if (!data.ContainsKey(item.Name)) {
                    return new BadRequestObjectResult($"Input data lack keys {item.Name}");
                }
                var inputData = data[item.Name.ToString()];
                item.InnerXml = inputData;
            }
            log.LogInformation("Returning the parsed XML");
            return new ContentResult() { Content = template.InnerXml, ContentType = "text/xml" };
        }

        public static XmlDocument GetTemplate(string templateId)
        {
            XmlDocument doc = new XmlDocument();
            string fileName = String.Empty;
            switch (templateId.ToLower())
            {
                case "testtemplate":
                    fileName = "testTemplate";
                    break;
                default:
                    break;
            }
            if (String.IsNullOrEmpty(fileName)) return null;
            doc.Load($"./Templates/{fileName}.xml");
            return doc;
        }
    }
}
