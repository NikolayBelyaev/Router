using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Router.Core.Model
{
    public class DeleteRouteTargetResponse
    {
        public enum ReturnCode
        {
            Ok,
            BadRequest,
            NotFound,
            InternalServerError
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public ReturnCode Code { get; }

        public DeleteRouteTargetResponse(ReturnCode code)
        {
            Code = code;
        }
    }
}
