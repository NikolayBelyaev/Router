using System.Threading.Tasks;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers.Abstraction;
using Router.Core.Model;

namespace Router.Core.Handlers
{
    public class SetMaintenanceOnRequestHandler 
        : RequestHandler<SetMaintenanceOnRequest, SetMaintenanceOnResponse, SetMaintenanceOnResponse.ReturnCode>
    {
        protected override SetMaintenanceOnResponse.ReturnCode Ok => SetMaintenanceOnResponse.ReturnCode.Ok;
        protected override SetMaintenanceOnResponse.ReturnCode BadRequest => SetMaintenanceOnResponse.ReturnCode.BadRequest;
        protected override SetMaintenanceOnResponse.ReturnCode InternalServerError => SetMaintenanceOnResponse.ReturnCode.InternalServerError;
        
        protected override Task<SetMaintenanceOnResponse.ReturnCode> Validate(
            SetMaintenanceOnRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            return string.IsNullOrEmpty(request.RouteTarget.Trim())
                ? Task.FromResult(SetMaintenanceOnResponse.ReturnCode.BadRequest)
                : Task.FromResult(SetMaintenanceOnResponse.ReturnCode.Ok);
        }

        protected override SetMaintenanceOnResponse RespondWithError(SetMaintenanceOnResponse.ReturnCode errorCode)
        {
            return new SetMaintenanceOnResponse(errorCode);
        }

        protected override async Task<SetMaintenanceOnResponse> ProcessValidRequest(
            SetMaintenanceOnRequest request,
            IRoutingConfigurationStorage configurationStorage
        )
        {
            await configurationStorage.BeginServerMaintenance(request.RouteTarget.ToLower());
            return new SetMaintenanceOnResponse(SetMaintenanceOnResponse.ReturnCode.Ok);
        }
    }
}