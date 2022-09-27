{{ ossHeader }}

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
{{ usageExampleCode }}
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
}
```

Using this approach, you get a type-safe `hitResponse` object in the case of a cache hit.  But if the cache read
results in a Miss or an error, you'll also get a type-safe object that you can use to get more info about what happened.

### Tuning

Momento client-libraries provide pre-built configuration bundles out-of-the-box.  We want to do the hard work of
tuning for different environments for you, so that you can focus on the things that are unique to your business.
(We even have a blog series about it!  [Shockingly simple: Cache clients that do the hard work for you](https://www.gomomento.com/blog/shockingly-simple-cache-clients-that-do-the-hard-work-for-you))

You can find the pre-built configurations in our `Configurations` namespace.  Some of the pre-built configurations that
you might be interested in:

* `Configurations.Laptop` - this one is a development environment, just for poking around.  It has relaxed timeouts
      and assumes that your network latencies might be a bit high.
* `Configurations.InRegion.Default` - provides defaults suitable for an environment where your client is running in the same region as the Momento
      service.  It has more aggressive timeouts and retry behavior than the Laptop config.
* `Configurations.InRegion.LowLatency` - This config prioritizes keeping p99.9 latencies as low as possible, potentially sacrificing
      some throughput to achieve this.  Use this configuration if the most important factor is to ensure that cache
      unavailability doesn't force unacceptably high latencies for your own application.

{{ ossFooter }}