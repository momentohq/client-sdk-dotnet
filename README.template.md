{{ ossHeader }}

## Getting Started :running:

### Requirements

You will need the [`dotnet` runtime and command line tools](https://dotnet.microsoft.com/en-us/download).  After installing them, you should have the `dotnet` command on your PATH.

**IDE Notes**: You will most likely want an IDE that supports .NET development, such as [Microsoft Visual Studio](https://visualstudio.microsoft.com/vs), [JetBrains Rider](https://www.jetbrains.com/rider/), or [Microsoft Visual Studio Code](https://code.visualstudio.com/).

### Examples

Ready to dive right in?  Just check out the [examples](./examples/README.md) directory for complete, working examples of
how to use the SDK.

### Momento Response Types

The return values of the methods on the Momento `SimpleCacheClient` class are designed to allow you to use your
IDE to help you easily discover all the possible responses, including errors.  We use [pattern matching](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching) to distinguish between different types of responses,
which means that you can get compile-time safety when interacting with the API, rather than having bugs sneak in at runtime.

Here's an example:

```csharp
CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
if (getResponse is CacheGetResponse.Hit hitResponse)
{
  Console.WriteLine($"Looked up value: {hitResponse.ValueString}, Stored value: {VALUE}");
}
else if (getResponse is CacheGetResponse.Error getError)
{
  Console.WriteLine($"Error getting value: {getError.Message}");
}
```

See the [Error Handling](#error-handling) section below for more details.


### Installation

To create a new .NET project and add the Momento client library as a dependency:

```bash
mkdir my-momento-dotnet-project
cd my-momento-dotnet-project
dotnet new console
dotnet add package Momento.Sdk
```

### Usage

Here is a quickstart you can use in your own project:

```csharp
{{ usageExampleCode }}
```

Note that the above code requires an environment variable named MOMENTO_AUTH_TOKEN which must
be set to a valid [Momento authentication token](https://docs.momentohq.com/docs/getting-started#obtain-an-auth-token).

### Error Handling

Error that occur in calls to SimpleCacheClient methods are surfaced to developers as part of the return values of
the calls, as opposed to by throwing exceptions.  This makes them more visible, and allows your IDE to be more
helpful in ensuring that you've handled the ones you care about.  (For more on our philosophy about this, see our
blog post on why [Exceptions are bugs](https://www.gomomento.com/blog/exceptions-are-bugs).  And send us any
feedback you have!)

The preferred way of interpreting the return values from SimpleCacheClient methods is using [Pattern matching](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching).  Here's a quick example:

```csharp
CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
if (getResponse is CacheGetResponse.Hit hitResponse)
{
    Console.WriteLine($"\nLooked up value: {hitResponse.ValueString}, Stored value: {VALUE}");
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

Note that, outside of SimpleCacheClient responses, exceptions can occur and should be handled as usual. For example, trying
to instantiate a SimpleCacheClient with an invalid authentication token will result in an IllegalArgumentException being thrown.

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

We hope that these configurations will meet the needs of most users, but if you find them lacking in any way, please
open a github issue, or contact us at `support@momentohq.com`.  We would love to hear about your use case so that we
can fix or extend the pre-built configs to support it.

If you do need to customize your configuration beyond what our pre-builts provide, see the
[Advanced Configuration Guide](./docs/advanced-config.md).


{{ ossFooter }}
