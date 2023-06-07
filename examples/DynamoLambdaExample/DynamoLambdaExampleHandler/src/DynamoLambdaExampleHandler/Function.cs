using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DynamoLambdaExampleHandler
{
    public class Function
    {
        public async Task<string> FunctionHandler(ILambdaContext context)
        {
            var client = new AmazonDynamoDBClient();

            // Set up the item details
            string tableName = "TacoBellInvestigator";
            string keyAttributeName = "key";
            string valueAttributeName = "value";

            string keyAttributeBase = "test-key";
            string valueAttributeBase = "test-value"; 

            for (int i = 1; i <= 20; i++)
                {
                    string keyAttributeValue = $"{keyAttributeBase}-{i}";
                    string valueAttributeValue = $"{valueAttributeBase}-{i}";

                    // Create the item request
                    var request = new PutItemRequest
                    {
                        TableName = tableName,
                        Item = new Dictionary<string, AttributeValue>
                        {
                            { keyAttributeName, new AttributeValue { S = keyAttributeValue } },
                            { valueAttributeName, new AttributeValue { S = valueAttributeValue } },
                        }
                    };

                    // Put the item in the table
                    await client.PutItemAsync(request);
                }
            
            client.Dispose();
            return "Item created successfully";
        }
    }
}
