using System.Threading.Tasks;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers.Abstraction;
using Router.Core.Model;

namespace Router.Core.Handlers
{
    public class GetRouteTargetConfigurationRequestHandler
        : RequestHandler<GetRouteTargetConfigurationRequest, GetRouteTargetConfigurationResponse, GetRouteTargetConfigurationResponse.ReturnCode>
    {
        protected override GetRouteTargetConfigurationResponse.ReturnCode Ok => GetRouteTargetConfigurationResponse.ReturnCode.Ok;
        protected override GetRouteTargetConfigurationResponse.ReturnCode BadRequest => GetRouteTargetConfigurationResponse.ReturnCode.BadRequest;
        protected override GetRouteTargetConfigurationResponse.ReturnCode InternalServerError => GetRouteTargetConfigurationResponse.ReturnCode.InternalServerError;

        protected override Task<GetRouteTargetConfigurationResponse.ReturnCode> Validate(
            GetRouteTargetConfigurationRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            return Task.FromResult(GetRouteTargetConfigurationResponse.ReturnCode.Ok);
        }

        protected override GetRouteTargetConfigurationResponse RespondWithError(GetRouteTargetConfigurationResponse.ReturnCode errorCode)
        {
            return new GetRouteTargetConfigurationResponse(errorCode, null);
        }

        protected override async Task<GetRouteTargetConfigurationResponse> ProcessValidRequest(
            GetRouteTargetConfigurationRequest request, 
            IRoutingConfigurationStorage configurationStorage
        )
        {
            var configuration = await configurationStorage.GetRouteTargetConfiguration();
            return new GetRouteTargetConfigurationResponse(GetRouteTargetConfigurationResponse.ReturnCode.Ok, configuration);
        }
    }
}