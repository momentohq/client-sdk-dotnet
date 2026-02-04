using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;

var authProvider = new EnvMomentoV2TokenProvider();

var defaultTtl = TimeSpan.FromSeconds(60);

using (ICacheClient client = new CacheClient(Configurations.Laptop.V1(), authProvider, defaultTtl))
{
    await AdvancedExamples.CreateCacheExample(client);
    await AdvancedExamples.ListCachesExample(client);
    await AdvancedExamples.SetGetDeleteExample(client);
    await AdvancedExamples.DeleteCacheExample(client);
    await AdvancedExamples.EagerConnectionExample();

    Console.WriteLine("\nProgram has completed successfully.");
}


public class AdvancedExamples
{
    private const string CacheName = "momento-example";
    private const string Key = "MyKey";
    private const string Value = "MyData";

    public static async Task CreateCacheExample(ICacheClient client)
    {

        Console.WriteLine($"Creating cache {CacheName}");
        var createCacheResponse = await client.CreateCacheAsync(CacheName);
        // Check the create response for an error and handle as appropriate.
        if (createCacheResponse is CreateCacheResponse.Error createCacheError)
        {
            if (createCacheError.ErrorCode == MomentoErrorCode.LIMIT_EXCEEDED_ERROR)
            {
                Console.WriteLine("Error: cache limit exceeded. We need to talk to support@moentohq.com! Exiting.");
            }
            else
            {
                Console.WriteLine($"Error creating cache: {createCacheError.Message}. Exiting.");
            }
            // Any error is considered fatal.
            Environment.Exit(1);
        }
        // If there's already a cache by this name, alert the user.
        if (createCacheResponse is CreateCacheResponse.CacheAlreadyExists)
        {
            Console.WriteLine($"A cache with the name {CacheName} already exists");
        }
    }

    public static async Task ListCachesExample(ICacheClient client)
    {
        Console.WriteLine("\nListing caches:");
        ListCachesResponse listCachesResponse = await client.ListCachesAsync();
        if (listCachesResponse is ListCachesResponse.Success listCachesSuccess)
        {
            foreach (CacheInfo cacheInfo in listCachesSuccess.Caches)
            {
                Console.WriteLine($"- {cacheInfo.Name}");
            }
        }
        else if (listCachesResponse is ListCachesResponse.Error listCachesError)
        {
            // We do not consider this a fatal error, so we just report it.
            Console.WriteLine($"Error listing caches: {listCachesError.Message}");
        }
    }

    public static async Task SetGetDeleteExample(ICacheClient client)
    {
        Console.WriteLine($"\nSetting key: {Key} with value: {Value}");
        var setResponse = await client.SetAsync(CacheName, Key, Value);
        if (setResponse is CacheSetResponse.Error setError)
        {
            // Warn the user of the error and exit.
            Console.WriteLine($"Error setting value: {setError.Message}. Exiting.");
            Environment.Exit(1);
        }

        Console.WriteLine($"\nGetting value for key: {Key}");
        CacheGetResponse getResponse = await client.GetAsync(CacheName, Key);
        if (getResponse is CacheGetResponse.Hit getHit)
        {
            Console.WriteLine($"Looked up value: {getHit.ValueString}, Stored value: {Value}");
        }
        else if (getResponse is CacheGetResponse.Miss)
        {
            // This shouldn't be fatal but should be reported.
            Console.WriteLine($"Error: got a cache miss for {Key}!");
        }
        else if (getResponse is CacheGetResponse.Error getError)
        {
            // Also not considered fatal.
            Console.WriteLine($"Error getting value: {getError.Message}!");
        }

        Console.WriteLine($"\nDeleting key {Key}");
        var deleteKeyResponse = await client.DeleteAsync(CacheName, Key);
        if (deleteKeyResponse is CacheDeleteResponse.Error deleteKeyError)
        {
            // Also not considered fatal.
            Console.WriteLine($"Error deleting key: {deleteKeyError.Message}!");
        }
    }

    public static async Task DeleteCacheExample(ICacheClient client)
    {

        Console.WriteLine($"\nDeleting cache {CacheName}");
        var deleteCacheResponse = await client.DeleteCacheAsync(CacheName);
        if (deleteCacheResponse is DeleteCacheResponse.Error deleteCacheError)
        {
            // Report fatal error and exit
            Console.WriteLine($"Error deleting cache: {deleteCacheError.Message}. Exiting.");
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// By default, cache clients will connect lazily when the first request is
    /// issued.  This example shows how to configure a client to connect eagerly
    /// when it is instantiated, and to specify a distinct timeout for the
    /// connection to be established.
    /// </summary>
    public static async Task EagerConnectionExample()
    {
        var authProvider = new EnvMomentoV2TokenProvider();
        var defaultTtl = TimeSpan.FromSeconds(60);
        var config = Configurations.Laptop.V1();
        var eagerConnectionTimeout = TimeSpan.FromSeconds(10);

        Console.WriteLine("Creating a momento client with eager connection");
        using (var client = await CacheClient.CreateAsync(config, authProvider, defaultTtl, eagerConnectionTimeout))
        {
            Console.WriteLine("Successfully created a momento client with eager connection");
        }
    }
}
