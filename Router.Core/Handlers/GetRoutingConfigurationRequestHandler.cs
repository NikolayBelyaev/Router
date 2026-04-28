using System.Threading.Tasks;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers.Abstraction;
using Router.Core.Model;

namespace Router.Core.Handlers
{
    public class GetRoutingConfigurationRequestHandler 
        : RequestHandler<GetRoutingConfigurationRequest, GetRoutingConfigurationResponse, GetRoutingConfigurationResponse.ReturnCode>
    {
        protected override GetRoutingConfigurationResponse.ReturnCode Ok => GetRoutingConfigurationResponse.ReturnCode.Ok;
        protected override GetRoutingConfigurationResponse.ReturnCode BadRequest => GetRoutingConfigurationResponse.ReturnCode.BadRequest;
        protected override GetRoutingConfigurationResponse.ReturnCode InternalServerError => GetRoutingConfigurationResponse.ReturnCode.InternalServerError;
        
        protected override Task<GetRoutingConfigurationResponse.ReturnCode> Validate(
            GetRoutingConfigurationRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            return Task.FromResult(Ok);
        }

        protected override GetRoutingConfigurationResponse RespondWithError(GetRoutingConfigurationResponse.ReturnCode errorCode)
        {
            return new GetRoutingConfigurationResponse(errorCode, null);
        }

        protected override async Task<GetRoutingConfigurationResponse> ProcessValidRequest(
            GetRoutingConfigurationRequest request,
            IRoutingConfigurationStorage configurationStorage
        )
        {
            var configuration = await configurationStorage.GetRoutingConfiguration();
            return new GetRoutingConfigurationResponse(Ok, configuration);
        }
    }
}