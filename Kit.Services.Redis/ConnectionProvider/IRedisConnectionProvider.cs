using System;
using StackExchange.Redis;

namespace Kit.Services.Redis.ConnectionProvider
{
    public interface IRedisConnectionProvider
    {
        Lazy<IConnectionMultiplexer> Connection { get; }
        public string Host { get; }
    }
}