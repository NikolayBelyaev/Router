using System.Threading.Tasks;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers.Abstraction;
using Router.Core.Model;
using Router.Core.Model.Configuration;
using Router.Shared.Router;

namespace Router.Core.Handlers
{
    public class SetRoutingConfigurationRequestHandler 
        : RequestHandler<SetRoutingConfigurationRequest, SetRoutingConfigurationResponse, SetRoutingConfigurationResponse.ReturnCode>
    {
        protected override SetRoutingConfigurationResponse.ReturnCode Ok => SetRoutingConfigurationResponse.ReturnCode.Ok;
        protected override SetRoutingConfigurationResponse.ReturnCode BadRequest => SetRoutingConfigurationResponse.ReturnCode.BadRequest;
        protected override SetRoutingConfigurationResponse.ReturnCode InternalServerError => SetRoutingConfigurationResponse.ReturnCode.InternalServerError;
        
        protected override async Task<SetRoutingConfigurationResponse.ReturnCode> Validate(
            SetRoutingConfigurationRequest request, 
            IRoutingConfigurationStorage configurationStorage)
        {
            var entries = request.Configuration?.Entries;
            if (entries == null || entries.Length == 0)
                return SetRoutingConfigurationResponse.ReturnCode.BadConfiguration;

            foreach (var entry in entries)
            {
                if (string.IsNullOrEmpty(entry.Server.Trim()))
                    return SetRoutingConfigurationResponse.ReturnCode.BadConfiguration;
                
                if (string.IsNullOrEmpty(entry.Platform))
                    return SetRoutingConfigurationResponse.ReturnCode.BadConfiguration;
                
                if (string.IsNullOrEmpty(entry.RouteTarget.Trim()))
                    return SetRoutingConfigurationResponse.ReturnCode.BadConfiguration;
                
                if (!ClientBuildVersion.TryParse(entry.ClientVersion, out _))
                    return SetRoutingConfigurationResponse.ReturnCode.BadConfiguration;
                
                if (entry.UpdateMode == UpdateMode.None)
                    return SetRoutingConfigurationResponse.ReturnCode.BadConfiguration;
            }

            var routeTargets = await configurationStorage.GetRouteTargetConfiguration();
            foreach (var configuration in request.Configuration.Entries)
            {
                var foundTarget = false;
                var foundServer = false;
                foreach (var routeTarget in routeTargets.Entries)
                {
                    if (routeTarget.Target == configuration.RouteTarget.ToLower())
                        foundTarget = true;
                    if (routeTarget.Target == configuration.Server.ToLower())
                        foundServer = true;
                }

                if (!foundTarget || !foundServer)
                    return SetRoutingConfigurationResponse.ReturnCode.BadConfiguration;
            }
            
            return Ok;
        }

        protected override SetRoutingConfigurationResponse RespondWithError(SetRoutingConfigurationResponse.ReturnCode errorCode)
        {
            return new SetRoutingConfigurationResponse(errorCode, null);
        }

        protected override async Task<SetRoutingConfigurationResponse> ProcessValidRequest(
            SetRoutingConfigurationRequest request,
            IRoutingConfigurationStorage configurationStorage
        )
        {
            await configurationStorage.SetRoutingConfiguration(request.Configuration);

            var updated = await configurationStorage.GetRoutingConfiguration();
            
            return new SetRoutingConfigurationResponse(SetRoutingConfigurationResponse.ReturnCode.Ok, updated);
        }
    }
}