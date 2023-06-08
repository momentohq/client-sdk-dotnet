// using Amazon;
// using Amazon.DynamoDBv2;
// using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
// using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

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

    public async void DoStuff() {

        // Create a Redis connection
        Console.WriteLine("Creating redis client");
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

        // Get a Redis database
        Console.WriteLine("Fetching db");
        IDatabase db = redis.GetDatabase();

        // Ping the redis server
        Console.WriteLine("Pinging db");
        string pong = db.Ping();
        Console.WriteLine(pong);

        // Check the response
        if (pong == "PONG")
        {
            Console.WriteLine("Redis server is running");
        }
        else
        {
            Console.WriteLine("Unable to connect to Redis server");
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
