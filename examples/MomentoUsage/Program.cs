using System;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;

ICredentialProvider authProvider = new EnvMomentoV2TokenProvider();
const string CACHE_NAME = "cache";
const string KEY = "MyKey";
const string VALUE = "MyData";
TimeSpan DEFAULT_TTL = TimeSpan.FromSeconds(60);

using (ICacheClient client = new CacheClient(Configurations.Laptop.V1(), authProvider, DEFAULT_TTL))
{
    var createCacheResponse = await client.CreateCacheAsync(CACHE_NAME);
    if (createCacheResponse is CreateCacheResponse.Error createError)
    {
        Console.WriteLine($"Error creating cache: {createError.Message}. Exiting.");
        Environment.Exit(1);
    }

    Console.WriteLine($"Setting key: {KEY} with value: {VALUE}");
    var setResponse = await client.SetAsync(CACHE_NAME, KEY, VALUE);
    if (setResponse is CacheSetResponse.Error setError)
    {
        Console.WriteLine($"Error setting value: {setError.Message}. Exiting.");
        Environment.Exit(1);
    }

    Console.WriteLine($"Get value for key: {KEY}");
    CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
    if (getResponse is CacheGetResponse.Hit hitResponse)
    {
        Console.WriteLine($"Looked up value: {hitResponse.ValueString}, Stored value: {VALUE}");
    }
    else if (getResponse is CacheGetResponse.Error getError)
    {
        Console.WriteLine($"Error getting value: {getError.Message}");
    }
}
