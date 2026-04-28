using System;
using System.Linq;
using System.Threading.Tasks;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers.Abstraction;
using Router.Core.Model;
using Router.Core.Model.Configuration;

namespace Router.Core.Handlers
{
    public class AddRouteTargetRequestHandler
        : RequestHandler<AddRouteTargetRequest, AddRouteTargetResponse, AddRouteTargetResponse.ReturnCode>
    {
        protected override AddRouteTargetResponse.ReturnCode Ok => AddRouteTargetResponse.ReturnCode.Ok;
        protected override AddRouteTargetResponse.ReturnCode BadRequest => AddRouteTargetResponse.ReturnCode.BadRequest;
        protected override AddRouteTargetResponse.ReturnCode InternalServerError => AddRouteTargetResponse.ReturnCode.InternalServerError;

        protected override async Task<AddRouteTargetResponse.ReturnCode> Validate(
            AddRouteTargetRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            if (request == null)
                return AddRouteTargetResponse.ReturnCode.BadRequest;

            if (string.IsNullOrWhiteSpace(request.Target))
                return AddRouteTargetResponse.ReturnCode.BadRequest;

            if (string.IsNullOrWhiteSpace(request.Address))
                return AddRouteTargetResponse.ReturnCode.BadRequest;

            if (!Uri.TryCreate(request.Address, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                return AddRouteTargetResponse.ReturnCode.BadRequest;

            var existing = await configurationStorage.GetRouteTargetConfiguration();
            var conflict = existing?.Entries != null && existing.Entries.Any(t => t.Target == request.Target);

            if (conflict)
                return AddRouteTargetResponse.ReturnCode.Conflict;

            return Ok;
        }

        protected override AddRouteTargetResponse RespondWithError(AddRouteTargetResponse.ReturnCode errorCode)
        {
            return new AddRouteTargetResponse(errorCode);
        }

        protected override async Task<AddRouteTargetResponse> ProcessValidRequest(
            AddRouteTargetRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            var entry = new RouteTargetConfigurationEntry(request.Target, request.Address, request.Maintenance);

            await configurationStorage.AddRouteTarget(entry);

            return new AddRouteTargetResponse(AddRouteTargetResponse.ReturnCode.Ok, entry);
        }
    }
}
