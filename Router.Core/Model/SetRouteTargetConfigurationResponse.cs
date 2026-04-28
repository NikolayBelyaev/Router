using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Router.Core.Model.Configuration;

namespace Router.Core.Model
{
    public class SetRouteTargetConfigurationResponse
    {
        public enum ReturnCode
        {
            Ok,
            BadRequest,
            BadConfiguration,
            InternalServerError
        }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ReturnCode Code { get; }
        
        public RouteTargetConfiguration Configuration { get; }
        
        public SetRouteTargetConfigurationResponse(ReturnCode code, RouteTargetConfiguration configuration)
        {
            Code = code;
            Configuration = configuration;
        }
    }
}