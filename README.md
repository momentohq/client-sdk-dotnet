# Momento client-sdk-csharp

:warning: Experimental SDK :warning:

C# SDK for Momento is experimental and under active development.
There could be non-backward compatible changes or removal in the future.
Please be aware that you may need to update your source code with the current version of the SDK when its version gets upgraded.

---

<br/>

C# SDK for Momento, a serverless cache that automatically scales without any of the operational overhead required by traditional caching solutions.

<br/>

## Getting Started :running:

### Requirements
1. brew install nuget
1. brew install --cask dotnet
1. [Visual Studio](https://visualstudio.microsoft.com/vs/mac/)
1. [.NET](https://docs.microsoft.com/en-us/dotnet/core/install/macos)

### How to run
1. dotnet nuget add source https://momento.jfrog.io/artifactory/api/nuget/nuget-public --name Artifactory
1. dotnet build

### How to install
1. dotnet add package MomentoSdk -s https://momento.jfrog.io/artifactory/api/nuget/nuget-public
