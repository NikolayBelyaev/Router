using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Router.Shared.Router;

namespace Router.Core.Model
{
    public class AddRoutingRuleRequest
    {
        [JsonConverter(typeof(LowercaseStringConverter))]
        public string Server { get; }

        [JsonConverter(typeof(LowercaseStringConverter))]
        public string Platform { get; }

        [JsonConverter(typeof(LowercaseStringConverter))]
        public string ClientVersion { get; }

        [JsonConverter(typeof(LowercaseStringConverter))]
        public string RouteTarget { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Configuration.UpdateMode UpdateMode { get; }

        public AddRoutingRuleRequest(string server, string platform, string clientVersion, string routeTarget, Configuration.UpdateMode updateMode)
        {
            Server = server;
            Platform = platform;
            ClientVersion = clientVersion;
            RouteTarget = routeTarget;
            UpdateMode = updateMode;
        }
    }
}
