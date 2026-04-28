using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers;

namespace Router.Serverless
{
    public static class RouterApi
    {
        public static async Task<APIGatewayProxyResponse> GetServerAddress(
            APIGatewayProxyRequest request,
            IRoutingConfigurationStorage storage
        )
        {
            var output = await new GetServerAddressRequestHandler().Handle(request.Body, storage);
            return Respond(output);
        }

        public static async Task<APIGatewayProxyResponse> GetRoutingConfiguration(
            APIGatewayProxyRequest request,
            IRoutingConfigurationStorage storage
        )
        {
            var output = await new GetRoutingConfigurationRequestHandler().Handle(request.Body, storage);
            return Respond(output);
        }

        public static async Task<APIGatewayProxyResponse> SetRoutingConfiguration(
            APIGatewayProxyRequest request,
            IRoutingConfigurationStorage storage
        )
        {
            var output = await new SetRoutingConfigurationRequestHandler().Handle(request.Body, storage);
            return Respond(output);
        }

        public static async Task<APIGatewayProxyResponse> GetRouteTargetConfiguration(
            APIGatewayProxyRequest request,
            IRoutingConfigurationStorage storage
        )
        {
            var output = await new GetRouteTargetConfigurationRequestHandler().Handle(request.Body, storage);
            return Respond(output);
        }

        public static async Task<APIGatewayProxyResponse> SetRouteTargetConfiguration(
            APIGatewayProxyRequest request,
            IRoutingConfigurationStorage storage
        )
        {
            var output = await new SetRouteTargetConfigurationRequestHandler().Handle(request.Body, storage);
            return Respond(output);
        }

        public static async Task<APIGatewayProxyResponse> SetMaintenanceOn(
            APIGatewayProxyRequest request,
            IRoutingConfigurationStorage storage
        )
        {
            var output = await new SetMaintenanceOnRequestHandler().Handle(request.Body, storage);
            return Respond(output);
        }

        public static async Task<APIGatewayProxyResponse> SetMaintenanceOff(
            APIGatewayProxyRequest request,
            IRoutingConfigurationStorage storage
        )
        {
            var output = await new SetMaintenanceOffRequestHandler().Handle(request.Body, storage);
            return Respond(output);
        }

        private static APIGatewayProxyResponse Respond<T>(T output)
        {
            return new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(output),
                IsBase64Encoded = false,
                StatusCode = 200
            };
        }

        public static APIGatewayProxyResponse Authorize(string token, IDictionary<string, string> headers)
        {

            var tokenExists = headers.TryGetValue("Authorization", out var headerToken);
            var valid = tokenExists && string.Equals(token, headerToken);

            return valid
                ? null
                : new APIGatewayProxyResponse
                {
                    Body = JsonConvert.SerializeObject(new {Code = "Unauthorized"}),
                    IsBase64Encoded = false,
                    StatusCode = 400
                };
        }
    }
}