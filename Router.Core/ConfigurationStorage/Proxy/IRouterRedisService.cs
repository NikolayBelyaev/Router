using System.Threading.Tasks;
using Router.Core.Model.Configuration;

namespace Router.Core.ConfigurationStorage.Proxy
{
    public interface IRouterRedisService
    {
        Task<RoutingConfiguration> GetRoutingConfiguration();
        Task<RouteTargetConfiguration> GetRouteTargetConfiguration();
        Task PushRoutingConfiguration(RoutingConfiguration config);
        Task PushRouteTargetConfiguration(RouteTargetConfiguration config);

        Task PushRoutingRule(string id, RoutingConfigurationEntry entry);
        Task RemoveRoutingRule(string id);

        Task PushRouteTarget(string target, RouteTargetConfigurationEntry entry);
        Task RemoveRouteTarget(string target);
    }
}
