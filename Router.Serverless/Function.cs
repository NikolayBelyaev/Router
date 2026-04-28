using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Router.Core.ConfigurationStorage;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace Router.Serverless
{
    public class Functions
    {
        private const string Token = "Token";

        private const string AWSAccessKeyId = "AWSAccessKeyId";
        private const string AWSSecret = "AWSSecret";
        private const string DynamoDbTablePrefix = "router-";
        
        private readonly IRoutingConfigurationStorage _storage = new DynamoDbRoutingConfigurationStorage(
            AWSAccessKeyId,
            AWSSecret,
            RegionEndpoint.EUCentral1,
            DynamoDbTablePrefix
        );
        
        public Functions()
        {
        }

        public APIGatewayProxyResponse GetServerAddress(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return RouterApi.GetServerAddress(request, _storage).Result;
        }
        
        public APIGatewayProxyResponse GetRoutingConfiguration(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return RouterApi.Authorize(Token, request.Headers) 
                   ?? RouterApi.GetRoutingConfiguration(request, _storage).Result;
        }
        
        public APIGatewayProxyResponse GetRouteTargetConfiguration(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return RouterApi.Authorize(Token, request.Headers) 
                   ?? RouterApi.GetRouteTargetConfiguration(request, _storage).Result;
        }
    }
}