using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Kit.Logging.Abstraction;
using Router.Core.ConfigurationStorage.DynamoDb;
using Router.Core.Model.Configuration;

namespace Router.Core.ConfigurationStorage
{
    public class DynamoDbRoutingConfigurationStorage : IRoutingConfigurationStorage
    {
        public const string RoutingConfigurationTable = "router-configuration";
        public const string RouteTargetsTable = "router-targets";

        private readonly AmazonDynamoDBClient _client;
        private readonly DynamoDBContext _context;
        private readonly IKitLogger _logger;

        private readonly string _tablePrefix;
        
        public DynamoDbRoutingConfigurationStorage(
            string awsAccessKeyId,
            string awsSecret, 
            RegionEndpoint endpoint,
            string tablePrefix, 
            IKitLogger logger = null)
        {
            _tablePrefix = tablePrefix;
            _logger = logger;
            var config = new AmazonDynamoDBConfig
            {
                RegionEndpoint = endpoint,
                Timeout = TimeSpan.FromSeconds(15)
            };
            _client = new AmazonDynamoDBClient(
                awsAccessKeyId, 
                awsSecret, 
                config
            );
            _context = new DynamoDBContext(_client, new DynamoDBContextConfig { TableNamePrefix = _tablePrefix });
        }
        
        public async Task<RoutingConfiguration> GetRoutingConfiguration()
        {
            try
            {
                await EnsureRoutingConfigurationTableCreated(_tablePrefix);

                var search = _context.ScanAsync<RoutingDocument>(RoutingDocument.SearchAll());
                var documents = await search.GetRemainingAsync();
                var routes = documents
                    .Select(FromDocument)
                    .OrderBy(x => x.Server)
                    .ThenBy(x =>
                        int.Parse(x.ClientVersion.Substring(x.ClientVersion.LastIndexOf(".", StringComparison.Ordinal) +
                                                            1)))
                    .ThenBy(x => x.Platform)
                    .ToArray();

                return new RoutingConfiguration(routes);
            }
            catch (Exception e)
            {
                _logger?.Error($"[DynamoDbRoutingConfigurationStorage.GetRoutingConfiguration] Exception: {e}");
                return null;
            }
        }

        public async Task SetRoutingConfiguration(RoutingConfiguration configuration)
        {
            await EnsureRoutingConfigurationTableCreated(_tablePrefix);
            
            var search = _context.ScanAsync<RoutingDocument>(RoutingDocument.SearchAll());
            var documents = await search.GetRemainingAsync();
            
            foreach (var document in documents)
                await _context.DeleteAsync(document);

            var nextDocuments = configuration.Entries.Select(ToDocument).ToArray();

            foreach (var nextDocument in nextDocuments)
            {
                nextDocument.Server = nextDocument.Server.ToLower();
                nextDocument.RouteTarget = nextDocument.RouteTarget.ToLower();
                await _context.SaveAsync(nextDocument);
            }
        }

        public async Task<RouteTargetConfiguration> GetRouteTargetConfiguration()
        {
            try
            {
                await EnsureRouteTargetsTableCreated(_tablePrefix);

                var search = _context.ScanAsync<RouteTargetDocument>(RouteTargetDocument.SearchAll());
                var documents = await search.GetRemainingAsync();
                var targets = documents
                    .Select(FromDocument)
                    .OrderBy(x => x.Target)
                    .ToArray();

                return new RouteTargetConfiguration(targets);
            }
            catch (Exception e)
            {
                _logger?.Error($"[DynamoDbRoutingConfigurationStorage.GetRouteTargetConfiguration] Exception: {e}");
                return null;
            }
        }

        public async Task SetRouteTargetConfiguration(RouteTargetConfiguration configuration)
        {
            await EnsureRouteTargetsTableCreated(_tablePrefix);
            
            var search = _context.ScanAsync<RouteTargetDocument>(RouteTargetDocument.SearchAll());
            var documents = await search.GetRemainingAsync();

            foreach (var document in documents)
                await _context.DeleteAsync(document);

            var nextDocuments = configuration.Entries.Select(ToDocument).ToArray();

            foreach (var nextDocument in nextDocuments)
            {
                var convertedNextDocument = new RouteTargetDocument(
                    nextDocument.Target.ToLower(),
                    nextDocument.Address,
                    nextDocument.Maintenance);
                await _context.SaveAsync(convertedNextDocument);
            }
        }

        public async Task BeginServerMaintenance(string target)
        {
            await EnsureRouteTargetsTableCreated(_tablePrefix);
            
            var search = _context.ScanAsync<RouteTargetDocument>(RouteTargetDocument.SearchRouteTarget(target));
            var documents = await search.GetRemainingAsync();
            var targetDocument = documents.First();

            targetDocument.Maintenance = true;

            await _context.SaveAsync(targetDocument);
        }

        public async Task FinishServerMaintenance(string target)
        {
            await EnsureRouteTargetsTableCreated(_tablePrefix);
            
            var search = _context.ScanAsync<RouteTargetDocument>(RouteTargetDocument.SearchRouteTarget(target));
            var documents = await search.GetRemainingAsync();
            var targetDocument = documents.First();

            targetDocument.Maintenance = false;

            await _context.SaveAsync(targetDocument);
        }

