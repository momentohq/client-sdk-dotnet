using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;

ICredentialProvider authProvider = new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN");
const string CACHE_NAME = "momento-example";
const string KEY = "MyKey";
const string VALUE = "MyData";
const uint DEFAULT_TTL_SECONDS = 60;

using (SimpleCacheClient client = new SimpleCacheClient(Configurations.Laptop.Latest(), authProvider, DEFAULT_TTL_SECONDS))
{
    Console.WriteLine($"Creating cache {CACHE_NAME}");
    var createCacheResponse = await client.CreateCacheAsync(CACHE_NAME);
    // Check the create response for an error and handle as appropriate.
    if (createCacheResponse is CreateCacheResponse.Error createCacheError)
    {
        if (createCacheError.ErrorCode == MomentoErrorCode.LIMIT_EXCEEDED_ERROR)
        {
            Console.WriteLine("Error: cache limit exceeded. We need to talk to support@moentohq.com! Exiting.");
        } else
        {
            Console.WriteLine($"Error creating cache: {createCacheError.Message}. Exiting.");
        }
        // Any error is considered fatal.
        Environment.Exit(1);
    }
    // If there's already a cache by this name, alert the user.
    if (createCacheResponse is CreateCacheResponse.CacheAlreadyExists)
    {
        Console.WriteLine($"A cache with the name {CACHE_NAME} already exists");
    }

    Console.WriteLine("\nListing caches:");
    string? token = null;
    do
    {
        ListCachesResponse listCachesResponse = await client.ListCachesAsync(token);
        if (listCachesResponse is ListCachesResponse.Success listCachesSuccess)
        {
            foreach (CacheInfo cacheInfo in listCachesSuccess.Caches)
            {
                Console.WriteLine($"- {cacheInfo.Name}");
            }
            token = listCachesSuccess.NextPageToken;
        } else if (listCachesResponse is ListCachesResponse.Error listCachesError)
        {
            // We do not consider this a fatal error, so we just report it.
            Console.WriteLine($"Error listing caches: {listCachesError.Message}");
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

    Console.WriteLine($"\nGetting value for key: {KEY}");
    CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
    if (getResponse is CacheGetResponse.Hit getHit)
    {
        Console.WriteLine($"Looked up value: {getHit.ValueString}, Stored value: {VALUE}");
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
    var deleteCacheResponse = await client.DeleteCacheAsync(CACHE_NAME);
    if (deleteCacheResponse is DeleteCacheResponse.Error deleteCacheError)
    {
        // Report fatal error and exit
        Console.WriteLine("Error deleting cache: {deleteCacheError.Message}. Exiting.");
        Environment.Exit(1);
    }
    Console.WriteLine("\nProgram has completed successfully.");
}
