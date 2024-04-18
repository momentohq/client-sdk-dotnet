# Running tests

Unless you are testing older .NET runtimes on Windows, you should run the tests against the newer runtimes as follows:
  - https://dotnet.microsoft.com/en-us/download/dotnet/6.0

```
TEST_API_KEY=$your_momento_token make test-net6
```

To test against older .NET runtimes run:

```
TEST_API_KEY=$your_momento_token make test-net-framework
```

To run specific tests:

```
TEST_API_KEY=$your_momento_token dotnet test --framework net6.0 --filter "FullyQualifiedName~CacheDataTest"
```
