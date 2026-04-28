using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace Router.Core.ConfigurationStorage.DynamoDb
{
    [DynamoDBTable(DynamoDbRoutingConfigurationStorage.RouteTargetsTable)]
    public class RouteTargetDocument
    {
        [DynamoDBHashKey]
        public string Target { get; set; }
        
        public string Address { get; set; }
        public bool Maintenance { get; set; }
        
        public RouteTargetDocument()
        {
        }
        
        public RouteTargetDocument(string target, string address, bool maintenance)
        {
            Target = target;
            Address = address;
            Maintenance = maintenance;
        }

        public static CreateTableRequest CreateRequest(string tablePrefix) => new CreateTableRequest
        {
            TableName = $"{tablePrefix}{DynamoDbRoutingConfigurationStorage.RouteTargetsTable}",
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
                    AttributeName = nameof(Target),
                    AttributeType = ScalarAttributeType.S
                }
            },
            KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = nameof(Target),
                    KeyType = KeyType.HASH
                }
            }
        };

        public static ScanCondition[] SearchAll() => new[]
        {
            new ScanCondition(
                nameof(Target),
                ScanOperator.NotEqual,
                "None"
            )
        };
        
        public static ScanCondition[] SearchRouteTarget(string target) => new[]
        {
            new ScanCondition(
                nameof(Target),
                ScanOperator.Equal,
                target
            )
        };
    }
}