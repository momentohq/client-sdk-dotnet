using Amazon.Lambda.Core;
using StackExchange.Redis;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DynamoLambdaExampleHandler1;

public class Function
{
    public async Task<string> FunctionHandler(ILambdaContext context)
    {
        await new Function().DoStuff();
        return "Task completed";
    }

    static byte[] GenerateLargeValue(int sizeInBytes)
    {
        byte[] value = new byte[sizeInBytes];
        new Random().NextBytes(value);
        return value;
    }

    public async Task<string> DoStuff() {

        // Create a Redis connection
        Console.WriteLine("Creating redis client");
        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = { "tacobellrediscluster.exmof5.ng.0001.usw2.cache.amazonaws.com:6379" }
        };
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configurationOptions);

        // Get a Redis database
        Console.WriteLine("Fetching db");
        IDatabase db = redis.GetDatabase();

        // Ping the redis server
        Console.WriteLine("Pinging db");
        var pong = db.Ping();
        Console.WriteLine(pong);

        // Set 20 string keys with string values
        for (int i = 1; i <= 20; i++)
        {
            string key = "key" + i;
            // string value = "value" + i;
            byte[] value = GenerateLargeValue(1 * 1024 * 1024); // 1MB
            bool success = db.StringSet(key, value);

            if (success)
            {
                Console.WriteLine($"Key '{key}' set with a value of 1MB");

            }
            else
            {
                Console.WriteLine($"Failed to set key '{key}'");
            }
        }

        var tasks = new Dictionary<Task<RedisValue>, System.Diagnostics.Stopwatch>();

        // Get values of 10 keys asynchronously
        for (int i = 1; i <= 10; i++)
        {
            string key = "key" + i;

            Console.WriteLine("Starting the stopwatch");
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            var response = db.StringGetAsync(key);
            
            tasks[response] = startTime;
            response.ContinueWith(
                (r) => {
                    tasks[response].Stop();
                    return r;
                }
            );
            Console.WriteLine("Stopping stopwatch");
        }

        Console.WriteLine("Awaiting pending tasks...");
        Task.WaitAll(tasks.Keys.ToArray());
        Console.WriteLine("Done awaiting tasks");

        foreach(var entry in tasks)
        {
            Console.WriteLine(entry.Value.ElapsedMilliseconds);
        }

        // Close the connection
        redis.Close();

        return "Task completed!";
    }


    static void Main(string[] args) {
        try {
            Console.WriteLine("Calling DoStuff");
            new Function().DoStuff();
        } catch(Exception ex) {
            Console.WriteLine("Caught Exception");
            Console.WriteLine(ex);
        }
    }

}