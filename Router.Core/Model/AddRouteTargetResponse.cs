using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Router.Core.Model.Configuration;

namespace Router.Core.Model
{
    public class AddRouteTargetResponse
    {
        public enum ReturnCode
        {
            Ok,
            BadRequest,
            Conflict,
            InternalServerError
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public ReturnCode Code { get; }

        public RouteTargetConfigurationEntry Entry { get; }

        public AddRouteTargetResponse(ReturnCode code, RouteTargetConfigurationEntry entry = null)
        {
            Code = code;
            Entry = entry;
        }
    }
}
