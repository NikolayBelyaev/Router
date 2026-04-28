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
    public class UpdateRoutingRuleRequestHandler
        : RequestHandler<UpdateRoutingRuleRequest, UpdateRoutingRuleResponse, UpdateRoutingRuleResponse.ReturnCode>
    {
        protected override UpdateRoutingRuleResponse.ReturnCode Ok => UpdateRoutingRuleResponse.ReturnCode.Ok;
        protected override UpdateRoutingRuleResponse.ReturnCode BadRequest => UpdateRoutingRuleResponse.ReturnCode.BadRequest;
        protected override UpdateRoutingRuleResponse.ReturnCode InternalServerError => UpdateRoutingRuleResponse.ReturnCode.InternalServerError;

        protected override async Task<UpdateRoutingRuleResponse.ReturnCode> Validate(
            UpdateRoutingRuleRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Id))
                return UpdateRoutingRuleResponse.ReturnCode.BadRequest;

            if (string.IsNullOrWhiteSpace(request.Server))
                return UpdateRoutingRuleResponse.ReturnCode.BadRequest;

            if (string.IsNullOrWhiteSpace(request.Platform))
                return UpdateRoutingRuleResponse.ReturnCode.BadRequest;

            if (string.IsNullOrWhiteSpace(request.RouteTarget))
                return UpdateRoutingRuleResponse.ReturnCode.BadRequest;

            if (!ClientBuildVersion.TryParse(request.ClientVersion, out _))
                return UpdateRoutingRuleResponse.ReturnCode.BadRequest;

            if (request.UpdateMode == UpdateMode.None)
                return UpdateRoutingRuleResponse.ReturnCode.BadConfiguration;

            var existing = await configurationStorage.GetRoutingConfiguration();
            var existingEntry = existing?.Entries?.FirstOrDefault(e => e.Id == request.Id);

            if (existingEntry == null)
                return UpdateRoutingRuleResponse.ReturnCode.NotFound;

            var targets = await configurationStorage.GetRouteTargetConfiguration();
            var targetNames = targets?.Entries?.Select(t => t.Target).ToHashSet() ?? new HashSet<string>();

            if (!targetNames.Contains(request.RouteTarget))
                return UpdateRoutingRuleResponse.ReturnCode.BadConfiguration;

            if (!targetNames.Contains(request.Server))
                return UpdateRoutingRuleResponse.ReturnCode.BadConfiguration;

            return Ok;
        }

        protected override UpdateRoutingRuleResponse RespondWithError(UpdateRoutingRuleResponse.ReturnCode errorCode)
        {
            return new UpdateRoutingRuleResponse(errorCode);
        }

        protected override async Task<UpdateRoutingRuleResponse> ProcessValidRequest(
            UpdateRoutingRuleRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            var entry = new RoutingConfigurationEntry(
                request.Id,
                request.Server,
                request.Platform,
                request.ClientVersion,
                request.RouteTarget,
                request.UpdateMode
            );

            await configurationStorage.UpdateRoutingRule(request.Id, entry);

            return new UpdateRoutingRuleResponse(UpdateRoutingRuleResponse.ReturnCode.Ok, request.Id, entry);
        }
    }
}
