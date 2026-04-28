using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Router.Core.ConfigurationStorage.Mongo
{
    public class RouteTargetEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        
        public string Target { get; set; }
        
        public string Address { get; set; }
        
        public bool Maintenance { get; set; }
        
        public RouteTargetEntity()
        {
        }
        
        public RouteTargetEntity(string target, string address, bool maintenance)
        {
            Target = target;
            Address = address;
            Maintenance = maintenance;
        }
    }
}