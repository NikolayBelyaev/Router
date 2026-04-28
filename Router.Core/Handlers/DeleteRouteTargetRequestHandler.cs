using System.Linq;
using System.Threading.Tasks;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers.Abstraction;
using Router.Core.Model;

namespace Router.Core.Handlers
{
    public class DeleteRouteTargetRequestHandler
        : RequestHandler<DeleteRouteTargetRequest, DeleteRouteTargetResponse, DeleteRouteTargetResponse.ReturnCode>
    {
        protected override DeleteRouteTargetResponse.ReturnCode Ok => DeleteRouteTargetResponse.ReturnCode.Ok;
        protected override DeleteRouteTargetResponse.ReturnCode BadRequest => DeleteRouteTargetResponse.ReturnCode.BadRequest;
        protected override DeleteRouteTargetResponse.ReturnCode InternalServerError => DeleteRouteTargetResponse.ReturnCode.InternalServerError;

        protected override async Task<DeleteRouteTargetResponse.ReturnCode> Validate(
            DeleteRouteTargetRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Target))
                return DeleteRouteTargetResponse.ReturnCode.BadRequest;

            var targets = await configurationStorage.GetRouteTargetConfiguration();
            var exists = targets?.Entries != null && targets.Entries.Any(t => t.Target == request.Target);

            if (!exists)
                return DeleteRouteTargetResponse.ReturnCode.NotFound;

            var rules = await configurationStorage.GetRoutingConfiguration();
            var inUse = rules?.Entries != null && rules.Entries.Any(e => e.RouteTarget == request.Target);

            if (inUse)
                return DeleteRouteTargetResponse.ReturnCode.BadRequest;

            return Ok;
        }

        protected override DeleteRouteTargetResponse RespondWithError(DeleteRouteTargetResponse.ReturnCode errorCode)
        {
            return new DeleteRouteTargetResponse(errorCode);
        }

        protected override async Task<DeleteRouteTargetResponse> ProcessValidRequest(
            DeleteRouteTargetRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            await configurationStorage.DeleteRouteTarget(request.Target);

            return new DeleteRouteTargetResponse(DeleteRouteTargetResponse.ReturnCode.Ok);
        }
    }
}
