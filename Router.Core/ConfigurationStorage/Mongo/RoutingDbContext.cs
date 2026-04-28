using System;
using MongoDB.Driver;

namespace Router.Core.ConfigurationStorage.Mongo
{
    public class RoutingDbContext
    {
        public IMongoCollection<RoutingEntity> Routes { get; }
        public IMongoCollection<RouteTargetEntity> RouteTargets { get; }
        
        public RoutingDbContext(string connectionString)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);

            settings.RetryWrites = true;
            settings.RetryReads = true;
            settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(15);
            settings.MaxConnectionLifeTime = TimeSpan.FromSeconds(20);
            
            var client = new MongoClient(settings);
            var database = client.GetDatabase("routing");

            Routes = database.GetCollection<RoutingEntity>(nameof(Routes));
            RouteTargets = database.GetCollection<RouteTargetEntity>(nameof(RouteTargets));
        }
    }
}