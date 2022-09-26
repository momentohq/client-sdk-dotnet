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
            using SimpleCacheClient client = new SimpleCacheClient(MOMENTO_AUTH_TOKEN, DEFAULT_TTL_SECONDS);
            try
            {
                client.CreateCache(CACHE_NAME);
            }
            catch (AlreadyExistsException)
            {
                Console.WriteLine($"Cache with name {CACHE_NAME} already exists.\n");
            }
            Console.WriteLine("Listing caches:");
            String token = null;
            do
            {
                ListCachesResponse resp = client.ListCaches(token);
                foreach (CacheInfo cacheInfo in resp.Caches)
                {
                    Console.WriteLine(cacheInfo.Name);
                }
                token = resp.NextPageToken;
            } while (!String.IsNullOrEmpty(token));
            Console.WriteLine($"\nSetting key: {KEY} with value: {VALUE}");
            await client.SetAsync(CACHE_NAME, KEY, VALUE);
            Console.WriteLine($"\nGet value for  key: {KEY}");
            CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
            Console.WriteLine($"\nLookedup value: {getResponse.String()}, Stored value: {VALUE}");
        }
    }
}

```

----------------------------------------------------------------------------------------
For more info, visit our website at [https://gomomento.com](https://gomomento.com)!
