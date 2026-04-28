using System;
using System.Linq;
using System.Threading.Tasks;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers.Abstraction;
using Router.Core.Model;
using Router.Core.Model.Configuration;

namespace Router.Core.Handlers
{
    public class UpdateRouteTargetRequestHandler
        : RequestHandler<UpdateRouteTargetRequest, UpdateRouteTargetResponse, UpdateRouteTargetResponse.ReturnCode>
    {
        protected override UpdateRouteTargetResponse.ReturnCode Ok => UpdateRouteTargetResponse.ReturnCode.Ok;
        protected override UpdateRouteTargetResponse.ReturnCode BadRequest => UpdateRouteTargetResponse.ReturnCode.BadRequest;
        protected override UpdateRouteTargetResponse.ReturnCode InternalServerError => UpdateRouteTargetResponse.ReturnCode.InternalServerError;

        protected override async Task<UpdateRouteTargetResponse.ReturnCode> Validate(
            UpdateRouteTargetRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Id))
                return UpdateRouteTargetResponse.ReturnCode.BadRequest;

            if (string.IsNullOrWhiteSpace(request.Target))
                return UpdateRouteTargetResponse.ReturnCode.BadRequest;

            if (string.IsNullOrWhiteSpace(request.Address))
                return UpdateRouteTargetResponse.ReturnCode.BadRequest;

            if (!Uri.TryCreate(request.Address, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                return UpdateRouteTargetResponse.ReturnCode.BadRequest;

            var existing = await configurationStorage.GetRouteTargetConfiguration();
            var existingEntry = existing?.Entries?.FirstOrDefault(t => t.Target == request.Id);

            if (existingEntry == null)
                return UpdateRouteTargetResponse.ReturnCode.NotFound;

            if (request.Id != request.Target)
            {
                var conflict = existing.Entries.Any(t => t.Target == request.Target);
                if (conflict)
                    return UpdateRouteTargetResponse.ReturnCode.Conflict;
            }

            return Ok;
        }

        protected override UpdateRouteTargetResponse RespondWithError(UpdateRouteTargetResponse.ReturnCode errorCode)
        {
            return new UpdateRouteTargetResponse(errorCode);
        }

        protected override async Task<UpdateRouteTargetResponse> ProcessValidRequest(
            UpdateRouteTargetRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            var entry = new RouteTargetConfigurationEntry(request.Target, request.Address, request.Maintenance);

            await configurationStorage.UpdateRouteTarget(request.Id, entry);

            return new UpdateRouteTargetResponse(UpdateRouteTargetResponse.ReturnCode.Ok, entry);
        }
    }
}
