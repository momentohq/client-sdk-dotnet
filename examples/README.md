<img src="https://docs.momentohq.com/img/logo.svg" alt="logo" width="400"/>

# Momento .NET Client Examples

## Prerequisites

* [`dotnet`](https://dotnet.microsoft.com/en-us/download) SDK version 6 is required.
* A Momento auth token is required.  You can generate one using the [Momento CLI](https://github.com/momentohq/momento-cli).

## Running the advanced example

To run the advanced example code defined in [`MomentoApplication/Program.cs`](./MomentoApplication/Program.cs),
run the following from within the `examples` directory:

```bash
MOMENTO_AUTH_TOKEN=<YOUR AUTH TOKEN> dotnet run --project MomentoApplication
```

## Error Handling

Errors that occur in calls to SimpleCacheClient methods are surfaced to developers as part of the return values of
the calls, as opposed to by throwing exceptions.  This makes them more visible, and allows your IDE to be more
helpful in ensuring that you've handled the ones you care about.  (For more on our philosophy about this, see our
blog post on why [Exceptions are bugs](https://www.gomomento.com/blog/exceptions-are-bugs).  And send us any
feedback you have!)

The preferred way of interpreting the return values from SimpleCacheClient methods is using [Pattern matching](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching).  Here's a quick example:

```csharp
CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
if (getResponse is CacheGetResponse.Hit hitResponse)
{
    Console.WriteLine($"\nLooked up value: {hitResponse.String()}, Stored value: {VALUE}");
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

Note that, outside of SimpleCacheClient responses, exceptions can occur and should be handled as usual. For example, trying to instantiate a SimpleCacheClient with an invalid authentication token will result in an
IllegalArgumentException being thrown.

## Running the load generator example

This repo includes a very basic load generator, to allow you to experiment with performance in your environment based on
different configurations. It's very simplistic, and only intended to give you a quick way to explore the performance of
the Momento client running in a single C# process.

You can find the code in [MomentoLoadGen/Program.cs](./MomentoLoadGen/Program.cs).  At the bottom of the file are several
configuration options you can tune.  For example, you can experiment with the number of concurrent requests; increasing
this value will often increase throughput, but may impact client-side latency.  Likewise, decreasing the number of
concurrent requests may increase client-side latency if it means less competition for resources on the machine your
client is running on.

Performance will be impacted by network latency, so you'll get the best results if you run on a cloud VM in the same
region as your Momento cache.  The Momento client libraries ship with pre-built configurations that are tuned for
performance in different environments; look for the `IConfiguration` at the bottom of the loadgen code.  You may wish to
change that value from `Configurations.Laptop` to `Configurations.InRegion` if you will be running your client code
on a cloud VM.

If you have questions or need help experimenting further, please reach out to us at `support@momentohq.com`!

To run the load generator (from the `examples` directory):

```bash
# Run example load generator
MOMENTO_AUTH_TOKEN=<YOUR AUTH TOKEN> dotnet run --project MomentoLoadGen
```
