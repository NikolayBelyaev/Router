using Kit.Services.Redis.Configuration;
using Kit.Services.Redis.ConnectionProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kit.Services.Redis
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<RedisConfiguration>(configuration.GetSection(nameof(RedisConfiguration)))
                .AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();
        }
    }
}