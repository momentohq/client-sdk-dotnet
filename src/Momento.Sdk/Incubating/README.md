# Incubating

The `Momento.Sdk.Incubating` namespace has work-in-progress features that may or may be be in the final version.

# Dictionary Methods

This demonstrates the methods and response types for a dictionary data type in the cache:

```csharp
using Momento.Sdk.Incubating;

class Driver
{
    public static void Main()
    {
        using var client = SimpleCacheClientFactory.CreateClient(authToken: "YOUR-AUTH-TOKEN", defaultTtlSeconds: 60);
        var cacheName = "my-cache";

public static class Driver
{
    public async static Task Main()
    {
        var authToken = System.Environment.GetEnvironmentVariable("TEST_AUTH_TOKEN")!;
        //using var client = new SimpleCacheClient(authToken, 60);
        var cacheName = "my-example-cache";

        using var client = Momento.Sdk.Incubating.SimpleCacheClientFactory.CreateClient(authToken, 60);

        // Set a value
        await client.DictionarySetAsync(cacheName: cacheName, dictionaryName: "my-dictionary",
            field: "my-key", value: "my-value", refreshTtl: false, ttlSeconds: 60);

        // Set multiple values
        await client.DictionarySetBatchAsync(
            cacheName: cacheName,
            dictionaryName: "my-dictionary",
            new Dictionary<string, string>() {
                { "key1", "value1" },
                { "key2", "value2" },
                { "key3", "value3" }},
            refreshTtl: false);

        // Get a value
        var getResponse = await client.DictionaryGetAsync(
            cacheName: cacheName,
            dictionaryName: "my-dictionary",
            field: "key1");
        var status = getResponse.Status; // HIT
        string value = getResponse.String()!; // "value1"

        // Get multiple values
        var getBatchResponse = await client.DictionaryGetBatchAsync(
            cacheName: cacheName,
            dictionaryName: "my-dictionary",
            new string[] { "key1", "key2", "key3", "key4" });
        var manyStatus = getBatchResponse.Status; // [HIT, HIT, HIT, MISS]
        var values = getBatchResponse.Strings(); // ["value1", "value2", "value3", null]
        var responses = getBatchResponse.Responses; // individual responses

        // Get the whole dictionary
        var fetchResponse = await client.DictionaryFetchAsync(
            cacheName: cacheName,
            dictionaryName: "my-dictionary");
        status = fetchResponse.Status;
        var dictionary = fetchResponse.StringStringDictionary()!;
        value = dictionary["key1"]; // == "value1"
    }
}
```
