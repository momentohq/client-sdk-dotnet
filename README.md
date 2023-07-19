<head>
  <meta name="Momento .NET Client Library Documentation" content=".NET client software development kit for Momento Cache">
</head>
<img src="https://docs.momentohq.com/img/logo.svg" alt="logo" width="400"/>

[![project status](https://momentohq.github.io/standards-and-practices/badges/project-status-official.svg)](https://github.com/momentohq/standards-and-practices/blob/main/docs/momento-on-github.md)
[![project stability](https://momentohq.github.io/standards-and-practices/badges/project-stability-stable.svg)](https://github.com/momentohq/standards-and-practices/blob/main/docs/momento-on-github.md)

# Momento .NET Client Library

Momento Cache is a fast, simple, pay-as-you-go caching solution without any of the operational overhead
required by traditional caching solutions.  This repo contains the source code for the Momento .NET client library.

To get started with Momento you will need a Momento Auth Token. You can get one from the [Momento Console](https://console.gomomento.com).

* Website: [https://www.gomomento.com/](https://www.gomomento.com/)
* Momento Documentation: [https://docs.momentohq.com/](https://docs.momentohq.com/)
* Getting Started: [https://docs.momentohq.com/getting-started](https://docs.momentohq.com/getting-started)
* .NET SDK Documentation: [https://docs.momentohq.com/develop/sdks/dotnet](https://docs.momentohq.com/develop/sdks/dotnet)
* Discuss: [Momento Discord](https://discord.gg/3HkAKjUZGq)

Japanese: [日本語](README.ja.md)

## Packages

The Momento Dotnet SDK package is available on nuget: [momentohq/client-sdk-dotnet](https://www.nuget.org/packages/Momento.Sdk).

## Usage

Here is a quickstart you can use in your own project:

```csharp
﻿using System;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;

ICredentialProvider authProvider = new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN");
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

```

Note that the above code requires an environment variable named MOMENTO_AUTH_TOKEN which must
be set to a valid [Momento authentication token](https://docs.momentohq.com/docs/getting-started#obtain-an-auth-token).

## Getting Started and Documentation

Documentation is available on the [Momento Docs website](https://docs.momentohq.com).

## Examples

Ready to dive right in? Just check out the [examples](./examples/README.md) directory for complete, working examples of
how to use the SDK.

## Developing

If you are interested in contributing to the SDK, please see the [CONTRIBUTING](./CONTRIBUTING.md) docs.

----------------------------------------------------------------------------------------
For more info, visit our website at [https://gomomento.com](https://gomomento.com)!
