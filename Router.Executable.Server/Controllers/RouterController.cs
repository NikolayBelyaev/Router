using System.Text;
using Microsoft.AspNetCore.Mvc;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers;
using Router.Core.Model;
using Router.Executable.Server.Attributes;
using Router.Executable.Server.Filters;
using Router.Shared.Router;

namespace Router.Executable.Server.Controllers
{
    [ApiController]
    [RequestLoggingFilter]
    [Route("Prod/")]
    public class RouterController : ControllerBase
    {
        private readonly IRoutingConfigurationStorage _storageProxy;

        public RouterController(IRoutingConfigurationStorage storageProxy)
        {
            _storageProxy = storageProxy;
        }
        
        [HttpPost("get-server-address")]
        public async Task<GetServerAddressResponse> GetServerAddress()
        {
            var body = await GetBody();
            var output = await new GetServerAddressRequestHandler().Handle(body, _storageProxy);
            return output;
        }

        [AuthorizationFilter]
        [HttpGet("get-routing-configuration")]
        public async Task<GetRoutingConfigurationResponse> GetRoutingConfiguration()
        {
            var body = await GetBody();
            var output = await new GetRoutingConfigurationRequestHandler().Handle(body, _storageProxy);
            return output;
        }

        [AuthorizationFilter]
        [HttpPost("set-routing-configuration")]
        public async Task<SetRoutingConfigurationResponse> SetRoutingConfiguration()
        {
            var body = await GetBody();
            var output = await new SetRoutingConfigurationRequestHandler().Handle(body, _storageProxy);
            return output;
        }

        [AuthorizationFilter]
        [HttpGet("get-route-target-configuration")]
        public async Task<GetRouteTargetConfigurationResponse> GetRouteTargetConfiguration()
        {
            var body = await GetBody();
            var output = await new GetRouteTargetConfigurationRequestHandler().Handle(body, _storageProxy);
            return output;
        }

        [AuthorizationFilter]
        [HttpPost("set-route-target-configuration")]
        public async Task<SetRouteTargetConfigurationResponse> SetRouteTargetConfiguration()
        {
            var body = await GetBody();
            var output = await new SetRouteTargetConfigurationRequestHandler().Handle(body, _storageProxy);
            return output;
        }

        [AuthorizationFilter]
        [HttpPost("set-maintenance-on")]
        public async Task<SetMaintenanceOnResponse> SetMaintenanceOn()
        {
            var body = await GetBody();
            var output = await new SetMaintenanceOnRequestHandler().Handle(body, _storageProxy);
            return output;
        }

        [AuthorizationFilter]
        [HttpPost("set-maintenance-off")]
        public async Task<SetMaintenanceOffResponse> SetMaintenanceOff()
        {
            var body = await GetBody();
            var output = await new SetMaintenanceOffRequestHandler().Handle(body, _storageProxy);
            return output;
        }

        [AuthorizationFilter]
        [HttpPost("add-routing-rule")]
        public async Task<AddRoutingRuleResponse> AddRoutingRule()
        {
            var body = await GetBody();
            return await new AddRoutingRuleRequestHandler().Handle(body, _storageProxy);
        }

        [AuthorizationFilter]
        [HttpPost("update-routing-rule")]
        public async Task<UpdateRoutingRuleResponse> UpdateRoutingRule()
        {
            var body = await GetBody();
            return await new UpdateRoutingRuleRequestHandler().Handle(body, _storageProxy);
        }

        [AuthorizationFilter]
        [HttpPost("delete-routing-rule")]
        public async Task<DeleteRoutingRuleResponse> DeleteRoutingRule()
        {
            var body = await GetBody();
            return await new DeleteRoutingRuleRequestHandler().Handle(body, _storageProxy);
        }

        [AuthorizationFilter]
        [HttpPost("add-route-target")]
        public async Task<AddRouteTargetResponse> AddRouteTarget()
        {
            var body = await GetBody();
            return await new AddRouteTargetRequestHandler().Handle(body, _storageProxy);
        }

        [AuthorizationFilter]
        [HttpPost("update-route-target")]
        public async Task<UpdateRouteTargetResponse> UpdateRouteTarget()
        {
            var body = await GetBody();
            return await new UpdateRouteTargetRequestHandler().Handle(body, _storageProxy);
        }

        [AuthorizationFilter]
        [HttpPost("delete-route-target")]
        public async Task<DeleteRouteTargetResponse> DeleteRouteTarget()
        {
            var body = await GetBody();
            return await new DeleteRouteTargetRequestHandler().Handle(body, _storageProxy);
        }

        private async Task<string> GetBody()
        {
            string body;
            using (StreamReader reader
                   = new StreamReader(ControllerContext.HttpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                body = await reader.ReadToEndAsync();
            }

            return body;
        }
    }
}