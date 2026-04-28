using System.Linq;
using System.Threading.Tasks;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers.Abstraction;
using Router.Core.Model;

namespace Router.Core.Handlers
{
    public class DeleteRoutingRuleRequestHandler
        : RequestHandler<DeleteRoutingRuleRequest, DeleteRoutingRuleResponse, DeleteRoutingRuleResponse.ReturnCode>
    {
        protected override DeleteRoutingRuleResponse.ReturnCode Ok => DeleteRoutingRuleResponse.ReturnCode.Ok;
        protected override DeleteRoutingRuleResponse.ReturnCode BadRequest => DeleteRoutingRuleResponse.ReturnCode.BadRequest;
        protected override DeleteRoutingRuleResponse.ReturnCode InternalServerError => DeleteRoutingRuleResponse.ReturnCode.InternalServerError;

        protected override async Task<DeleteRoutingRuleResponse.ReturnCode> Validate(
            DeleteRoutingRuleRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Id))
                return DeleteRoutingRuleResponse.ReturnCode.BadRequest;

            var existing = await configurationStorage.GetRoutingConfiguration();
            var exists = existing?.Entries != null && existing.Entries.Any(e => e.Id == request.Id);

            if (!exists)
                return DeleteRoutingRuleResponse.ReturnCode.NotFound;

            return Ok;
        }

        protected override DeleteRoutingRuleResponse RespondWithError(DeleteRoutingRuleResponse.ReturnCode errorCode)
        {
            return new DeleteRoutingRuleResponse(errorCode);
        }

        protected override async Task<DeleteRoutingRuleResponse> ProcessValidRequest(
            DeleteRoutingRuleRequest request,
            IRoutingConfigurationStorage configurationStorage)
        {
            await configurationStorage.DeleteRoutingRule(request.Id);

            return new DeleteRoutingRuleResponse(DeleteRoutingRuleResponse.ReturnCode.Ok);
        }
    }
}
