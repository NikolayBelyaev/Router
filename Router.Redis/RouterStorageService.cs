using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kit.Logging.Abstraction;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Router.Redis
{
    public class RouterStorageService : IRouterStorageService
    {
        private readonly IRouterRedisStorage _storage;
        private readonly IKitLogger _logger;

        public RouterStorageService(IRouterRedisStorage storage, IKitLogger logger)
        {
            _storage = storage;
            _logger = logger;
        }

        public async Task Push<TValue>(TValue value, string key)
        {
            try
            {
                var stringValue = JsonConvert.SerializeObject(value);
                await _storage.RouterConfiguration.StringSetAsync(key, stringValue);
            }
            catch (Exception e)
            {
                _logger.Error($"Error in {nameof(RouterStorageService)}. Redis key: {key} Message: {e.Message}", e);
            }
        }

        public async Task<TValue> Grab<TValue>(string key) where TValue : class
        {
            try
            {
                var stringValue = await _storage.RouterConfiguration.StringGetAsync(key);

                if (stringValue.IsNullOrEmpty)
                    return null;

                return JsonConvert.DeserializeObject<TValue>(stringValue);
            }
            catch (Exception e)
            {
                _logger.Error($"Error in {nameof(RouterStorageService)}. Message: {e.Message}", e);
            }

            return null;
        }

        public async Task HashSet<TValue>(string hashKey, string field, TValue value)
        {
            try
            {
                var json = JsonConvert.SerializeObject(value);
                await _storage.RouterConfiguration.HashSetAsync(hashKey, field, json);
            }
            catch (Exception e)
            {
                _logger.Error($"Error in {nameof(RouterStorageService)}.{nameof(HashSet)}. Key: {hashKey} Field: {field} Message: {e.Message}", e);
            }
        }

        public async Task HashDelete(string hashKey, string field)
        {
            try
            {
                await _storage.RouterConfiguration.HashDeleteAsync(hashKey, field);
            }
            catch (Exception e)
            {
                _logger.Error($"Error in {nameof(RouterStorageService)}.{nameof(HashDelete)}. Key: {hashKey} Field: {field} Message: {e.Message}", e);
            }
        }

        public async Task HashClear(string hashKey)
        {
            try
            {
                await _storage.RouterConfiguration.KeyDeleteAsync(hashKey);
            }
            catch (Exception e)
            {
                _logger.Error($"Error in {nameof(RouterStorageService)}.{nameof(HashClear)}. Key: {hashKey} Message: {e.Message}", e);
            }
        }

        public async Task<Dictionary<string, TValue>> HashGetAll<TValue>(string hashKey) where TValue : class
        {
            try
            {
                var entries = await _storage.RouterConfiguration.HashGetAllAsync(hashKey);

                if (entries == null || entries.Length == 0)
                    return null;

                return entries.ToDictionary(
                    e => e.Name.ToString(),
                    e => JsonConvert.DeserializeObject<TValue>(e.Value.ToString())
                );
            }
            catch (Exception e)
            {
                _logger.Error($"Error in {nameof(RouterStorageService)}.{nameof(HashGetAll)}. Key: {hashKey} Message: {e.Message}", e);
            }

            return null;
        }
    }
}
