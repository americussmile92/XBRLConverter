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
                var response = new ContentResult
                {
                    Content = template.OuterXml,
                    ContentType = "application/xhtml+xml", // Set the content type to XHTML
                    StatusCode = 200 // Set the desired HTTP status code
                };
                return response;
            } catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new InternalServerErrorResult();
            }
            
        }
    }
}
