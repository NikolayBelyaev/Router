using Router.Core.Model.Configuration;

namespace Router.Executable.Admin.Models;

public class RoutingConfigurationWrapper(RoutingConfiguration configuration)
{
    public RoutingConfiguration Configuration { get; } = configuration;
}