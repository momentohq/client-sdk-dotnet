# Incubating

The `MomentoSdk.Incubating` namespace has work-in-progress features that may or may be be in the final version.

# Dictionary Methods

This demonstrates the methods and response types for a dictionary data type in the cache:

```csharp
using MomentoSdk.Incubating;

class Driver
{
    public static void Main()
    {
        var client = SimpleCacheClientFactory.CreateClient(authToken: "YOUR-AUTH-TOKEN", defaultTtlSeconds: 60);
        var cacheName = "my-cache";

        // Set a value
        client.DictionarySet(cacheName: cacheName, dictionaryName: "my-dictionary",
            field: "my-key", value: "my-value", ttlSeconds: 60, refreshTtl: false);

        // Set multiple values
        client.DictionarySetMulti(
            cacheName: cacheName,
            dictionaryName: "my-dictionary",
            new Dictionary<string, string>() {
                { "key1", "value1" },
                { "key2", "value2" },
                { "key3", "value3" }},
            refreshTtl: false);

        // Get a value
        var getResponse = client.DictionaryGet(
            cacheName: cacheName,
            dictionaryName: "my-dictionary",
            field: "key1");
        var status = getResponse.Status; // HIT
        string value = getResponse.String()!; // "value1"

        // Get multiple values
        var getMultiResponse = client.DictionaryGetMulti(
            cacheName: cacheName,
            dictionaryName: "my-dictionary",
            "key1", "key2", "key3", "key4");
        var manyStatus = getMultiResponse.Status; // [HIT, HIT, HIT, MISS]
        var values = getMultiResponse.Strings(); // ["value1", "value2", "value3", null]
        var responses = getMultiResponse.Responses; // individual responses

        // Get the whole dictionary
        var getAllResponse = client.DictionaryGetAll(
            cacheName: cacheName,
            dictionaryName: "my-dictionary");
        status = getAllResponse.Status;
        var dictionary = getAllResponse.StringDictionary()!;
        value = dictionary["key1"]; // == "value1"
    }
}
```
