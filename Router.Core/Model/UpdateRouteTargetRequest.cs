using Newtonsoft.Json;
using Router.Shared.Router;

namespace Router.Core.Model
{
    public class UpdateRouteTargetRequest
    {
        public string Id { get; }

        [JsonConverter(typeof(LowercaseStringConverter))]
        public string Target { get; }

        public string Address { get; }

        public bool Maintenance { get; }

        public UpdateRouteTargetRequest(string id, string target, string address, bool maintenance)
        {
            Id = id;
            Target = target;
            Address = address;
            Maintenance = maintenance;
        }
    }
}
