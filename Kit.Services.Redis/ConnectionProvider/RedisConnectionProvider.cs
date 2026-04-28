using System;
using System.Threading;
using Kit.Services.Redis.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Kit.Services.Redis.ConnectionProvider
{
    public class RedisConnectionProvider : IRedisConnectionProvider, IDisposable
    {
        public Lazy<IConnectionMultiplexer> Connection { get; }
        public string Host { get; }

        public RedisConnectionProvider(IOptions<RedisConfiguration> configuration)
        {
            Connection = new Lazy<IConnectionMultiplexer>(
                () =>
                {
                    var options = ConfigurationOptions.Parse(configuration.Value.ConnectionString);
                    options.AbortOnConnectFail = false;
                    
                    return ConnectionMultiplexer.Connect(options);
                },
                LazyThreadSafetyMode.ExecutionAndPublication
            );
            Host = configuration.Value.Host;
        }

        public void Dispose()
        {
            Connection?.Value?.Dispose();
        }
    }
}