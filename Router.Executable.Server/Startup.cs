using System.Text.Json.Serialization;
using Amazon;
using Kit.Exe.Environment;
using Kit.ExecutableKit.HealthCheck;
using Kit.Logging;
using Kit.Logging.Abstraction;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Router.Core.ConfigurationStorage;
using Router.Core.ConfigurationStorage.Proxy;
using Router.Executable.Server.HostedServices;
using Router.Executable.Server.Options;
using Router.Redis;

namespace Router.Executable.Server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IRuntimeEnvironment _environment;

        public Startup()
        {
            _environment = new EnvironmentVariableRuntimeEnvironment();
            _configuration = _environment.BuildConfiguration();
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .Configure<RouterConfiguration>(_configuration.GetSection("RouterConfiguration"))
                .AddHealthChecks()
                .Add(
                    new HealthCheckRegistration(
                        "default",
                        _ => new DefaultHealthCheck(),
                        null,
                        new[] {"default"}
                    )
                );

            serviceCollection.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder // todo: make these options more specific (for security reasons)
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            serviceCollection.AddScoped<DynamoDbRoutingConfigurationStorage>(provider =>
            {
                var storageConfiguration = provider.GetService<IOptions<RouterConfiguration>>()?.Value;

                if (storageConfiguration == null)
                {
                    throw new Exception("Can't get router configuration");
                }
                
                return new DynamoDbRoutingConfigurationStorage(
                    storageConfiguration.AWSAccessKeyId,
                    storageConfiguration.AWSSecret,
                    RegionEndpoint.EUCentral1,
                    storageConfiguration.DynamoDbTablePrefix,
                    provider.GetService<IKitLogger>());
            });

            serviceCollection
                .AddRouterRedisStorage(_configuration)
                .AddScoped<IRouterRedisService, RouterRedisService>();

            if (_environment.IsLocal() || _environment.IsDebug())
                serviceCollection.AddSingleton<IRoutingConfigurationStorage, MockRoutingConfigurationStorage>();
            else
                serviceCollection.AddSingleton<IRoutingConfigurationStorage, RoutingConfigurationStorageProxy>();

            serviceCollection.AddHostedService<EnsureMaintenanceTargetHostedService>();

            serviceCollection
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                });

            serviceCollection.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            serviceCollection.AddConsoleLogging();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors();
            app.UseAuthentication();

            app.UseMvc();
            app.UseHealthChecks("/health");
        }
    }
}