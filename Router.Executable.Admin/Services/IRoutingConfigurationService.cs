using Router.Core.Model;
using Router.Core.Model.Configuration;
using Router.Executable.Admin.Enums;
using Router.Shared.Router;

namespace Router.Executable.Admin.Services;

public interface IRoutingConfigurationService
{
    Task<GetRoutingConfigurationResponse> GetRoutingConfig();
    Task<GetRouteTargetConfigurationResponse> GetRouteTargetConfig();
    Task<SetRoutingConfigurationResponse> SaveRoutingConfiguration(RoutingConfiguration config);
    Task<SetRouteTargetConfigurationResponse> SaveRouteTargetConfig(RouteTargetConfiguration config);
    Task<SetMaintenanceOnResponse> SetMaintenanceOn(string pendingTargetName);
    Task<SetMaintenanceOffResponse> SetMaintenanceOff(string pendingTargetName);
    Task<AddRoutingRuleResponse> AddRoutingRule(string server, ClientPlatform platform, ClientBuildVersion version, string routeTarget, UpdateMode updateMode);
    Task<UpdateRoutingRuleResponse> UpdateRoutingRule(string id, string server, ClientPlatform platform, ClientBuildVersion version, string routeTarget, UpdateMode updateMode);
    Task<DeleteRoutingRuleResponse> DeleteRoutingRule(string id);
    Task<AddRouteTargetResponse> AddRouteTarget(string target, string address, bool maintenance);
    Task<UpdateRouteTargetResponse> UpdateRouteTarget(string originalTarget, string target, string address, bool maintenance);
    Task<DeleteRouteTargetResponse> DeleteRouteTarget(string target);
}