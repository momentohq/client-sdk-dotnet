using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momento.Sdk;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;

string MOMENTO_AUTH_TOKEN = Environment.GetEnvironmentVariable("MOMENTO_AUTH_TOKEN");
// Use a GUID for the cache name so it's unlikely to exist already.
string CACHE_NAME = Guid.NewGuid().ToString();
const string KEY = "MyKey";
const string VALUE = "MyData";
const uint DEFAULT_TTL_SECONDS = 60;

using (SimpleCacheClient client = new SimpleCacheClient(Configurations.Laptop.Latest, MOMENTO_AUTH_TOKEN, DEFAULT_TTL_SECONDS))
{
    Console.WriteLine($"Creating cache {CACHE_NAME}");
    var createCacheResponse = await client.CreateCacheAsync(CACHE_NAME);
    // Check the create response for an error and handle as appropriate.
    if (createCacheResponse is CreateCacheResponse.Error createError)
    {
        if (createError.ErrorCode == MomentoErrorCode.LIMIT_EXCEEDED_ERROR)
        {
            Console.WriteLine("Error: cache limit exceeded. We need to talk to support@moentohq.com! Exiting.");
        } else
        {
            Console.WriteLine($"Error creating cache: {createError.Message}. Exiting.");
        }
        // Any error is considered fatal.
        Environment.Exit(1);
    }
    // If there's already a cache by this name, we don't want to modify it.
    if (createCacheResponse is CreateCacheResponse.CacheAlreadyExists)
    {
        Console.WriteLine($"Error: strangely, a cache with the name {CACHE_NAME} already exists. Exiting.");
        Environment.Exit(1);
    }

    Console.WriteLine("\nListing caches:");
    String token = null;
    do
    {
        ListCachesResponse response = await client.ListCachesAsync(token);
        if (response is ListCachesResponse.Success successResponse)
        {
            foreach (CacheInfo cacheInfo in successResponse.Caches)
            {
                Console.WriteLine($"- {cacheInfo.Name}");
            }
            token = successResponse.NextPageToken;
        } else if (response is ListCachesResponse.Error listError)
        {
            // We do not consider this a fatal error, so we just report it.
            Console.WriteLine($"Error listing caches: {listError.Message}");
            break;
        }
    } while (!String.IsNullOrEmpty(token));

    Console.WriteLine($"\nSetting key: {KEY} with value: {VALUE}");
    var setResponse = await client.SetAsync(CACHE_NAME, KEY, VALUE);
    if (setResponse is CacheSetResponse.Error setError)
    {
        // Warn the user of the error and exit.
        Console.WriteLine($"Error setting value: {setError.Message}. Exiting.");
        Environment.Exit(1);
    }

    Console.WriteLine($"\nGetting value for  key: {KEY}");
    CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
    if (getResponse is CacheGetResponse.Hit hitResponse)
    {
        Console.WriteLine($"Looked up value: {hitResponse.String()}, Stored value: {VALUE}");
    } else if (getResponse is CacheGetResponse.Miss)
    {
        // This shouldn't be fatal but should be reported.
        Console.WriteLine($"Error: got a cache miss for {KEY}!");
    } else if (getResponse is CacheGetResponse.Error getError) {
        // Also not considered fatal.
        Console.WriteLine($"Error getting value: {getError.Message}!");
    }

    Console.WriteLine($"\nDeleting key {KEY}");
    var deleteKeyResponse = await client.DeleteAsync(CACHE_NAME, KEY);
    if (deleteKeyResponse is CacheDeleteResponse.Error deleteKeyError) {
        // Also not considred fatal.
        Console.WriteLine($"Error deleting key: {deleteKeyError.Message}!");
    }

    Console.WriteLine($"\nDeleting cache {CACHE_NAME}");
    var deleteResponse = await client.DeleteCacheAsync(CACHE_NAME);
    if (deleteResponse is DeleteCacheResponse.Error deleteCacheError)
    {
        // Report fatal error and exit
        Console.WriteLine("Error deleting cache: {deleteCacheError.Message}. Exiting.");
        Environment.Exit(1);
    }
    Console.WriteLine("\nProgram has completed successfully.");
}
