using Kit.Logging.Abstraction;
using Router.Core.ConfigurationStorage;
using Router.Core.Model.Configuration;

namespace Router.Executable.Server.HostedServices
{
    public class EnsureMaintenanceTargetHostedService : IHostedService
    {
        public const string MaintenanceTarget = "maintenance";
        public const string MaintenanceAddress = "http://maintenance";

        private readonly IRoutingConfigurationStorage _storage;
        private readonly IKitLogger _logger;

        public EnsureMaintenanceTargetHostedService(IRoutingConfigurationStorage storage, IKitLogger logger)
        {
            _storage = storage;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var config = await _storage.GetRouteTargetConfiguration()
                             ?? new RouteTargetConfiguration(Array.Empty<RouteTargetConfigurationEntry>());

                var hasMaintenance = config.Entries.Any(e =>
                    string.Equals(e.Target, MaintenanceTarget, StringComparison.CurrentCultureIgnoreCase));

                if (hasMaintenance)
                    return;

                var entry = new RouteTargetConfigurationEntry(MaintenanceTarget, MaintenanceAddress, true);
                await _storage.AddRouteTarget(entry);

                _logger.Info($"[EnsureMaintenanceTarget] Created reserved '{MaintenanceTarget}' target");
            }
            catch (Exception e)
            {
                _logger.Error($"[EnsureMaintenanceTarget] Failed to ensure '{MaintenanceTarget}' target: {e}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
