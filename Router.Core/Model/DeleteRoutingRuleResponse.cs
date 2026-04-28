using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Router.Core.Model
{
    public class DeleteRoutingRuleResponse
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

        public DeleteRoutingRuleResponse(ReturnCode code)
        {
            Code = code;
        }
    }
}
