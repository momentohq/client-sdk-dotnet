using Amazon.Lambda.Core;
using StackExchange.Redis;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;
using System;
using System.IO;
using System.IO.Compression;
using Momento.Sdk.Responses;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MomentoRedisExampleLambdaHandler;

public class Function
{
    const string CACHE_NAME = "MomentoLambda";
    const string REDIS_CLUSTER_ENDPOINT = "momentoredisexamplecluster.exmof5.ng.0001.usw2.cache.amazonaws.com:6379";
    public string bigString = new string('x', 1024*1024/2);

    public bool COMPRESS_VALUES = true;
    public int numMomentoGetRequests = 10;
    public int numMomentoSetRequests = 10;
    public int numRedisGetRequests = 10;
    public int numRedisSetRequests = 10;
    // public Dictionary<Task<CacheSetResponse>, System.Diagnostics.Stopwatch> momentoSetTasks = new Dictionary<Task<CacheSetResponse>, System.Diagnostics.Stopwatch>();
    // public Dictionary<Task<CacheGetResponse>, System.Diagnostics.Stopwatch> momentoGetTasks = new Dictionary<Task<CacheGetResponse>, System.Diagnostics.Stopwatch>();
    public Dictionary<Task<bool>, System.Diagnostics.Stopwatch> redisSetTasks = new Dictionary<Task<bool>, System.Diagnostics.Stopwatch>();
    public Dictionary<Task<RedisValue>, System.Diagnostics.Stopwatch> redisGetTasks = new Dictionary<Task<RedisValue>, System.Diagnostics.Stopwatch>();

    public async Task<string> FunctionHandler(ILambdaContext context)
    {
        await new Function().DoStuff();
        return "Task completed";
    }

    public async Task<string> DoStuff() {

        Console.WriteLine("Start executing DoStuff");
        // var lambdaStartTime = System.Diagnostics.Stopwatch.StartNew();

        // Create Momento client
        ICredentialProvider authProvider = new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN");
        var momentoClient = new CacheClient(
            Configurations.InRegion.Lambda.Latest().WithClientTimeout(TimeSpan.FromSeconds(3000)),
            authProvider,
            TimeSpan.FromSeconds(30)
        );

        // Create cache if not already exists
        createMomentoCache(momentoClient);

        // Create a Redis connection
        // Console.WriteLine("Creating redis client");
        // var configurationOptions = new ConfigurationOptions
        // {
        //     EndPoints = { REDIS_CLUSTER_ENDPOINT }
        // };
        // ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configurationOptions);

        // Get a Redis database
        // Console.WriteLine("Fetching db");
        // IDatabase db = redis.GetDatabase();

        // var startKeyId = 1;
        
        // while (lambdaStartTime.Elapsed.TotalMinutes < 4) {
            // Fire momento and redis requests
            var momentoSetTasks = await DoMomentoSets(momentoClient);
            Console.WriteLine("momentoSetTasks.Count " + momentoSetTasks.Count);
            var momentoGetTasks = DoMomentoGets(momentoClient);
            Console.WriteLine("momentoGetTasks.Count " + momentoGetTasks.Count);
            // DoRedisSets(db, startKeyId);
            // DoRedisGets(db, startKeyId);

            Console.WriteLine("Awaiting pending tasks...");
            Task.WaitAll(momentoSetTasks.Keys.ToArray());
            Task.WaitAll(momentoGetTasks.Keys.ToArray());
            // Task.WaitAll(redisSetTasks.Keys.ToArray());
            // Task.WaitAll(redisGetTasks.Keys.ToArray());
            Console.WriteLine("Done awaiting tasks");

            // print momento set tasks
            Console.WriteLine("------- MOMENTO SET RESULT -------");
            foreach(var entry in momentoSetTasks) {
                var key = await entry.Key;
                if (key is CacheSetResponse.Error err) {
                   Console.WriteLine("ERROR: " + err.Message);
               } else if (key is CacheSetResponse.Success hit) {
                   Console.WriteLine("Set in " + entry.Value.ElapsedMilliseconds);
               }
            }

            // print momento get tasks
            Console.WriteLine("------- MOMENTO GET RESULT -------");
            foreach(var entry in momentoGetTasks)
            {
                var key = await entry.Key;
                if (key is CacheGetResponse.Miss) {
                    Console.WriteLine("Unexpected MISS in " + entry.Value.ElapsedMilliseconds);
                } else if (key is CacheGetResponse.Error err) {
                    Console.WriteLine("ERROR: " + err.Message);
                } else if (key is CacheGetResponse.Hit hit) {
                    Console.WriteLine("Got " + hit.ValueString.Length + " in " + entry.Value.ElapsedMilliseconds);
                }
            }

            // print redis set tasks
            // Console.WriteLine("------- REDIS SET RESULT -------");
            // foreach(var entry in redisSetTasks)
            // {
            //     Console.WriteLine(entry.Value.ElapsedMilliseconds);
            // }

            // print redis get tasks
            // Console.WriteLine("------- REDIS GET RESULT -------");
            // foreach(var entry in redisGetTasks)
            // {
            //     Console.WriteLine(entry.Value.ElapsedMilliseconds);
            // }

            // startKeyId = startKeyId + numRedisSetRequests;
        // }
        
        // Close the connection
        // redis.Close();
        
        Console.WriteLine("End executing DoStuff");

        return "Task completed!";
    }

