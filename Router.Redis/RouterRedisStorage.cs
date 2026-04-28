using System;
using Kit.Services.Redis.ConnectionProvider;
using Router.Redis.Enums;
using StackExchange.Redis;

namespace Router.Redis
{
    public class RouterRedisStorage : IDisposable, IRouterRedisStorage
    {
        private readonly Lazy<IConnectionMultiplexer> _multiplexer;

        public IDatabase RouterConfiguration => _lazyDatabase.Value;

        private readonly Lazy<IDatabase> _lazyDatabase;

        public RouterRedisStorage(IRedisConnectionProvider redisConnectionProvider)
        {
            _multiplexer = redisConnectionProvider.Connection;

            _lazyDatabase = new Lazy<IDatabase>(() => GetDatabase(RedisDatabase.Router));
        }

        private IDatabase GetDatabase(RedisDatabase database)
        {
            return _multiplexer.Value.GetDatabase((int)database);
        }

        public void Dispose()
        {
            _multiplexer?.Value?.Dispose();
        }
    }
}