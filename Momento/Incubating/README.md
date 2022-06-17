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

        // Set some values
        CacheDictionarySetResponse sr = scc.DictionarySet(cacheName: "my-cache", dictionaryName: "my-dictionary",
            new Dictionary<string, string>() {
                { "key1", "value1" },
                { "key2", "value2" },
                { "key3", "value3" }
        });
        string dictionaryName = sr.DictionaryName();
        Dictionary<string, CacheDictionaryValue> dictionary = sr.Dictionary();

        // Get a value
        CacheDictionaryGetResponse gr = scc.DictionaryGet(cacheName: "my-cache", dictionaryName: "my-dictionary", key: "key1");
        CacheGetStatus status = gr.Status;
        string value = gr.String();

        // Get the whole dictionary
        CacheDictionaryGetAllResponse gar = scc.DictionaryGetAll(cacheName: "my-cache", dictionaryName: "my-dictionary");
        status = gar.Status;
        dictionary = gar.Dictionary();
        value = dictionary["key1"];
    }
}
```
