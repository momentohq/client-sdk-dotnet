# Incubating

The `MomentoSdk.Incubating` namespace has work-in-progress features that may or may be be in the final version.

# Dictionary Methods

This demonstrates the methods and response types for a dictionary data type in the cache:

```csharp
using System.Collections.Generic;
using MomentoSdk.Responses;
using MomentoSdk.Incubating.Responses;


class Driver
{
    public static void Main()
    {
        MomentoSdk.Incubating.SimpleCacheClient scc = new MomentoSdk.Incubating.SimpleCacheClient(
            authToken: "MYTOKEN", defaultTtlSeconds: 60);

        // Set a value
        scc.DictionarySet(cacheName: "my-cache", dictionaryName: "my-dictionary"
            key: "my-key", value: "my-value", ttlSeconds: 60, refreshTtl: false);

        // Set multiple values
        scc.DictionarySetMulti(
            cacheName: "my-cache",
            dictionaryName: "my-dictionary",
            new Dictionary<string, string>() {
                { "key1", "value1" },
                { "key2", "value2" },
                { "key3", "value3" }},
            refreshTtl: false);

        // Get a value
        CacheDictionaryGetResponse gr = scc.DictionaryGet(
            cacheName: "my-cache",
            dictionaryName: "my-dictionary",
            key: "key1");
        CacheGetStatus status = gr.Status(); // HIT
        string value = gr.String(); // "value1"

        // Get multiple values
        CacheDictionaryGetMultiResponse mgr = scc.DictionaryGetMulti(
            cacheName: "my-cache",
            dictionaryName: "my-dictionary",
            "key1", "key2", "key3", "key4");
        IList<CacheGetStatus> manyStatus = mgr.Status(); // [HIT, HIT, HIT, MISS]
        IList<string?> values = mgr.Values(); // ["value1", "value2", "value3", null]
        IList<CacheGetResponse> individualResponses = mgr.ToList();

        // Get the whole dictionary
        CacheDictionaryGetAllResponse gar = scc.DictionaryGetAll(
            cacheName: "my-cache",
            dictionaryName: "my-dictionary");
        status = gar.Status;
        Dictionary<string, string> dictionary = gar.Dictionary();
        value = dictionary["key1"]; // == "value1"
    }
}
```
