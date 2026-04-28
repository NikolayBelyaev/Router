using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers.Abstraction;
using Router.Core.Model;
using Router.Core.Model.Configuration;
using Router.Shared.Router;

namespace Router.Core.Handlers
{
    public class AddRoutingRuleRequestHandler
        : RequestHandler<AddRoutingRuleRequest, AddRoutingRuleResponse, AddRoutingRuleResponse.ReturnCode>
    {
        protected override AddRoutingRuleResponse.ReturnCode Ok => AddRoutingRuleResponse.ReturnCode.Ok;
        protected override AddRoutingRuleResponse.ReturnCode BadRequest => AddRoutingRuleResponse.ReturnCode.BadRequest;

        protected override AddRoutingRuleResponse.ReturnCode InternalServerError =>
            AddRoutingRuleResponse.ReturnCode.InternalServerError;

        protected override async Task<AddRoutingRuleResponse.ReturnCode> Validate(
            AddRoutingRuleRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            if (request == null)
                return AddRoutingRuleResponse.ReturnCode.BadRequest;

            if (string.IsNullOrWhiteSpace(request.Server))
                return AddRoutingRuleResponse.ReturnCode.BadRequest;

            if (string.IsNullOrWhiteSpace(request.Platform))
                return AddRoutingRuleResponse.ReturnCode.BadRequest;

            if (string.IsNullOrWhiteSpace(request.RouteTarget))
                return AddRoutingRuleResponse.ReturnCode.BadRequest;

            if (!ClientBuildVersion.TryParse(request.ClientVersion, out _))
                return AddRoutingRuleResponse.ReturnCode.BadRequest;

            if (request.UpdateMode == UpdateMode.None)
                return AddRoutingRuleResponse.ReturnCode.BadConfiguration;

            var targets = await configurationStorage.GetRouteTargetConfiguration();
            var targetNames = targets?.Entries?.Select(t => t.Target).ToHashSet() ?? new HashSet<string>();

            if (!targetNames.Contains(request.RouteTarget))
                return AddRoutingRuleResponse.ReturnCode.BadConfiguration;

            if (!targetNames.Contains(request.Server))
                return AddRoutingRuleResponse.ReturnCode.BadConfiguration;

            var existing = await configurationStorage.GetRoutingConfiguration();
            var conflict = existing?.Entries != null
                           && existing.Entries.Any(e =>
                               e.Server == request.Server &&
                               e.Platform == request.Platform &&
                               e.ClientVersion == request.ClientVersion);

            if (conflict)
                return AddRoutingRuleResponse.ReturnCode.Conflict;

            return Ok;
        }

        protected override AddRoutingRuleResponse RespondWithError(AddRoutingRuleResponse.ReturnCode errorCode)
        {
            return new AddRoutingRuleResponse(errorCode);
        }

        protected override async Task<AddRoutingRuleResponse> ProcessValidRequest(
            AddRoutingRuleRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            var entry = new RoutingConfigurationEntry(
                string.Empty,
                request.Server,
                request.Platform,
                request.ClientVersion,
                request.RouteTarget,
                request.UpdateMode
            );

            var id = await configurationStorage.AddRoutingRule(entry);

            var savedEntry = new RoutingConfigurationEntry(id, request.Server, request.Platform, request.ClientVersion,
                request.RouteTarget, request.UpdateMode);

            return new AddRoutingRuleResponse(AddRoutingRuleResponse.ReturnCode.Ok, id, savedEntry);
        }
    }
}