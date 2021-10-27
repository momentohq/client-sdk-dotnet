## C-Sharp sdk

### Prerequisites
1. brew install nuget
1. brew install --cask dotnet
1. [Visual Studio](https://visualstudio.microsoft.com/vs/mac/)
1. [.NET](https://docs.microsoft.com/en-us/dotnet/core/install/macos)

### How to run
1. dotnet nuget add source https://momento.jfrog.io/artifactory/api/nuget/nuget-public --name Artifactory
1. dotnet build

### How to install
1. dotnet nuget add source https://momento.jfrog.io/artifactory/api/nuget/nuget-public --name Artifactory
1. dotnet nuget install MomentoSdk -Source Artifactory
