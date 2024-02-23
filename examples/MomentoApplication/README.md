<img src="https://docs.momentohq.com/img/logo.svg" alt="logo" width="400"/>

# Momento Application Example

This example is intended to show off the complete suite of Momento's cache client's
functionality, including:

* instantiating the Momento client
* creating, listing, and deleting caches
* setting and retrieving values for cache keys
* response processing and error handling

## Prerequisites

* [`dotnet`](https://dotnet.microsoft.com/en-us/download) 6.0 or higher is required
* A Momento auth token is required.  You can generate one using the [Momento CLI](https://github.com/momentohq/momento-cli).

## Running the application example

Run the following from within the `examples` directory:

```bash
MOMENTO_API_KEY=<YOUR AUTH TOKEN> dotnet run --project MomentoApplication
```

Within the `MomentoAppication` directory you can run:

```bash
MOMENTO_API_KEY=<YOUR AUTH TOKEN> dotnet run
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
