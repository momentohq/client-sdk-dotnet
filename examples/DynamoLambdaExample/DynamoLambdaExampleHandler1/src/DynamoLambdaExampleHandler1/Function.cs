using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DynamoLambdaExampleHandler1;

public class Function
{
    public async Task<string> FunctionHandler(ILambdaContext context)
    {
        DoStuff();
        return "Task completed";
    }

    public void DoStuff() {

        var client = new AmazonDynamoDBClient();

        var numRequests = 10;
        // var tasks = new Task<GetItemResponse>[numRequests];
        var tasks = new Dictionary<Task<GetItemResponse>, System.Diagnostics.Stopwatch>();

        string tableName = "TacoBellInvestigator";
        string keyAttributeName = "key";
        string keyAttributeBase = "test-key";

        for (int i = 1; i <= numRequests; i++) {
            Console.WriteLine("Getting inside the loop");
            string keyAttributeValue = $"{keyAttributeBase}-{i}";
            var request = new GetItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    {keyAttributeName, new AttributeValue { S = keyAttributeValue}}
                } 
            };

            Console.WriteLine("GetRequest created");
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Starting the stopwatch");
            var response = client.GetItemAsync(request);
            Console.WriteLine(response);
        
            tasks[response] = startTime;
            response.ContinueWith(
                (r) => {
                    tasks[response].Stop();
                    return r;
                }
            );
            Console.WriteLine("Stopped stopwatch");
        }

        Console.WriteLine("Awaiting pending tasks...");
        Task.WaitAll(tasks.Keys.ToArray());
        Console.WriteLine("Done awaiting tasks");

        foreach(var entry in tasks)
        {
            Console.WriteLine(entry.Value.ElapsedMilliseconds);
        }
    }


    static void Main(string[] args) {
        try {
            new Function().DoStuff();
        } catch(Exception ex) {
            Console.WriteLine("Caught Exception");
            Console.WriteLine(ex);
        }
    }

}
