using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Router.Shared.Router;

namespace Router.Core.Model.Configuration
{
    public class RoutingConfigurationEntry
    {
        public string Id { get; }

        [JsonConverter(typeof(LowercaseStringConverter))]
        public string Server { get; }

        [JsonConverter(typeof(LowercaseStringConverter))]
        public string Platform { get; }

        [JsonConverter(typeof(LowercaseStringConverter))]
        public string ClientVersion { get; }

        [JsonConverter(typeof(LowercaseStringConverter))]
        public string RouteTarget { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UpdateMode UpdateMode { get; }

        public RoutingConfigurationEntry(
            string id,
            string server,
            string platform,
            string clientVersion,
            string routeTarget,
            UpdateMode updateMode
        )
        {
            Id = id;
            Server = server;
            Platform = platform;
            ClientVersion = clientVersion;
            RouteTarget = routeTarget;
            UpdateMode = updateMode;
        }
    }
}