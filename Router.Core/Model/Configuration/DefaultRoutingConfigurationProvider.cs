using System;
using Router.Shared.Router;

namespace Router.Core.Model.Configuration
{
    public static class DefaultRoutingConfigurationProvider
    {
        public static RouteTargetConfiguration RouteTargetConfiguration => new RouteTargetConfiguration(
            new[]
            {
                new RouteTargetConfigurationEntry(
                    "development",
                    "http://development.mock",
                    false
                ),
                new RouteTargetConfigurationEntry(
                    "production",
                    "http://production.mock",
                    false
                ),
                new RouteTargetConfigurationEntry(
                    "maintenance",
                    "http://localhost:80",
                    true
                ),
            }
        );

        public static RoutingConfiguration RoutingConfiguration => new RoutingConfiguration(
            new[]
            {
                new RoutingConfigurationEntry(
                    Guid.NewGuid().ToString(),
                    "development",
                    ClientPlatformConstants.Ios,
                    "0.0.0",
                    "development",
                    UpdateMode.UpToDate
                ),
                new RoutingConfigurationEntry(
                    Guid.NewGuid().ToString(),
                    "development",
                    ClientPlatformConstants.Android,
                    "0.0.0",
                    "development",
                    UpdateMode.UpToDate
                ),

                new RoutingConfigurationEntry(
                    Guid.NewGuid().ToString(),
                    "production",
                    ClientPlatformConstants.Ios,
                    "0.0.0",
                    "production",
                    UpdateMode.UpToDate
                ),
                new RoutingConfigurationEntry(
                    Guid.NewGuid().ToString(),
                    "production",
                    ClientPlatformConstants.Android,
                    "0.0.0",
                    "production",
                    UpdateMode.UpToDate
                ),
            }
        );
    }
}