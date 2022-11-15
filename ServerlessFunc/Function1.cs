using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ServerlessFunc
{
    public static class Function1
    {
        /* "FunctionName" attribute specifies the name of my function. 
         * Hence, method name: "Run" isn't particularly important.
         * The first parameter of the function which is of type HttpRequest has an HttpTrigger attribute on it.
         * This attribute indicates that this function is triggered by an HttpRequest and it contains all the
         * configuration for this type of trigger including AuthorizationLevel, allowed http methods, and Route
         * which can be customized. Setting "Route" to "null" means to use the default route for this function
         * based on its name.
         */
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";
            /* If I keep the debugger in the below line, I should see the response message */
            return new OkObjectResult(responseMessage);
        }
    }
}
