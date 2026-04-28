using Newtonsoft.Json;
using Router.Shared.Router;

namespace Router.Core.Model.Configuration
{
    public class RouteTargetConfigurationEntry
    {
        [JsonConverter(typeof(LowercaseStringConverter))]
        public string Target { get; }
        
        [JsonConverter(typeof(LowercaseStringConverter))]
        public string Address { get; }
        
        public bool Maintenance { get; }
        
        public RouteTargetConfigurationEntry(string target, string address, bool maintenance)
        {
            Target = target;
            Address = address;
            Maintenance = maintenance;
        }
    }
}