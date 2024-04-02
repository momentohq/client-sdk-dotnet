using System;
using System.Diagnostics;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;


public class Program
{
    public static async Task Main(string[] args)
    {
        MeasureMemoryUsage("Start of program");
        ICredentialProvider authProvider = new EnvMomentoTokenProvider("MOMENTO_API_KEY");
        MeasureMemoryUsage("After auth provider");
        const string CACHE_NAME = "cache";
        const string KEY = "MyKey";
        const string VALUE = "MyData";
        TimeSpan DEFAULT_TTL = TimeSpan.FromSeconds(60);
        MeasureMemoryUsage("After timespan");


        using (ICacheClient client = new CacheClient(Configurations.Laptop.V1(), authProvider, DEFAULT_TTL))
        {
            MeasureMemoryUsage("After creating client");
            var createCacheResponse = await client.CreateCacheAsync(CACHE_NAME);
            MeasureMemoryUsage("After creating cache");
            if (createCacheResponse is CreateCacheResponse.Error createError)
            {
                Console.WriteLine($"Error creating cache: {createError.Message}. Exiting.");
                Environment.Exit(1);
            }

            Console.WriteLine($"Setting key: {KEY} with value: {VALUE}");
            var setResponse = await client.SetAsync(CACHE_NAME, KEY, VALUE);
            MeasureMemoryUsage("After setting value");
            if (setResponse is CacheSetResponse.Error setError)
            {
                Console.WriteLine($"Error setting value: {setError.Message}. Exiting.");
                Environment.Exit(1);
            }

            Console.WriteLine($"Get value for key: {KEY}");
            CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
            MeasureMemoryUsage("After getting value");
            if (getResponse is CacheGetResponse.Hit hitResponse)
            {
                Console.WriteLine($"Looked up value: {hitResponse.ValueString}, Stored value: {VALUE}");
            }
            else if (getResponse is CacheGetResponse.Error getError)
            {
                Console.WriteLine($"Error getting value: {getError.Message}");
            }
        }
    }

    public static void MeasureMemoryUsage(string message)
    {
        var process = Process.GetCurrentProcess();
        Console.WriteLine($"[{message}] Memory Usage: {process.WorkingSet64 / 1024} KB");
    }
}
