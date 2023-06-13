using System.Diagnostics;
using StackExchange.Redis;

namespace MomentoRedisExampleLambdaHandler;

public class RedisCache
{
    private IDatabase _db;
    public RedisCache(ConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }
    public void PingRedis() {
        // Check/Ping the redis server
        Console.WriteLine("Pinging db");
        var pong = _db.Ping();
        Console.WriteLine(pong);
    }

    public async Task<List<Task<RedisSetResult>>> DoRedisSets(int numRedisSetRequests)
    {
        Console.WriteLine("Executing Redis Sets");
        try
        {
            List<Task<RedisSetResult>> redisSetTasks = new List<Task<RedisSetResult>>();
            string sourceData = await Utils.GetSourceData();
            Console.WriteLine("Executing RedisSets");

            byte[] compressedSourceData = Compression.Compress(sourceData);
            
            for (int i = 1; i <= numRedisSetRequests; i++)
            {
                Console.WriteLine("Setting inside the loop");
                string key = $"key-{i}";
                redisSetTasks.Add(ExecuteSet(key, compressedSourceData));
            }

            return redisSetTasks;
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception in set " + e);
            throw new Exception();
        }
    }
    
    private async Task<RedisSetResult> ExecuteSet(string key, byte[] value)
    {
        Console.WriteLine($"Beginning redis set request for {key}");
        var stopWatch = Stopwatch.StartNew();
        var setResponseTask = _db.StringSetAsync(key, value);
        Console.WriteLine($"\tIssued redis set request for {key}");
        var setResponse = await setResponseTask;
        Console.WriteLine($"\tDone awaiting redis set response for {key}");
        stopWatch.Stop();
        var duration = stopWatch.ElapsedMilliseconds;
        // Console.WriteLine($"\tReturning redis set response for {key}: {setResponse}");        
        Console.WriteLine($"\tReturning redis set response for {key}");        
        return new RedisSetResult(Response: setResponse, ElapsedMillis: duration);
    }
    
#pragma warning disable CS1998
    public async Task<List<Task<RedisGetResult>>> DoRedisGets(int numRedisGetRequests, bool compressValue)
#pragma warning restore CS1998
    {
        Console.WriteLine("Executing Redis Gets");
        List<Task<RedisGetResult>> redisGetTasks = new List<Task<RedisGetResult>>();

        for (int i = 1; i <= numRedisGetRequests; i++)
        {
            Console.WriteLine("Executing RedisGets");
            string key = $"key-{i}";
            redisGetTasks.Add(ExecuteGet(key, compressValue));
        }

        return redisGetTasks;
    }
    
    private async Task<RedisGetResult> ExecuteGet(string key, bool compressValue)
    {
        
        Console.WriteLine($"Beginning redis get request for {key}");
        var withCompressionStopWatch = new Stopwatch();
        var stopWatch = Stopwatch.StartNew();
        if (compressValue)
        {
            withCompressionStopWatch = Stopwatch.StartNew();
        }
        var getResponseTask = _db.StringGetAsync(key);
        Console.WriteLine($"\tIssued redis get request for {key}");
        var getResponse = await getResponseTask;
        Console.WriteLine($"\tDone awaiting redis get response for {key}");
        stopWatch.Stop();
        var responseElapsedMillis = stopWatch.ElapsedMilliseconds;
        long withDecompressionElapsedMillis = 0;

        if (compressValue)
        {
            if (getResponse.HasValue)
            {
                var decompressed = Compression.Decompress(getResponse!);
                Console.WriteLine($"\tResponse finished decompressing for {key}");
                withCompressionStopWatch.Stop();
                withDecompressionElapsedMillis = withCompressionStopWatch.ElapsedMilliseconds;
            }
            else
            {
                Console.WriteLine($"\tValue not found for {key}");
            }
        }
        
        var getResult = new RedisGetResult(
            Response: getResponse,
            ElapsedMillis: responseElapsedMillis,
            ElapsedMillisWithDecompression: withDecompressionElapsedMillis
        );
        // Console.WriteLine($"\tReturning get response for {key}: {getResult}");
        Console.WriteLine($"\tReturning get response for {key}");
        return getResult;
    }

    public async void LogRedisSetResult(List<Task<RedisSetResult>> redisSetTasks)
    {
        Console.WriteLine("------- REDIS SET RESULT -------");
        foreach(var setTask in redisSetTasks)
        {
            var setResult = await setTask;
            Console.WriteLine($"Set response in {setResult.ElapsedMillis}");
        }
    }
    
    public async void LogRedisGetResult(List<Task<RedisGetResult>> redisGetTasks, bool compressValue)
    {
        Console.WriteLine("------- REDIS GET RESULT -------");
        foreach(var getTask in redisGetTasks)
        {
            var getResult = await getTask;
            Console.WriteLine(compressValue
                ? $"Got response with compression in {getResult.ElapsedMillisWithDecompression}"
                : $"Got response in {getResult.ElapsedMillis}");
        }
    }
}