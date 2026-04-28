using Router.Core.Model.Configuration;

namespace Router.Executable.Admin.Models;

public class RouteTargetConfigurationWrapper(RouteTargetConfiguration configuration)
{
    public RouteTargetConfiguration Configuration { get; } = configuration;
}