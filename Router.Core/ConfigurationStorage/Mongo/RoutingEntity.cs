using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Router.Core.Model.Configuration;

namespace Router.Core.ConfigurationStorage.Mongo
{
    public class RoutingEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        
        public string Server { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)] 
        public string Platform { get; set; }
        
        public string ClientVersion { get; set; }
        
        public string RouteTarget { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)] 
        public UpdateMode UpdateMode { get; set; }
        
        public RoutingEntity()
        {
        }
        
        public RoutingEntity(
            string server, 
            string platform, 
            string clientVersion, 
            string routeTarget, 
            UpdateMode updateMode
        )
        {
            Server = server;
            Platform = platform;
            ClientVersion = clientVersion;
            RouteTarget = routeTarget;
            UpdateMode = updateMode;
        }
    }
}