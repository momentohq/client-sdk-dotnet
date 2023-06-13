using System.Diagnostics;
using Momento.Sdk;
using Momento.Sdk.Responses;

namespace MomentoRedisExampleLambdaHandler;

public class MomentoCache
{
    private string _cacheName;
    private CacheClient _client;

    public MomentoCache(CacheClient client, string cacheName)
    {
        _client = client;
        _cacheName = cacheName;
    }
    
    public async Task CreateMomentoCache()
    {
        var response = await _client.CreateCacheAsync(_cacheName);

        switch (response)
        {
            case CreateCacheResponse.Success:
                Console.WriteLine($"\t Cache: {_cacheName} created successfully.");
                break;
            case CreateCacheResponse.CacheAlreadyExists:
                Console.WriteLine($"\t Cache: {_cacheName} already exists.");
                break;
            case CreateCacheResponse.Error:
                Console.WriteLine($"\t Error creating cache: {_cacheName}");
                break;
        }
    }
    
    public async Task<List<Task<SetResult>>> DoMomentoSets(int numMomentoSetRequests, bool withSleep = false)
    {
        Console.WriteLine($"\t WITH SLEEP flag: {withSleep}");
        try
        {
            List<Task<SetResult>> momentoSetTasks = new List<Task<SetResult>>();
            string sourceData = await Utils.GetSourceData();
            Console.WriteLine("Executing MomentoSets");

            byte[] compressedSourceData = Compression.Compress(sourceData);
            
            for (int i = 1; i <= numMomentoSetRequests; i++)
            {
                Console.WriteLine("Setting inside the loop");
                string key = $"key-{i}";
                momentoSetTasks.Add(ExecuteSet(key, compressedSourceData, withSleep));
            }

            return momentoSetTasks;
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception in set " + e);
            throw new Exception();
        }
    }
    
    private async Task<SetResult> ExecuteSet(string key, byte[] value, bool withSleep)
    {
        Console.WriteLine($"Beginning set request for {key}");
        var stopWatch = Stopwatch.StartNew();
        var setResponseTask = _client.SetAsync(_cacheName, key, value, TimeSpan.FromMinutes(30));
        Console.WriteLine($"\tIssued set request for {key}");
        var setResponse = await setResponseTask;
        Console.WriteLine($"\tDone awaiting set response for {key}");
        
        if (withSleep)
        {
            await Task.Delay(500);
            Console.WriteLine($"SET returned from sleeping 500ms for key {key}");
        }
        
        stopWatch.Stop();
        var duration = stopWatch.ElapsedMilliseconds;
        Console.WriteLine($"\tReturning set response for {key}: {setResponse}");        
        return new SetResult(Response: setResponse, ElapsedMillis: duration);
    }
    
#pragma warning disable CS1998
    public async Task<List<Task<GetResult>>> DoMomentoGets(int numMomentoGetRequests, bool compressValue, bool withSleep = false) {
#pragma warning restore CS1998
        List<Task<GetResult>> momentoGetTasks = new List<Task<GetResult>>();

        for (int i = 1; i <= numMomentoGetRequests; i++)
        {
            Console.WriteLine("Executing MomentoGets");
            string key = $"key-{i}";
            momentoGetTasks.Add(ExecuteGet(key, compressValue, withSleep));
        }

        return momentoGetTasks;
    }

    private async Task<GetResult> ExecuteGet(string key, bool compressValue, bool withSleep)
    {
        var withCompressionStopWatch = new Stopwatch();
        Console.WriteLine($"Beginning get request for {key}");
        var stopWatch = Stopwatch.StartNew();
        if (compressValue)
        {
            withCompressionStopWatch = Stopwatch.StartNew();
        }
        var getResponseTask = _client.GetAsync(_cacheName, key);
        Console.WriteLine($"\tIssued get request for {key}");
        var getResponse = await getResponseTask;
        Console.WriteLine($"\tDone awaiting get response for {key}");
        
        if (withSleep)
        {
            await Task.Delay(500);
            Console.WriteLine($"GET returned from sleeping 500ms for key {key}");
        }
        
        stopWatch.Stop();
        var responseElapsedMillis = stopWatch.ElapsedMilliseconds;
        long withDecompressionElapsedMillis = 0;
        long responseSize = 0;
        long decompressedResponseSize = 0;
        if (getResponse is CacheGetResponse.Hit hitResponse)
        {
            Console.WriteLine($"\tResponse for {key} is a hit; decompressing");
            responseSize = hitResponse.ValueByteArray.Length;
            if (compressValue)
            {
                var decompressed = Compression.Decompress(hitResponse.ValueByteArray);
                Console.WriteLine($"\tResponse finished decompressing for {key}");
                decompressedResponseSize = decompressed.Length;
                withCompressionStopWatch.Stop();
                withDecompressionElapsedMillis = withCompressionStopWatch.ElapsedMilliseconds;
            }
            
        }
        else
        {
            Console.WriteLine(
                $"\n\nGOT SOMETHING BESIDES A HIT FOR KEY {key}; PROBABLY SHOULDN'T USE THIS DATA\n\n");
        }
        
        var getResult = new GetResult(
            Response: getResponse,
            ResponseSize: responseSize,
            DecompressedResponseSize: decompressedResponseSize,
            ElapsedMillis: responseElapsedMillis,
            ElapsedMillisWithDecompression: withDecompressionElapsedMillis
        );
        Console.WriteLine($"\tReturning get response for {key}: {getResult}");
        return getResult;
    }
    
    public async void LogMomentoSetResult(List<Task<SetResult>> momentoSetTasks)
    {
        Console.WriteLine("------- MOMENTO SET RESULT -------");
        foreach(var setTask in momentoSetTasks)
        {
            var setResult = await setTask;
            if (setResult.Response is CacheSetResponse.Error err) {
                Console.WriteLine("ERROR: " + err.Message);
            } else if (setResult.Response is CacheSetResponse.Success) {
                Console.WriteLine("Set in " + setResult.ElapsedMillis);
            }
        }
    }
    
    public async void LogMomentoGetResult(List<Task<GetResult>> momentoGetTasks, bool compressValue)
    {
        Console.WriteLine("------- MOMENTO GET RESULT -------");
        
        foreach(var task in momentoGetTasks)
        {
            var getResult = await task;
            if (getResult.Response is CacheGetResponse.Miss) {
                Console.WriteLine($"Unexpected MISS for result: {getResult}");
            } else if (getResult.Response is CacheGetResponse.Error) {
                Console.WriteLine($"ERROR: {getResult}");
            } else if (getResult.Response is CacheGetResponse.Hit)
            {
                Console.WriteLine(!compressValue
                    ? $"Got HIT for: {getResult.ResponseSize} in {getResult.ElapsedMillis}"
                    : $"Got HIT after decompressing for: {getResult.DecompressedResponseSize} in {getResult.ElapsedMillisWithDecompression}");
            }
        }
    }
}