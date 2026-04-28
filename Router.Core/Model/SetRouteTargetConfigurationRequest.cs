using Router.Core.Model.Configuration;

namespace Router.Core.Model
{
    public class SetRouteTargetConfigurationRequest
    {
        public RouteTargetConfiguration Configuration { get; }
        
        public SetRouteTargetConfigurationRequest(RouteTargetConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}