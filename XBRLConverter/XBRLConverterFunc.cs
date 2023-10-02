using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.IO;
using System.Web.Http;

namespace XBRLConverter
{
    public static class XBRLConverterFunc
    {
        [FunctionName("XBRLConverterFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "xbrlconverter")] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            try
            {
                log.LogInformation("Getting the template");
                var template = TemplateOperation.GetTemplate(context);
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var requestXml = new XmlDocument();
                requestXml.LoadXml(requestBody);
                if (requestBody == null)
                {
                    return new BadRequestObjectResult("Request body cannot be empty");
                }
                TemplateOperation.ReplaceTemplateData(requestXml, template, log);

                // prepare response
                var outputPath = $"{Guid.NewGuid()}.xbrl";
                SaveToXhtmlFile.Save(template, outputPath, log);
                string xhtmlContent = File.ReadAllText(outputPath);
                var response = new ContentResult
                {
                    Content = xhtmlContent,
                    ContentType = "application/xhtml+xml", 
                    StatusCode = 200
                };

                return response;

            } catch (Exception ex)
            {
                log.LogError("Cannot generate XBRL file", ex);
                return new InternalServerErrorResult();
            }
            
        }
    }
}
