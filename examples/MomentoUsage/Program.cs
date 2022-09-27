//// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Momento.Sdk;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;

String MOMENTO_AUTH_TOKEN = Environment.GetEnvironmentVariable("MOMENTO_AUTH_TOKEN");
String CACHE_NAME = "cache";
String KEY = "MyKey";
String VALUE = "MyData";
uint DEFAULT_TTL_SECONDS = 60;

using (SimpleCacheClient client = new SimpleCacheClient(Configurations.Laptop.Latest, MOMENTO_AUTH_TOKEN, DEFAULT_TTL_SECONDS))
{
    var createCacheResult = await client.CreateCacheAsync(CACHE_NAME);

    Console.WriteLine($"\nSetting key: {KEY} with value: {VALUE}");
    await client.SetAsync(CACHE_NAME, KEY, VALUE);
    Console.WriteLine($"\nGet value for  key: {KEY}");
    CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
    if (getResponse is CacheGetResponse.Hit hitResponse)
    {
        Console.WriteLine($"\nLookedup value: {hitResponse.String()}, Stored value: {VALUE}");
    }
}
