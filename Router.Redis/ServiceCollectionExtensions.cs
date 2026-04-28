using Kit.Services.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Router.Redis
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRouterRedisStorage(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddRedis(configuration)
                .AddScoped<IRouterRedisStorage, RouterRedisStorage>()
                .AddScoped<IRouterStorageService, RouterStorageService>();

            return services;
        }
    }
}