using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Router.Core.Model.Configuration;

namespace Router.Core.Model
{
    public class GetRouteTargetConfigurationResponse
    {
        public enum ReturnCode
        {
            Ok,
            BadRequest,
            InternalServerError
        }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ReturnCode Code { get; }
        
        public RouteTargetConfiguration Configuration { get; }
        
        public GetRouteTargetConfigurationResponse(ReturnCode code, RouteTargetConfiguration configuration)
        {
            Code = code;
            Configuration = configuration;
        }
    }
}