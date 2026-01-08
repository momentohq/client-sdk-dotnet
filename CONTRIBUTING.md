# Running tests

## Prerequisites

* [`dotnet`](https://dotnet.microsoft.com/en-us/download) 6.0 or higher is required
* A Momento API key is required.  You can generate one using the [Momento Console](https://console.gomomento.com/api-keys).
* A Momento service endpoint is required. Choose the one for the [region](https://docs.momentohq.com/platform/regions) you'll be using, e.g. `cell-1-ap-southeast-1-1.prod.a.momentohq.com` for ap-southeast-1 or `cell-alpha-dev.preprod.a.momentohq.com` for alpha.

```shell
export MOMENTO_API_KEY=<v2 api key>
export MOMENTO_ENDPOINT=<endpoint>
export V1_API_KEY=<v1 api key>
```

## Recommended

The Makefile target `test` runs against .NET 6.0 and has additional OS-conditional logic to run .NET Framework tests on Windows. Use this target by default.

## Specifics

You can explicitly run the tests against the newer runtimes as follows:

- https://dotnet.microsoft.com/en-us/download/dotnet/6.0

```
make test-dotnet6
```

To test against older .NET runtimes run:

```
make test-dotnet-framework
```

To run specific tests:

```
dotnet test --framework net6.0 --filter "FullyQualifiedName~CacheDataTest"
```
