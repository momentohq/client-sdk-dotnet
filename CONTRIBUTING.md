# Running tests

Unless you are testing older .NET runtimes on Windows, you should run the tests against the newer runtimes as follows:

```
make test-net6
```

To test against older .NET runtimes run:

```
make test-net-framework
```

To run specific tests:

```
dotnet test -f net6.0 --filter "FullyQualifiedName~CacheDataTest"
```
