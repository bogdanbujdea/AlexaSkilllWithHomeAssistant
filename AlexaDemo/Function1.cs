using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using HADotNet.Core;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AlexaDemo
{
    public static class Function1
    {
        [FunctionName("dot-net-skill")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string json = await req.ReadAsStringAsync();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);
            return await ProcessRequest(skillRequest);
        }

        private static async Task<IActionResult> ProcessRequest(SkillRequest skillRequest)
        {
            var requestType = skillRequest.GetRequestType();
            SkillResponse response = null;
            if (requestType == typeof(LaunchRequest))
            {
                response = ResponseBuilder.Tell("Welcome to dot net!");
                response.Response.ShouldEndSession = false;
            }
            else if (requestType == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;
                if (intentRequest.Intent.Name == "StartAC")
                {
                    await StartTheAC();
                    response = ResponseBuilder.Tell("Starting the AC");
                    response.Response.ShouldEndSession = false;
                }
            }
            else if (requestType == typeof(SessionEndedRequest))
            {
                response = ResponseBuilder.Tell("See you next time!");
                response.Response.ShouldEndSession = true;
            }
            return new OkObjectResult(response);
        }

        private static async Task StartTheAC()
        {
            ClientFactory.Initialize("http://localhost:8123", "<you Home Assistant API key here>");
            var serviceClient = ClientFactory.GetClient<ServiceClient>();
            var stateObjects = await serviceClient.CallService("climate", "set_hvac_mode", JsonConvert.SerializeObject(
                new
                {
                    entity_id = "climate.ecobee",
                    hvac_mode = "cool"
                }));
        }
    }
}
