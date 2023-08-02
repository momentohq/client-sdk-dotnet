<img src="https://docs.momentohq.com/img/logo.svg" alt="logo" width="400"/>

# Momento Load Generator Example

This repo includes a very basic load generator, to allow you to experiment with performance in your environment based on
different configurations. It's very simplistic, and only intended to give you a quick way to explore the performance of
the Momento client running in a single C# process.

You can find the code in [Program.cs](./Program.cs).  At the bottom of the file are several
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

## Prerequisites

* [`dotnet`](https://dotnet.microsoft.com/en-us/download) 6.0 or higher is required
* A Momento auth token is required.  You can generate one using the [Momento CLI](https://github.com/momentohq/momento-cli).

## Running the load generator

To run the load generator (from the `examples` directory):

```bash
# Run example load generator
MOMENTO_AUTH_TOKEN=<YOUR AUTH TOKEN> dotnet run --project MomentoLoadGen
```

Within the `MomentoLoadGen` directory you can run:

```bash
# Run example load generator
MOMENTO_AUTH_TOKEN=<YOUR AUTH TOKEN> dotnet run
```

If you make modifications to the code, remember to do a clean otherwise
the program might not run.

```bash
dotnet clean
MOMENTO_AUTH_TOKEN=<YOUR AUTH TOKEN> dotnet run
```
