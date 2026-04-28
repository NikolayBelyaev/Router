using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Router.Core.Model.Configuration;

namespace Router.Core.Model
{
    public class UpdateRouteTargetResponse
    {
        public enum ReturnCode
        {
            Ok,
            BadRequest,
            NotFound,
            Conflict,
            InternalServerError
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public ReturnCode Code { get; }

        public RouteTargetConfigurationEntry Entry { get; }

        public UpdateRouteTargetResponse(ReturnCode code, RouteTargetConfigurationEntry entry = null)
        {
            Code = code;
            Entry = entry;
        }
    }
}
