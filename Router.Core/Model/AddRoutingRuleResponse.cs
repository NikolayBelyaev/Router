using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Router.Core.Model.Configuration;

namespace Router.Core.Model
{
    public class AddRoutingRuleResponse
    {
        public enum ReturnCode
        {
            Ok,
            BadRequest,
            Conflict,
            BadConfiguration,
            InternalServerError
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public ReturnCode Code { get; }

        public string Id { get; }

        public RoutingConfigurationEntry Entry { get; }

        public AddRoutingRuleResponse(ReturnCode code, string id = null, RoutingConfigurationEntry entry = null)
        {
            Code = code;
            Id = id;
            Entry = entry;
        }
    }
}
