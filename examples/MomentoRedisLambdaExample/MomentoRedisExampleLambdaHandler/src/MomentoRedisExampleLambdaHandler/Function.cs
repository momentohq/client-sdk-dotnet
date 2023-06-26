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
    public bool RunMomento { get; set; }
    public bool CompressMomento { get; set; }
    public bool SleepMomento { get; set; }
    public bool RunRedis { get; set; }
    public bool CompressRedis { get; set; }
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

        List<Task<SetResult>> momentoSetTasks;
        List<Task<GetResult>> momentoGetTasks;
        List<Task<RedisSetResult>> redisSetTasks;
        List<Task<RedisGetResult>> redisGetTasks;
        
        momentoSetTasks = input.RunMomento ? await momentoCache.DoMomentoSets(_numMomentoSetRequests, withSleep: input.SleepMomento) : new List<Task<SetResult>>();
        redisSetTasks = input.RunRedis ? await redisCache.DoRedisSets(_numRedisSetRequests) : new List<Task<RedisSetResult>>();
        momentoGetTasks = input.RunMomento ? await momentoCache.DoMomentoGets(_numMomentoGetRequests, compressValue: input.CompressMomento, withSleep: input.SleepMomento) : new List<Task<GetResult>>();
        redisGetTasks = input.RunRedis ? await redisCache.DoRedisGets(_numRedisGetRequests, compressValue: input.CompressRedis) : new List<Task<RedisGetResult>>();

        Console.WriteLine("Awaiting pending tasks");
        // empty lists should no-op
        Task.WaitAll(momentoSetTasks.Select(task => (Task)task).ToArray());
        Task.WaitAll(redisSetTasks.Select(task => (Task)task).ToArray());
        Task.WaitAll(momentoGetTasks.Select(task => (Task)task).ToArray());
        Task.WaitAll(redisGetTasks.Select(task => (Task)task).ToArray());
        Console.Write("Done awaiting tasks");

        if (input.RunMomento) {
            momentoCache.LogMomentoSetResult(momentoSetTasks);
            momentoCache.LogMomentoGetResult(momentoGetTasks, compressValue: input.CompressMomento);
        }
        if (input.RunRedis) {
            redisCache.LogRedisSetResult(redisSetTasks);
            redisCache.LogRedisGetResult(redisGetTasks, compressValue: input.CompressRedis);
        }

        return "Lambda Execution Complete";
    }

    static void Main() {
        try {
            Console.WriteLine("Calling DoStuff");
            var input = new Input
            {
                RunMomento = true,
                CompressMomento = false,
                SleepMomento = false,
                RunRedis = false,
                CompressRedis = false
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