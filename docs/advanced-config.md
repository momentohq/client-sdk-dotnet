<head>
  <meta name="Momento .NET Client Library Documentation" content=".NET client software development kit for Momento Serverless Cache">
</head>
<img src="https://docs.momentohq.com/img/logo.svg" alt="logo" width="400"/>

# Momento .NET Client Library: Advanced Configuration

## Changing the Client Timeout

If the only setting you need to change is the client-side timeout, you can do this on the pre-built configs via
the `WithClientTimeout` method:

```csharp
new SimpleCacheClient(
    Configurations.InRegion.Default.Latest().WithClientTimeout(TimeSpan.FromSeconds(2)),
    authProvider,
    defaultTtl
)
```

## Custom Configurations

In most cases we hope that our pre-built configurations will meet your needs.  However, if you do need to override
any of our default settings, you can create your own configuration object that implements our `IConfiguration`
interface.


The easiest way to do that is to use our `Configuration` class, which takes four arguments:

```csharp
new Configuration(loggerFactory, retryStrategy, middlewares, transportStrategy)
```

You can look at the source code in [Configurations.cs](../src/Momento.Sdk/Config/Configurations.cs) to see how our
pre-builts are constructed, which is a great starting point for custom configurations.

Here's more info on each of those arguments.

### loggerFactory

This is an instance of the .NET `ILoggerFactory`.  This will be used to configure logging for all Momento classes that
consume this `IConfiguration`.

### retryStrategy

Controls whether failed requests are retried, and how.  The interface for this argument is `IRetryStrategy`; we provide
some simple pre-built implementations such as `FixedCountRetryStrategy`, but you may provide your own implementation of
this interface for custom retry behavior.

### middlewares

Here you may provide zero or more instances of `IMiddleware`, which allow you to intercept requests and responses and
modify them as they are processed.  This could be used for logging debug or performance information, among other
possible use cases.  See `LoggingMiddleware` for a very simple example.

### transportStrategy

This configures the low-level networking settings for communicating with the Momento service.  See the `ITransportStrategy`
interface for more details.



