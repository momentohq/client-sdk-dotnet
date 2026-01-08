{{ ossHeader }}

Japanese: [日本語](README.ja.md)

## Packages

The Momento Dotnet SDK package is available on nuget: [momentohq/client-sdk-dotnet](https://www.nuget.org/packages/Momento.Sdk).

## Prerequisites

- [`dotnet`](https://dotnet.microsoft.com/en-us/download) 6.0 or higher is required
- A [Momento API key](https://docs.momentohq.com/cache/develop/authentication/api-keys) is required.  You can generate one using the [Momento Console](https://console.gomomento.com/api-keys).
- A Momento service endpoint is required. Choose the one for the [region](https://docs.momentohq.com/platform/regions) you'll be using, e.g. `cell-1-ap-southeast-1-1.prod.a.momentohq.com` for ap-southeast-1

## Usage

Here is a quickstart you can use in your own project:

```csharp
{% include "./examples/MomentoUsage/Program.cs" %}
```

## Getting Started and Documentation

Documentation is available on the [Momento Docs website](https://docs.momentohq.com).

## Examples

Ready to dive right in? Just check out the [examples](./examples/README.md) directory for complete, working examples of
how to use the SDK.

## Developing

If you are interested in contributing to the SDK, please see the [CONTRIBUTING](./CONTRIBUTING.md) docs.

{{ ossFooter }}
