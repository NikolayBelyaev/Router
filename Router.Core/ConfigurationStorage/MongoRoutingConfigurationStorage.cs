using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Router.Core.ConfigurationStorage.Mongo;
using Router.Core.Model.Configuration;

namespace Router.Core.ConfigurationStorage
{
    public class MongoRoutingConfigurationStorage : IRoutingConfigurationStorage
    {
        private readonly RoutingDbContext _dbContext;

        public MongoRoutingConfigurationStorage(RoutingDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<RoutingConfiguration> GetRoutingConfiguration()
        {
            var entities = await _dbContext.Routes
                .AsQueryable()
                .ToListAsync();

            var entries = entities.Select(FromEntity).ToArray();
            
            return new RoutingConfiguration(entries);
        }

        public async Task SetRoutingConfiguration(RoutingConfiguration configuration)
        {
            var entities = configuration.Entries.Select(ToEntity).ToArray();

            await _dbContext.Routes.DeleteManyAsync(x => true);
            await _dbContext.Routes.InsertManyAsync(entities);
        }

        public async Task<RouteTargetConfiguration> GetRouteTargetConfiguration()
        {
            var entities = await _dbContext.RouteTargets
                .AsQueryable()
                .ToListAsync();

            var entries = entities.Select(FromEntity).ToArray();
            
            return new RouteTargetConfiguration(entries);
        }

        public async Task SetRouteTargetConfiguration(RouteTargetConfiguration configuration)
        {
            var entities = configuration.Entries.Select(ToEntity).ToArray();

            await _dbContext.RouteTargets.DeleteManyAsync(x => true);
            await _dbContext.RouteTargets.InsertManyAsync(entities);
        }

        public async Task BeginServerMaintenance(string target)
        {
            await _dbContext.RouteTargets.UpdateManyAsync(
                Builders<RouteTargetEntity>.Filter.Eq(x => x.Target, target),
                Builders<RouteTargetEntity>.Update.Set(x => x.Maintenance, true)
            );
        }

        public async Task FinishServerMaintenance(string target)
        {
            await _dbContext.RouteTargets.UpdateManyAsync(
                Builders<RouteTargetEntity>.Filter.Eq(x => x.Target, target),
                Builders<RouteTargetEntity>.Update.Set(x => x.Maintenance, false)
            );
        }

        public async Task<RouteTargetConfigurationEntry> GetRouteTarget(string target)
        {
            var query = await _dbContext.RouteTargets
                .AsQueryable()
                .Where(x => x.Target == target)
                .ToListAsync();

            var entity = query.First();

            return FromEntity(entity);
        }

        public async Task<RoutingConfigurationEntry[]> GetRouting(string serverType)
        {
            var query = await _dbContext.Routes
                .AsQueryable()
                .Where(x => x.Server == serverType)
                .ToListAsync();

            return query.Select(FromEntity).ToArray();
        }

        private static RoutingConfigurationEntry FromEntity(RoutingEntity entity)
        {
            return new RoutingConfigurationEntry(
                entity.Id.ToString(),
                entity.Server,
                entity.Platform,
                entity.ClientVersion,
                entity.RouteTarget,
                entity.UpdateMode
            );
        }
        
        private static RoutingEntity ToEntity(RoutingConfigurationEntry entry)
        {
            return new RoutingEntity(
                entry.Server,
                entry.Platform,
                entry.ClientVersion,
                entry.RouteTarget,
                entry.UpdateMode
            );

        }
        
        private static RouteTargetConfigurationEntry FromEntity(RouteTargetEntity entity)
        {
            return new RouteTargetConfigurationEntry(
                entity.Target,
                entity.Address,
                entity.Maintenance
            );
        }
        
        private static RouteTargetEntity ToEntity(RouteTargetConfigurationEntry entry)
        {
            return new RouteTargetEntity(
                entry.Target,
                entry.Address,
                entry.Maintenance
            );
        }

        public async Task<string> AddRoutingRule(RoutingConfigurationEntry entry)
        {
            var entity = new RoutingEntity(entry.Server, entry.Platform, entry.ClientVersion, entry.RouteTarget, entry.UpdateMode);
            await _dbContext.Routes.InsertOneAsync(entity);
            return entity.Id.ToString();
        }

        public async Task UpdateRoutingRule(string id, RoutingConfigurationEntry entry)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(id);
            var update = Builders<RoutingEntity>.Update
                .Set(x => x.Server, entry.Server)
                .Set(x => x.Platform, entry.Platform)
                .Set(x => x.ClientVersion, entry.ClientVersion)
                .Set(x => x.RouteTarget, entry.RouteTarget)
                .Set(x => x.UpdateMode, entry.UpdateMode);
            await _dbContext.Routes.UpdateOneAsync(
                Builders<RoutingEntity>.Filter.Eq(x => x.Id, objectId),
                update
            );
        }

        public async Task DeleteRoutingRule(string id)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(id);
            await _dbContext.Routes.DeleteOneAsync(
                Builders<RoutingEntity>.Filter.Eq(x => x.Id, objectId)
            );
        }

        public async Task AddRouteTarget(RouteTargetConfigurationEntry entry)
        {
            await _dbContext.RouteTargets.InsertOneAsync(ToEntity(entry));
        }

        public async Task UpdateRouteTarget(string oldTarget, RouteTargetConfigurationEntry entry)
        {
            var update = Builders<RouteTargetEntity>.Update
                .Set(x => x.Target, entry.Target)
                .Set(x => x.Address, entry.Address)
                .Set(x => x.Maintenance, entry.Maintenance);
            await _dbContext.RouteTargets.UpdateOneAsync(
                Builders<RouteTargetEntity>.Filter.Eq(x => x.Target, oldTarget),
                update
            );
        }

        public async Task DeleteRouteTarget(string target)
        {
            await _dbContext.RouteTargets.DeleteOneAsync(
                Builders<RouteTargetEntity>.Filter.Eq(x => x.Target, target)
            );
        }
    }
}