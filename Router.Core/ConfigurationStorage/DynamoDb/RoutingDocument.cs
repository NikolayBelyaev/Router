using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Router.Core.Model.Configuration;

namespace Router.Core.ConfigurationStorage.DynamoDb
{
    [DynamoDBTable(DynamoDbRoutingConfigurationStorage.RoutingConfigurationTable)]
    public class RoutingDocument
    {
        [DynamoDBHashKey]
        public string Key { get; set; }

        public string Server { get; set; }
        public string Platform { get; set; }
        public string ClientVersion { get; set; }
        public string RouteTarget { get; set; }
        public UpdateMode UpdateMode { get; set; }

        public RoutingDocument()
        {
            Key = Guid.NewGuid().ToString();
        }

        public RoutingDocument(
            string server,
            string platform,
            string clientVersion,
            string routeTarget,
            UpdateMode updateMode
        ) : this()
        {
            Server = server;
            Platform = platform;
            ClientVersion = clientVersion;
            RouteTarget = routeTarget;
            UpdateMode = updateMode;
        }
        
        public static CreateTableRequest CreateRequest(string tablePrefix) => new CreateTableRequest
        {
            TableName = $"{tablePrefix}{DynamoDbRoutingConfigurationStorage.RoutingConfigurationTable}",
            GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>(),
            LocalSecondaryIndexes = new List<LocalSecondaryIndex>(),
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 60,
                WriteCapacityUnits = 5
            },
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new AttributeDefinition
                {
                    AttributeName = nameof(Key),
                    AttributeType = ScalarAttributeType.S
                }
            },
            KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = nameof(Key),
                    KeyType = KeyType.HASH
                }
            }
        };
        
        public static ScanCondition[] SearchAll() => new[]
        {
            new ScanCondition(
                nameof(Server),
                ScanOperator.NotEqual,
                "None"
            )
        };

        public static ScanCondition[] SearchByKey(string key) => new[]
        {
            new ScanCondition(
                nameof(Key),
                ScanOperator.Equal,
                key
            )
        };
        
        public static ScanCondition[] SearchServerType(string serverType) => new[]
        {
            new ScanCondition(
                nameof(Server),
                ScanOperator.Equal,
                serverType
            )
        };
    }
}