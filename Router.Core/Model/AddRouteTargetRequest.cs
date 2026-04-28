using Newtonsoft.Json;
using Router.Shared.Router;

namespace Router.Core.Model
{
    public class AddRouteTargetRequest
    {
        [JsonConverter(typeof(LowercaseStringConverter))]
        public string Target { get; }

        public string Address { get; }

        public bool Maintenance { get; }

        public AddRouteTargetRequest(string target, string address, bool maintenance)
        {
            Target = target;
            Address = address;
            Maintenance = maintenance;
        }
    }
}
