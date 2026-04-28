using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Router.Core.Model
{
    public class SetMaintenanceOffResponse
    {
        public enum ReturnCode
        {
            Ok,
            BadRequest,
            InternalServerError
        }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ReturnCode Code { get; }
        
        public SetMaintenanceOffResponse(ReturnCode code)
        {
            Code = code;
        }
    }
}