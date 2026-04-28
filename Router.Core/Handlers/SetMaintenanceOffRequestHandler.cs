using System.Threading.Tasks;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers.Abstraction;
using Router.Core.Model;

namespace Router.Core.Handlers
{
    public class SetMaintenanceOffRequestHandler 
        : RequestHandler<SetMaintenanceOffRequest, SetMaintenanceOffResponse, SetMaintenanceOffResponse.ReturnCode>
    {
        protected override SetMaintenanceOffResponse.ReturnCode Ok => SetMaintenanceOffResponse.ReturnCode.Ok;
        protected override SetMaintenanceOffResponse.ReturnCode BadRequest => SetMaintenanceOffResponse.ReturnCode.BadRequest;
        protected override SetMaintenanceOffResponse.ReturnCode InternalServerError => SetMaintenanceOffResponse.ReturnCode.InternalServerError;
        
        protected override Task<SetMaintenanceOffResponse.ReturnCode> Validate(
            SetMaintenanceOffRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            return string.IsNullOrEmpty(request.RouteTarget.Trim())
                ? Task.FromResult(SetMaintenanceOffResponse.ReturnCode.BadRequest)
                : Task.FromResult(SetMaintenanceOffResponse.ReturnCode.Ok);
        }

        protected override SetMaintenanceOffResponse RespondWithError(SetMaintenanceOffResponse.ReturnCode errorCode)
        {
            return new SetMaintenanceOffResponse(errorCode);
        }

        protected override async Task<SetMaintenanceOffResponse> ProcessValidRequest(
            SetMaintenanceOffRequest request,
            IRoutingConfigurationStorage configurationStorage
        )
        {
            await configurationStorage.FinishServerMaintenance(request.RouteTarget.ToLower());
            return new SetMaintenanceOffResponse(SetMaintenanceOffResponse.ReturnCode.Ok);
        }
    }
}