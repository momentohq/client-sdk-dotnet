using System;
using System.Diagnostics;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;
using Microsoft.Diagnostics.NETCore.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        MeasureMemoryUsage("Start of program");
        WriteMemoryDump("./heapdump");
        ICredentialProvider authProvider = new StringMomentoTokenProvider("eyJlbmRwb2ludCI6ImNlbGwtNC11cy13ZXN0LTItMS5wcm9kLmEubW9tZW50b2hxLmNvbSIsImFwaV9rZXkiOiJleUpoYkdjaU9pSklVekkxTmlKOS5leUp6ZFdJaU9pSnRhV05vWVdWc1FHMXZiV1Z1ZEc5b2NTNWpiMjBpTENKMlpYSWlPakVzSW5BaU9pSkRRVUU5SWl3aVpYaHdJam94TnpFeU5qazBORFEyZlEuTzdUNXVWaXhjUlZuQmc1aS1FOWZyRlVsc1dtVWJZcTZqaVhhNTRuamh0dyJ9");
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
            Console.ReadLine();
            WriteMemoryDump("./heapdump");
        }
    }

    public static void MeasureMemoryUsage(string message)
    {
        var process = Process.GetCurrentProcess();
        Console.WriteLine($"[{message}] Memory Usage: {process.WorkingSet64 / 1024} KB");
        Console.ReadLine();
    }

    public static void WriteMemoryDump(string prefixPath)
    {
        try
        {
            var processId = Process.GetCurrentProcess().Id;
            var client = new DiagnosticsClient(processId);
            string dumpPath = $"{prefixPath}-{DateTime.Now:yyyMMdd_HHmmss}.dmp";
            client.WriteDump(DumpType.Full, dumpPath, logDumpGeneration: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating heap dump: {ex.Message}");
        }
    }
}