    private async Task createMomentoCache(CacheClient client) {
        var response = await client.CreateCacheAsync(CACHE_NAME);
    }
    
    private async Task<Dictionary<Task<CacheSetResponse>, System.Diagnostics.Stopwatch>> DoMomentoSets(CacheClient client) {
        try {

            Dictionary<Task<CacheSetResponse>, System.Diagnostics.Stopwatch> momentoSetTasks = new Dictionary<Task<CacheSetResponse>, System.Diagnostics.Stopwatch>();

            Console.WriteLine("Executing MomentoSets");
            string compressedString = Compress();

            for (int i = 1; i <= numMomentoSetRequests; i++) {
                Console.WriteLine("Setting inside the loop");
                string key = $"key-{i}";

                Console.WriteLine("Starting the stopwatch");
                var stopWatch = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine("Setting key " + key);
                Task<CacheSetResponse> response = client.SetAsync(CACHE_NAME, key, compressedString, TimeSpan.FromSeconds(600));
                // momentoSetTasks[response] = startTime;
                var continued = await response.ContinueWith(
                    (r) => {
                        try {
                            stopWatch.Stop();
                            Console.WriteLine("Stopped stopwatch");
                            return r;
                        } catch(Exception e) {
                            Console.WriteLine("Exception in set continuedWith  " + e);
                            throw e;
                        }
                        
                    }
                );
                momentoSetTasks[continued] = stopWatch;
            }
            return momentoSetTasks;
        } catch (Exception e) {
            Console.WriteLine("Exception in set " + e);
            throw e;
        }
        
    }

    private Dictionary<Task<CacheGetResponse>, System.Diagnostics.Stopwatch> DoMomentoGets(CacheClient client) {
        Dictionary<Task<CacheGetResponse>, System.Diagnostics.Stopwatch> momentoGetTasks = new Dictionary<Task<CacheGetResponse>, System.Diagnostics.Stopwatch>();

        for (int i = 1; i <= numMomentoGetRequests; i++) {
            Console.WriteLine("Getting inside the loop");
            string key = $"key-{i}";

            Console.WriteLine("Starting the stopwatch");
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Getting key " + key);
            Task<CacheGetResponse> response = client.GetAsync(CACHE_NAME, key);

            // momentoGetTasks[response] = startTime;
            response.ContinueWith(
                async (r) => {
                    try {
                        Console.WriteLine("In ContinueWith");
                        var result = await r;
                        stopWatch.Stop();
                        momentoGetTasks[r] = stopWatch;
                    
                        var hitResult = (CacheGetResponse.Hit)result;
                        Console.WriteLine("hitResult " + hitResult.GetType() + hitResult);
                        string hitValue = hitResult.ValueString;

                        string decompressedString = await DecompressAsync(hitValue);
                        Console.WriteLine("decompressedString " + decompressedString);
                        // await LogDecompressedStringAsync(decompressedString);
                        return r;
                    } catch(Exception e) {
                        Console.WriteLine(e.Message);
                        throw e;
                    }
                    
                }
            );
            Console.WriteLine("Stopped stopwatch");
        }
        return momentoGetTasks;
    }

