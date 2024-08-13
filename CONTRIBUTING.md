# Running tests

## Recommended

The Makefile target `test` runs against .NET 6.0 and has additional OS-conditional logic to run .NET Framework tests on Windows. Use this target by default.

## Specifics

You can explicitly run the tests against the newer runtimes as follows:

- https://dotnet.microsoft.com/en-us/download/dotnet/6.0

```
MOMENTO_API_KEY=$your_momento_token make test-dotnet6
```

To test against older .NET runtimes run:

```
MOMENTO_API_KEY=$your_momento_token make test-dotnet-framework
```

To run specific tests:

```
MOMENTO_API_KEY=$your_momento_token dotnet test --framework net6.0 --filter "FullyQualifiedName~CacheDataTest"
```
