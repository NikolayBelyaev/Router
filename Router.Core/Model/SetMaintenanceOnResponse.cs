using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Router.Core.Model
{
    public class SetMaintenanceOnResponse
    {
        public enum ReturnCode
        {
            Ok,
            BadRequest,
            InternalServerError
        }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ReturnCode Code { get; }
        
        public SetMaintenanceOnResponse(ReturnCode code)
        {
            Code = code;
        }
    }
}