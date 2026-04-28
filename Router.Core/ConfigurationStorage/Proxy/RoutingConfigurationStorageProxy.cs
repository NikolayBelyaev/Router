using System;
using System.Linq;
using System.Threading.Tasks;
using Kit.Logging.Abstraction;
using Router.Core.Model.Configuration;

namespace Router.Core.ConfigurationStorage.Proxy
{
    public class RoutingConfigurationStorageProxy : IRoutingConfigurationStorage
    {
        private readonly DynamoDbRoutingConfigurationStorage _dynamoDbStorage;
        private readonly IRouterRedisService _routerRedisService;
        private readonly IKitLogger _logger;

        private Task _updateRouterTask;
        private RoutingConfiguration _routingConfiguration;
        private RouteTargetConfiguration _routeTargetConfiguration;
        private DateTimeOffset _lastUpdateTime;

        public RoutingConfigurationStorageProxy(DynamoDbRoutingConfigurationStorage dynamoDbStorage, IRouterRedisService routerRedisService, IKitLogger logger)
        {
            _dynamoDbStorage = dynamoDbStorage;
            _routerRedisService = routerRedisService;
            _logger = logger;

            _updateRouterTask = null;
            _routingConfiguration = null;
            _routeTargetConfiguration = null;
        }

        public async Task<RoutingConfiguration> GetRoutingConfiguration()
        {
            var config = await _routerRedisService.GetRoutingConfiguration();

            if (config == null)
            {
                config = await _dynamoDbStorage.GetRoutingConfiguration();
                await _routerRedisService.PushRoutingConfiguration(config);
            }

            return config;
        }

        public async Task SetRoutingConfiguration(RoutingConfiguration configuration)
        {
            await _dynamoDbStorage.SetRoutingConfiguration(configuration);
            await _routerRedisService.PushRoutingConfiguration(configuration);
        }

        public async Task<RouteTargetConfiguration> GetRouteTargetConfiguration()
        {
            var config = await _routerRedisService.GetRouteTargetConfiguration();

            if (config == null)
            {
                config = await _dynamoDbStorage.GetRouteTargetConfiguration();
                await _routerRedisService.PushRouteTargetConfiguration(config);
            }

            return config;
        }

        public async Task SetRouteTargetConfiguration(RouteTargetConfiguration configuration)
        {
            await _dynamoDbStorage.SetRouteTargetConfiguration(configuration);
            await _routerRedisService.PushRouteTargetConfiguration(configuration);
        }

        public async Task BeginServerMaintenance(string target)
        {
            await _dynamoDbStorage.BeginServerMaintenance(target);

            var config = await _dynamoDbStorage.GetRouteTargetConfiguration();
            await _routerRedisService.PushRouteTargetConfiguration(config);
        }

        public async Task FinishServerMaintenance(string target)
        {
            await _dynamoDbStorage.FinishServerMaintenance(target);

            var config = await _dynamoDbStorage.GetRouteTargetConfiguration();
            await _routerRedisService.PushRouteTargetConfiguration(config);
        }

        public async Task<RouteTargetConfigurationEntry> GetRouteTarget(string target)
        {
            await Update();

            return _routeTargetConfiguration.Entries
                .FirstOrDefault(x => string.Equals(x.Target, target, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<RoutingConfigurationEntry[]> GetRouting(string serverType)
        {
            await Update();

            return _routingConfiguration.Entries
                .Where(x => string.Equals(x.Server, serverType, StringComparison.CurrentCultureIgnoreCase))
                .ToArray();
        }

        public async Task<string> AddRoutingRule(RoutingConfigurationEntry entry)
        {
            var id = await _dynamoDbStorage.AddRoutingRule(entry);
            var savedEntry = new RoutingConfigurationEntry(id, entry.Server, entry.Platform, entry.ClientVersion, entry.RouteTarget, entry.UpdateMode);
            await _routerRedisService.PushRoutingRule(id, savedEntry);
            return id;
        }

        public async Task UpdateRoutingRule(string id, RoutingConfigurationEntry entry)
        {
            await _dynamoDbStorage.UpdateRoutingRule(id, entry);
            await _routerRedisService.PushRoutingRule(id, entry);
        }

        public async Task DeleteRoutingRule(string id)
        {
            await _dynamoDbStorage.DeleteRoutingRule(id);
            await _routerRedisService.RemoveRoutingRule(id);
        }

        public async Task AddRouteTarget(RouteTargetConfigurationEntry entry)
        {
            await _dynamoDbStorage.AddRouteTarget(entry);
            await _routerRedisService.PushRouteTarget(entry.Target, entry);
        }

        public async Task UpdateRouteTarget(string oldTarget, RouteTargetConfigurationEntry entry)
        {
            await _dynamoDbStorage.UpdateRouteTarget(oldTarget, entry);

            if (oldTarget != entry.Target)
                await _routerRedisService.RemoveRouteTarget(oldTarget);

            await _routerRedisService.PushRouteTarget(entry.Target, entry);
        }

        public async Task DeleteRouteTarget(string target)
        {
            await _dynamoDbStorage.DeleteRouteTarget(target);
            await _routerRedisService.RemoveRouteTarget(target);
        }

        private async Task Update()
        {
            if (_routingConfiguration == null || _routeTargetConfiguration == null)
                await UpdateRouting();

            if (_lastUpdateTime < DateTimeOffset.Now - TimeSpan.FromSeconds(10) && (_updateRouterTask == null || _updateRouterTask.IsCompleted))
                _updateRouterTask = UpdateRouting();

            if (_lastUpdateTime < DateTimeOffset.Now - TimeSpan.FromSeconds(30))
                _logger.Error($"[RoutingConfigurationStorageProxy] Router is too old. {_lastUpdateTime}");
        }

        public async Task UpdateRouting()
        {
            _lastUpdateTime = DateTimeOffset.Now;
            _routingConfiguration = await GetRoutingConfiguration();
            _routeTargetConfiguration = await GetRouteTargetConfiguration();

            _logger.Info("[UpdateRouting] Router updated");
        }
    }
}
