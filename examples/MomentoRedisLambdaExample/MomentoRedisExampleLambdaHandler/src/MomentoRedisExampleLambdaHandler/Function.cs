using Amazon.Lambda.Core;
using StackExchange.Redis;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MomentoRedisExampleLambdaHandler;

public class Input
{
    public bool RunMomentoWithCompression { get; set; }
    public bool RunMomentoCompressionWithSleep { get; set; } 
    public bool RunMomentoWithoutCompression { get; set; }
    public bool RunRedisWithCompression { get; set; }
    public bool RunRedisWithoutCompression { get; set; }
}

public record struct GetResult(
    CacheGetResponse Response,
    long ResponseSize,
    long DecompressedResponseSize,
    long ElapsedMillis,
    long ElapsedMillisWithDecompression
);

public record struct SetResult(CacheSetResponse Response, long ElapsedMillis);

public record struct RedisGetResult(RedisValue Response, long ElapsedMillis, long ElapsedMillisWithDecompression);

public record struct RedisSetResult(bool Response, long ElapsedMillis);

public class Function
{
    const string CacheName = "MomentoLambda";
    const string RedisClusterEndpoint = "momentoredisexamplecluster.exmof5.ng.0001.usw2.cache.amazonaws.com:6379";
    private int _numMomentoGetRequests = 10;
    private int _numMomentoSetRequests = 10;
    private int _numRedisGetRequests = 10;
    private int _numRedisSetRequests = 10;

    /// <summary>
    /// Function Handler for running Momento Requests
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(Input input, ILambdaContext context)
    {
        await new Function().DoStuff(input);
        
        return "Task completed";
    }

