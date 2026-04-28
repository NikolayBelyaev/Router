using System.Threading.Tasks;
using Router.Core.Model.Configuration;

namespace Router.Core.ConfigurationStorage
{
    public interface IRoutingConfigurationStorage
    {
        Task<RoutingConfiguration> GetRoutingConfiguration();
        Task SetRoutingConfiguration(RoutingConfiguration configuration);

        Task<RouteTargetConfiguration> GetRouteTargetConfiguration();
        Task SetRouteTargetConfiguration(RouteTargetConfiguration configuration);

        Task BeginServerMaintenance(string target);
        Task FinishServerMaintenance(string target);

        Task<RouteTargetConfigurationEntry> GetRouteTarget(string target);
        Task<RoutingConfigurationEntry[]> GetRouting(string serverType);

        Task<string> AddRoutingRule(RoutingConfigurationEntry entry);
        Task UpdateRoutingRule(string id, RoutingConfigurationEntry entry);
        Task DeleteRoutingRule(string id);

        Task AddRouteTarget(RouteTargetConfigurationEntry entry);
        Task UpdateRouteTarget(string oldTarget, RouteTargetConfigurationEntry entry);
        Task DeleteRouteTarget(string target);
    }
}
