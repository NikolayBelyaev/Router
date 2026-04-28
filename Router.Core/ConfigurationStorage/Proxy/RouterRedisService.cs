using System;
using System.Linq;
using System.Threading.Tasks;
using Kit.Logging.Abstraction;
using Router.Core.Model.Configuration;
using Router.Redis;
using Router.Redis.Constants;

namespace Router.Core.ConfigurationStorage.Proxy
{
    public class RouterRedisService : IRouterRedisService
    {
        private readonly IRouterStorageService _redisStorage;
        private readonly IKitLogger _logger;

        public RouterRedisService(IRouterStorageService redisStorage, IKitLogger logger)
        {
            _redisStorage = redisStorage;
            _logger = logger;
        }

        public async Task<RoutingConfiguration> GetRoutingConfiguration()
        {
            try
            {
                var entries = await _redisStorage.HashGetAll<RoutingConfigurationEntry>(RouterRedisKeys.RoutingConfigurationRedisKey);

                if (entries == null || entries.Count == 0)
                    return null;

                return new RoutingConfiguration(entries.Values.ToArray());
            }
            catch (Exception e)
            {
                _logger.Error($"[RouterRedisService.GetRoutingConfiguration] Exception: {e}");
                return null;
            }
        }

        public async Task<RouteTargetConfiguration> GetRouteTargetConfiguration()
        {
            try
            {
                var entries = await _redisStorage.HashGetAll<RouteTargetConfigurationEntry>(RouterRedisKeys.RouteTargetConfigurationRedisKey);

                if (entries == null || entries.Count == 0)
                    return null;

                return new RouteTargetConfiguration(entries.Values.ToArray());
            }
            catch (Exception e)
            {
                _logger.Error($"[RouterRedisService.GetRouteTargetConfiguration] Exception: {e}");
                return null;
            }
        }

        public async Task PushRoutingConfiguration(RoutingConfiguration config)
        {
            try
            {
                await _redisStorage.HashClear(RouterRedisKeys.RoutingConfigurationRedisKey);

                foreach (var entry in config.Entries)
                    await _redisStorage.HashSet(RouterRedisKeys.RoutingConfigurationRedisKey, entry.Id, entry);
            }
            catch (Exception e)
            {
                _logger.Error($"[RouterRedisService.PushRoutingConfiguration] Exception: {e}");
            }
        }

        public async Task PushRouteTargetConfiguration(RouteTargetConfiguration config)
        {
            try
            {
                await _redisStorage.HashClear(RouterRedisKeys.RouteTargetConfigurationRedisKey);

                foreach (var entry in config.Entries)
                    await _redisStorage.HashSet(RouterRedisKeys.RouteTargetConfigurationRedisKey, entry.Target, entry);
            }
            catch (Exception e)
            {
                _logger.Error($"[RouterRedisService.PushRouteTargetConfiguration] Exception: {e}");
            }
        }

        public async Task PushRoutingRule(string id, RoutingConfigurationEntry entry)
        {
            try
            {
                await _redisStorage.HashSet(RouterRedisKeys.RoutingConfigurationRedisKey, id, entry);
            }
            catch (Exception e)
            {
                _logger.Error($"[RouterRedisService.PushRoutingRule] Exception: {e}");
            }
        }

        public async Task RemoveRoutingRule(string id)
        {
            try
            {
                await _redisStorage.HashDelete(RouterRedisKeys.RoutingConfigurationRedisKey, id);
            }
            catch (Exception e)
            {
                _logger.Error($"[RouterRedisService.RemoveRoutingRule] Exception: {e}");
            }
        }

        public async Task PushRouteTarget(string target, RouteTargetConfigurationEntry entry)
        {
            try
            {
                await _redisStorage.HashSet(RouterRedisKeys.RouteTargetConfigurationRedisKey, target, entry);
            }
            catch (Exception e)
            {
                _logger.Error($"[RouterRedisService.PushRouteTarget] Exception: {e}");
            }
        }

        public async Task RemoveRouteTarget(string target)
        {
            try
            {
                await _redisStorage.HashDelete(RouterRedisKeys.RouteTargetConfigurationRedisKey, target);
            }
            catch (Exception e)
            {
                _logger.Error($"[RouterRedisService.RemoveRouteTarget] Exception: {e}");
            }
        }
    }
}
