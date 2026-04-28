using Router.Core.Model.Configuration;

namespace Router.Core.Model
{
    public class SetRoutingConfigurationRequest
    {
        public RoutingConfiguration Configuration { get; }
        
        public SetRoutingConfigurationRequest(RoutingConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}