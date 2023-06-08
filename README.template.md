{{ ossHeader }}

Japanese: [日本語](README.ja.md)

## Packages

The Momento Dotnet SDK package is available on nuget: [momentohq/client-sdk-dotnet](https://www.nuget.org/packages/Momento.Sdk).

## Usage

Here is a quickstart you can use in your own project:

```csharp
{% include "./examples/MomentoUsage/Program.cs" %}
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

{{ ossFooter }}
