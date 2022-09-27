<img src="https://docs.momentohq.com/img/logo.svg" alt="logo" width="400"/>

[![project status](https://momentohq.github.io/standards-and-practices/badges/project-status-official.svg)](https://github.com/momentohq/standards-and-practices/blob/main/docs/momento-on-github.md)
[![project stability](https://momentohq.github.io/standards-and-practices/badges/project-stability-experimental.svg)](https://github.com/momentohq/standards-and-practices/blob/main/docs/momento-on-github.md) 

# Momento .NET Client Library


:warning: Experimental SDK :warning:

This is an official Momento SDK, but the API is in an early experimental stage and subject to backward-incompatible
changes.  For more info, click on the experimental badge above.


.NET client SDK for Momento Serverless Cache: a fast, simple, pay-as-you-go caching solution without
any of the operational overhead required by traditional caching solutions!



## Getting Started :running:

### Requirements

- brew install nuget 
- brew install --cask dotnet
- [Visual Studio](https://visualstudio.microsoft.com/vs/mac/)
- [.NET](https://docs.microsoft.com/en-us/dotnet/core/install/macos)

### Installation

```bash
dotnet add package Momento.Sdk
```

### Usage

Checkout our [examples](./examples/README.md) directory for complete examples of how to use the SDK.

Here is a quickstart you can use in your own project:

```csharp
﻿using System;
using System.Threading.Tasks;
using Momento.Sdk;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;
using Momento.Sdk.Config;
using Microsoft.Extensions.Logging;

namespace MomentoApplication
{
    class Program
    {
        static readonly String MOMENTO_AUTH_TOKEN = Environment.GetEnvironmentVariable("MOMENTO_AUTH_TOKEN");
        static readonly String CACHE_NAME = "cache";
        static readonly String KEY = "MyKey";
        static readonly String VALUE = "MyData";
        static readonly uint DEFAULT_TTL_SECONDS = 60;

        async static Task Main(string[] args)
        {
            using SimpleCacheClient client = new SimpleCacheClient(Configurations.Laptop.Latest, MOMENTO_AUTH_TOKEN, DEFAULT_TTL_SECONDS);
            var createCacheResult = client.CreateCache(CACHE_NAME);
            
            Console.WriteLine("Listing caches:");
            String token = null;
            do
            {
                ListCachesResponse resp = client.ListCaches(token);
                if (resp is ListCachesResponse.Success successResult)
                {
                    foreach (CacheInfo cacheInfo in successResult.Caches)
                    {
                        Console.WriteLine(cacheInfo.Name);
                    }
                    token = successResult.NextPageToken;
                }
            } while (!String.IsNullOrEmpty(token));
            Console.WriteLine($"\nSetting key: {KEY} with value: {VALUE}");
            await client.SetAsync(CACHE_NAME, KEY, VALUE);
            Console.WriteLine($"\nGet value for  key: {KEY}");
            CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
            if (getResponse is CacheGetResponse.Hit hitResponse)
            {
                Console.WriteLine($"\nLookedup value: {hitResponse.String()}, Stored value: {VALUE}");
            }
        }
    }
}

```

### Error Handling

Error cases in Momento are surfaced to developers as part of the return values of the method calls, as opposed
to by throwing exceptions.  This makes them more visible, and allows your IDE to be more helpful in ensuring that
you've handled the ones you care about.  (For more on our philosophy about this, see our blog post on why
[Exceptions are bugs](https://www.gomomento.com/blog/exceptions-are-bugs).  And send us any feedback you have!)

The preferred way of interpreting the return values from the Momento .NET methods is using [Pattern matching](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching).  Here's a quick example:

```csharp
CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
if (getResponse is CacheGetResponse.Hit hitResponse)
{
    Console.WriteLine($"\nLookedup value: {hitResponse.String()}, Stored value: {VALUE}");
} else {
      // you can handle other cases via pattern matching in `else if` blocks, or a default case
      // via the `else` block.  For each return value your IDE should be able to give you code
      // completion indicating the other possible types; in this case, `CacheGetResponse.Miss` and
      // `CacheGetResponse.Error`.
}
```

Using this approach, you get a type-safe `hitResponse` object in the case of a cache hit.  But if the cache read
results in a Miss or an error, you'll also get a type-safe object that you can use to get more info about what happened.

In cases where you get an error response, `Error` types will always include an `ErrorCode` that you can use to check
the error type:

```csharp
CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
if (getResponse is CacheGetResponse.Error errorResponse)
{
    if (errorResponse.ErrorCode == MomentoErrorCode.TIMEOUT_ERROR) {
       // this would represent a client-side timeout, and you could fall back to your original data source
    }
}
```

### Tuning

Momento client-libraries provide pre-built configuration bundles out-of-the-box.  We want to do the hard work of
tuning for different environments for you, so that you can focus on the things that are unique to your business.
(We even have a blog series about it!  [Shockingly simple: Cache clients that do the hard work for you](https://www.gomomento.com/blog/shockingly-simple-cache-clients-that-do-the-hard-work-for-you))

You can find the pre-built configurations in our `Configurations` namespace.  Some of the pre-built configurations that
you might be interested in:

- `Configurations.Laptop` - this one is a development environment, just for poking around.  It has relaxed timeouts
      and assumes that your network latencies might be a bit high.
- `Configurations.InRegion.Default` - provides defaults suitable for an environment where your client is running in the same region as the Momento
      service.  It has more aggressive timeouts and retry behavior than the Laptop config.
- `Configurations.InRegion.LowLatency` - This config prioritizes keeping p99.9 latencies as low as possible, potentially sacrificing
      some throughput to achieve this.  Use this configuration if the most important factor is to ensure that cache
      unavailability doesn't force unacceptably high latencies for your own application.

----------------------------------------------------------------------------------------
For more info, visit our website at [https://gomomento.com](https://gomomento.com)!