        public async Task<RouteTargetConfigurationEntry> GetRouteTarget(string target)
        {
            await EnsureRouteTargetsTableCreated(_tablePrefix);
            
            var search = _context.ScanAsync<RouteTargetDocument>(RouteTargetDocument.SearchRouteTarget(target));
            var documents = await search.GetRemainingAsync();
            var targetDocument = documents.First();

            return FromDocument(targetDocument);
        }

        public async Task<RoutingConfigurationEntry[]> GetRouting(string serverType)
        {
            await EnsureRoutingConfigurationTableCreated(_tablePrefix);
            
            var search = _context.ScanAsync<RoutingDocument>(RoutingDocument.SearchServerType(serverType));
            var documents = await search.GetRemainingAsync();

            return documents
                .Select(FromDocument)
                .ToArray();
        }

        private async Task EnsureRoutingConfigurationTableCreated(string tablePrefix)
        {
            var name = $"{_tablePrefix}{RoutingConfigurationTable}";
            var tablesList = await _client.ListTablesAsync();
            if (tablesList.TableNames.Contains(name))
                return;

            await _client.CreateTableAsync(RoutingDocument.CreateRequest(tablePrefix));
        }
        
        private async Task EnsureRouteTargetsTableCreated(string tablePrefix)
        {
            var name = $"{_tablePrefix}{RouteTargetsTable}";
            var tablesList = await _client.ListTablesAsync();
            if (tablesList.TableNames.Contains(name))
                return;

            await _client.CreateTableAsync(RouteTargetDocument.CreateRequest(tablePrefix));
        }

        private static RouteTargetConfigurationEntry FromDocument(RouteTargetDocument arg)
        {
            return new RouteTargetConfigurationEntry(
                arg.Target,
                arg.Address,
                arg.Maintenance
            );
        }

        private static RouteTargetDocument ToDocument(RouteTargetConfigurationEntry arg)
        {
            return new RouteTargetDocument(
                arg.Target,
                arg.Address,
                arg.Maintenance
            );
        }
        
        public async Task<string> AddRoutingRule(RoutingConfigurationEntry entry)
        {
            await EnsureRoutingConfigurationTableCreated(_tablePrefix);

            var doc = new RoutingDocument(
                entry.Server.ToLower(),
                entry.Platform.ToLower(),
                entry.ClientVersion.ToLower(),
                entry.RouteTarget.ToLower(),
                entry.UpdateMode
            );

            await _context.SaveAsync(doc);

            return doc.Key;
        }

        public async Task UpdateRoutingRule(string id, RoutingConfigurationEntry entry)
        {
            await EnsureRoutingConfigurationTableCreated(_tablePrefix);

            var search = _context.ScanAsync<RoutingDocument>(RoutingDocument.SearchByKey(id));
            var documents = await search.GetRemainingAsync();
            var doc = documents.First();

            doc.Server = entry.Server.ToLower();
            doc.Platform = entry.Platform.ToLower();
            doc.ClientVersion = entry.ClientVersion.ToLower();
            doc.RouteTarget = entry.RouteTarget.ToLower();
            doc.UpdateMode = entry.UpdateMode;

            await _context.SaveAsync(doc);
        }

        public async Task DeleteRoutingRule(string id)
        {
            await EnsureRoutingConfigurationTableCreated(_tablePrefix);

            var search = _context.ScanAsync<RoutingDocument>(RoutingDocument.SearchByKey(id));
            var documents = await search.GetRemainingAsync();
            var doc = documents.First();

            await _context.DeleteAsync(doc);
        }

        public async Task AddRouteTarget(RouteTargetConfigurationEntry entry)
        {
            await EnsureRouteTargetsTableCreated(_tablePrefix);

            var doc = new RouteTargetDocument(
                entry.Target.ToLower(),
                entry.Address,
                entry.Maintenance
            );

            await _context.SaveAsync(doc);
        }

        public async Task UpdateRouteTarget(string oldTarget, RouteTargetConfigurationEntry entry)
        {
            await EnsureRouteTargetsTableCreated(_tablePrefix);

            var oldDoc = await _context.LoadAsync<RouteTargetDocument>(oldTarget);

            if (oldTarget != entry.Target.ToLower())
            {
                await _context.DeleteAsync(oldDoc);
                var newDoc = new RouteTargetDocument(entry.Target.ToLower(), entry.Address, entry.Maintenance);
                await _context.SaveAsync(newDoc);
            }
            else
            {
                oldDoc.Address = entry.Address;
                oldDoc.Maintenance = entry.Maintenance;
                await _context.SaveAsync(oldDoc);
            }
        }

        public async Task DeleteRouteTarget(string target)
        {
            await EnsureRouteTargetsTableCreated(_tablePrefix);

            var doc = await _context.LoadAsync<RouteTargetDocument>(target);
            await _context.DeleteAsync(doc);
        }

        private static RoutingConfigurationEntry FromDocument(RoutingDocument arg)
        {
            return new RoutingConfigurationEntry(
                arg.Key,
                arg.Server,
                arg.Platform,
                arg.ClientVersion,
                arg.RouteTarget,
                arg.UpdateMode
            );
        }

        private static RoutingDocument ToDocument(RoutingConfigurationEntry arg)
        {
            return new RoutingDocument(
                arg.Server,
                arg.Platform,
                arg.ClientVersion,
                arg.RouteTarget,
                arg.UpdateMode
            );
        }
    }
}