    private async Task<string> DoStuff(Input input) {

        Console.WriteLine("Start executing DoStuff....");

        // Create Momento client
        ICredentialProvider authProvider = new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN");
        var momentoClient = new CacheClient(
            Configurations.InRegion.Lambda.Latest().WithClientTimeout(TimeSpan.FromSeconds(3000)),
            authProvider,
            TimeSpan.FromSeconds(30)
        );
        
        // Create a Redis client
        Console.WriteLine("Creating redis client");
        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = { RedisClusterEndpoint }
        };
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configurationOptions);
        
        // Instantiate MomentoCache and RedisCache
        MomentoCache momentoCache = new MomentoCache(momentoClient, CacheName);
        RedisCache redisCache = new RedisCache(redis);

        // Create momento cache if not already exists
        await momentoCache.CreateMomentoCache();
        
        // Ping redis client
        redisCache.PingRedis();
        
        await ExecuteLambda(input, momentoCache, redisCache);
        
        // Close the redis connection
        await redis.CloseAsync();
        
        Console.WriteLine("End executing DoStuff");

        return "Task completed!";
    }

    private async Task<string> ExecuteLambda(Input input, MomentoCache momentoCache, RedisCache redisCache)
    {
        Console.WriteLine("Inside Execute Lambda function");
        Console.WriteLine($"Input {input}");

        if (input is { RunMomentoWithCompression: true, RunMomentoWithoutCompression: true } or { RunRedisWithCompression: true, RunRedisWithoutCompression: true })
        {
            Console.WriteLine("Cannot run with and without compression configurations together!");
            throw new Exception("Invalid combination of parameters.");
        }

        if (input is { RunMomentoCompressionWithSleep: true, RunMomentoWithCompression: false})
        {
            Console.WriteLine("RunMomentoCompressionWithSleep can run only when RunMomentoWithCompression is true.");
            throw new Exception("Invalid combination of parameters. RunMomentoCompressionWithSleep can run only when RunMomentoWithCompression is true.");
        }

        var trueCount = 0;
        if (input.RunMomentoWithCompression) trueCount++;
        if (input.RunMomentoWithoutCompression) trueCount++;
        if (input.RunRedisWithCompression) trueCount++;
        if (input.RunRedisWithoutCompression) trueCount++;

        List<Task<SetResult>> momentoSetTasks;
        List<Task<GetResult>> momentoGetTasks;
        List<Task<RedisSetResult>> redisSetTasks;
        List<Task<RedisGetResult>> redisGetTasks;

        switch (trueCount)
        {
            case 1 when input.RunMomentoWithCompression:
                // Execute code for running Momento with compression
                Console.WriteLine("------- RUNNING MOMENTO WITH COMPRESSION -------");
                momentoSetTasks = await momentoCache.DoMomentoSets(_numMomentoSetRequests, withSleep: input.RunMomentoCompressionWithSleep);
                momentoGetTasks = await momentoCache.DoMomentoGets(_numMomentoGetRequests, compressValue: true, withSleep: input.RunMomentoCompressionWithSleep);
                
                Console.WriteLine("Awaiting pending tasks");
                Task.WaitAll(momentoSetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(momentoGetTasks.Select(task => (Task)task).ToArray());
                Console.Write("Done awaiting tasks");
                
                momentoCache.LogMomentoSetResult(momentoSetTasks);
                momentoCache.LogMomentoGetResult(momentoGetTasks, compressValue: true);
                break;
            
            case 1 when input.RunMomentoWithoutCompression:
                // Execute code for running Momento without compression
                Console.WriteLine("------- RUNNING MOMENTO WITHOUT COMPRESSION -------");
                momentoSetTasks = await momentoCache.DoMomentoSets(_numMomentoSetRequests);
                momentoGetTasks = await momentoCache.DoMomentoGets(_numMomentoGetRequests, compressValue: false);
                
                Console.WriteLine("Awaiting pending tasks");
                Task.WaitAll(momentoSetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(momentoGetTasks.Select(task => (Task)task).ToArray());
                Console.Write("Done awaiting tasks");
                
                momentoCache.LogMomentoSetResult(momentoSetTasks);
                momentoCache.LogMomentoGetResult(momentoGetTasks, compressValue: false);
                break;
            
            case 1 when input.RunRedisWithCompression:
                // Execute code for running Redis with compression
                Console.WriteLine("------- RUNNING REDIS WITH COMPRESSION -------");
                redisSetTasks = await redisCache.DoRedisSets(_numRedisSetRequests);
                redisGetTasks = await redisCache.DoRedisGets(_numRedisGetRequests, compressValue: true);
                
                Console.WriteLine("Awaiting pending tasks");
                Task.WaitAll(redisSetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(redisGetTasks.Select(task => (Task)task).ToArray());
                Console.Write("Done awaiting tasks");
                
                redisCache.LogRedisSetResult(redisSetTasks);
                redisCache.LogRedisGetResult(redisGetTasks, compressValue: true);
                break;
            
            case 1 when input.RunRedisWithoutCompression:
                // Execute code for running Redis without compression
                Console.WriteLine("------- RUNNING REDIS WITHOUT COMPRESSION -------");
                redisSetTasks = await redisCache.DoRedisSets(_numRedisSetRequests);
                redisGetTasks = await redisCache.DoRedisGets(_numRedisGetRequests, compressValue: false);
                
                Console.WriteLine("Awaiting pending tasks");
                Task.WaitAll(redisSetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(redisGetTasks.Select(task => (Task)task).ToArray());
                Console.Write("Done awaiting tasks");
                
                redisCache.LogRedisSetResult(redisSetTasks);
                redisCache.LogRedisGetResult(redisGetTasks, compressValue: false);
                break;
            
            case 2 when input is { RunMomentoWithCompression: true, RunRedisWithCompression: true }:
                // Execute code for running Momento with compression and Redis with compression
                Console.WriteLine("------- RUNNING MOMENTO AND REDIS WITH COMPRESSION -------");
                momentoSetTasks = await momentoCache.DoMomentoSets(_numMomentoSetRequests, withSleep: input.RunMomentoCompressionWithSleep);
                redisSetTasks = await redisCache.DoRedisSets(_numRedisSetRequests);
                momentoGetTasks = await momentoCache.DoMomentoGets(_numMomentoGetRequests, compressValue: true, withSleep: input.RunMomentoCompressionWithSleep);
                redisGetTasks = await redisCache.DoRedisGets(_numRedisGetRequests, compressValue: true);
                
                Console.WriteLine("Awaiting pending tasks");
                Task.WaitAll(momentoSetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(redisSetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(momentoGetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(redisGetTasks.Select(task => (Task)task).ToArray());
                Console.Write("Done awaiting tasks");
                
                momentoCache.LogMomentoSetResult(momentoSetTasks);
                momentoCache.LogMomentoGetResult(momentoGetTasks, compressValue: true);
                redisCache.LogRedisSetResult(redisSetTasks);
                redisCache.LogRedisGetResult(redisGetTasks, compressValue: true);
                break;
            
            case 2 when input is { RunMomentoWithCompression: true, RunRedisWithoutCompression: true }:
                // Execute code for running Momento with compression and Redis without compression
                Console.WriteLine("------- RUNNING MOMENTO WITH COMPRESSION AND REDIS WITHOUT COMPRESSION -------");
                momentoSetTasks = await momentoCache.DoMomentoSets(_numMomentoSetRequests, withSleep: input.RunMomentoCompressionWithSleep);
                redisSetTasks = await redisCache.DoRedisSets(_numRedisSetRequests);
                momentoGetTasks = await momentoCache.DoMomentoGets(_numMomentoGetRequests, compressValue: true, withSleep: input.RunMomentoCompressionWithSleep);
                redisGetTasks = await redisCache.DoRedisGets(_numRedisGetRequests, compressValue: false);
                
                Console.WriteLine("Awaiting pending tasks");
                Task.WaitAll(momentoSetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(redisSetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(momentoGetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(redisGetTasks.Select(task => (Task)task).ToArray());
                Console.Write("Done awaiting tasks");
                
                momentoCache.LogMomentoSetResult(momentoSetTasks);
                momentoCache.LogMomentoGetResult(momentoGetTasks, compressValue: true);
                redisCache.LogRedisSetResult(redisSetTasks);
                redisCache.LogRedisGetResult(redisGetTasks, compressValue: false);
                break;
            
            case 2 when input is { RunMomentoWithoutCompression: true, RunRedisWithCompression: true }:
                // Execute code for running Momento without compression and Redis with compression
                Console.WriteLine("------- RUNNING MOMENTO WITHOUT COMPRESSION AND REDIS WITH COMPRESSION -------");
                momentoSetTasks = await momentoCache.DoMomentoSets(_numMomentoSetRequests);
                redisSetTasks = await redisCache.DoRedisSets(_numRedisSetRequests);
                momentoGetTasks = await momentoCache.DoMomentoGets(_numMomentoGetRequests, compressValue: false);
                redisGetTasks = await redisCache.DoRedisGets(_numRedisGetRequests, compressValue: true);
                
                Console.WriteLine("Awaiting pending tasks");
                Task.WaitAll(momentoSetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(redisSetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(momentoGetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(redisGetTasks.Select(task => (Task)task).ToArray());
                Console.Write("Done awaiting tasks");
                
                momentoCache.LogMomentoSetResult(momentoSetTasks);
                momentoCache.LogMomentoGetResult(momentoGetTasks, compressValue: false);
                redisCache.LogRedisSetResult(redisSetTasks);
                redisCache.LogRedisGetResult(redisGetTasks, compressValue: true);
                break;
            
            case 2 when input is { RunMomentoWithoutCompression: true, RunRedisWithoutCompression: true }:
                // Execute code for running Momento without compression and Redis without compression
                Console.WriteLine("------- RUNNING MOMENTO AND REDIS WITHOUT COMPRESSION-------");
                momentoSetTasks = await momentoCache.DoMomentoSets(_numMomentoSetRequests);
                redisSetTasks = await redisCache.DoRedisSets(_numRedisSetRequests);
                momentoGetTasks = await momentoCache.DoMomentoGets(_numMomentoGetRequests, compressValue: false);
                redisGetTasks = await redisCache.DoRedisGets(_numRedisGetRequests, compressValue: false);
                
                Console.WriteLine("Awaiting pending tasks");
                Task.WaitAll(momentoSetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(redisSetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(momentoGetTasks.Select(task => (Task)task).ToArray());
                Task.WaitAll(redisGetTasks.Select(task => (Task)task).ToArray());
                Console.Write("Done awaiting tasks");
                
                momentoCache.LogMomentoSetResult(momentoSetTasks);
                momentoCache.LogMomentoGetResult(momentoGetTasks, compressValue: false);
                redisCache.LogRedisSetResult(redisSetTasks);
                redisCache.LogRedisGetResult(redisGetTasks, compressValue: false);
                break;

            default:
                // Handle the case when no parameters are set to true or more than two parameters are set to true
                throw new Exception("Invalid combination of parameters.");
        }

        return "Lambda Execution Complete";
    }

    static void Main() {
        try {
            Console.WriteLine("Calling DoStuff");
            Input input = new Input
            {
                RunMomentoWithCompression = true,
                RunMomentoCompressionWithSleep = false,
                RunMomentoWithoutCompression = false,
                RunRedisWithCompression = false,
                RunRedisWithoutCompression = false
            };
            var task = new Function().DoStuff(input);
            Console.WriteLine("Waiting for task via task.Result");
            Console.WriteLine($"RESULT: {task.Result}");
        } catch(Exception ex) {
            Console.WriteLine("Caught Exception");
            Console.WriteLine(ex);
        }
    }

}