    // private async Task LogDecompressedStringAsync(string decompressedString)
    // {
    //     Console.WriteLine("Decompressed string: " + decompressedString);
    // }

    private string Compress() {
        string compressedString;
        // Convert the string to bytes
        byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(bigString);

        // Create a memory stream to hold the compressed data
        using (MemoryStream outputMemoryStream = new MemoryStream())
        {
             // Create a GZip stream and specify CompressionMode.Compress
            using (GZipStream compressionStream = new GZipStream(outputMemoryStream, CompressionMode.Compress))
            {
                // Compress the input string bytes
                compressionStream.Write(inputBytes, 0, inputBytes.Length);
            }

            // Get the compressed bytes from the memory stream
            byte[] compressedBytes = outputMemoryStream.ToArray();

            // Convert the compressed bytes to a Base64 string
            compressedString = Convert.ToBase64String(compressedBytes);
        }
        
        return compressedString;
    }

    private async Task<string> DecompressAsync(string valueString)
{
    byte[] compressedBytes = Convert.FromBase64String(valueString);

    using (MemoryStream compressedStream = new MemoryStream(compressedBytes))
    {
        using (MemoryStream decompressedStream = new MemoryStream())
        {
            using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                await gzipStream.CopyToAsync(decompressedStream);
            }

            byte[] decompressedBytes = decompressedStream.ToArray();
            string decompressedString = System.Text.Encoding.UTF8.GetString(decompressedBytes);

            Console.WriteLine(decompressedString);
            return decompressedString;
        }
    }
}


    private void DoRedisSets(IDatabase db, int startKeyId) {
        // Set string keys with string values
        for (int i = startKeyId; i <= numRedisSetRequests; i++)
        {
            string key = "key" + i;
            byte[] value = GenerateLargeValue(1 * 1024 * 1024); // 1MB
            Console.WriteLine("Starting the stopwatch");
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Setting key " + key);
            Task<bool> response = db.StringSetAsync(key, value);
            redisSetTasks[response] = startTime;

            response.ContinueWith(
                (r) => {
                    redisSetTasks[response].Stop();
                    return r;
                }
            );
            Console.WriteLine("Stopped stopwatch");
        }
    }

    private void DoRedisGets(IDatabase db, int startKeyId) {
        // Get values of keys asynchronously
        for (int i = startKeyId; i <= 10; i++)
        {
            string key = "key" + i;

            Console.WriteLine("Starting the stopwatch");
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Getting key " + key);
            var response = db.StringGetAsync(key);
            
            redisGetTasks[response] = startTime;
            response.ContinueWith(
                (r) => {
                    redisGetTasks[response].Stop();
                    return r;
                }
            );
            Console.WriteLine("Stopping stopwatch");
        }
    }

    private void pingRedis(IDatabase db) {
        // Chceck/Ping the redis server
        Console.WriteLine("Pinging db");
        var pong = db.Ping();
        Console.WriteLine(pong);
    }

    private static byte[] GenerateLargeValue(int sizeInBytes)
    {
        byte[] value = new byte[sizeInBytes];
        new Random().NextBytes(value);
        return value;
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