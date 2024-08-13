.PHONY: all build build-dotnet6 build-dotnet-framework clean clean-build precommit restore test test-dotnet6 test-dotnet-framework run-examples help

# Determine the operating system
OS := $(shell uname)

# Set the default .NET version to .NET 6.0
DOTNET_VERSION := net6.0
TEST_LOGGER_OPTIONS := --logger "console;verbosity=detailed"

# Windows-specific settings
ifeq ($(OS), Windows_NT)
    BUILD_TARGETS := build-dotnet6 build-dotnet-framework
    TEST_TARGETS := test-dotnet6 test-dotnet-framework
else
    BUILD_TARGETS := build-dotnet6
    TEST_TARGETS := test-dotnet6
endif

# Enable gRPC-Web if requested
GRPC_WEB_FLAG :=
ifeq ($(GRPC_WEB), true)
    GRPC_WEB_FLAG := -p:DefineConstants=USE_GRPC_WEB
endif

## Generate sync unit tests, format, lint, and test
all: precommit


## Build the project (conditioned by OS)
build: ${BUILD_TARGETS}


## Build the project for .NET 6.0
build-dotnet6:
	@echo "Building the project for .NET 6.0..."
	@dotnet build -f ${DOTNET_VERSION} ${GRPC_WEB_FLAG}


## Build the project on .NET Framework
build-dotnet-framework:
	@echo "Building the project for .NET Framework 4.62..."
	@dotnet build -f net462 ${GRPC_WEB_FLAG}

## Remove build files
clean:
	@echo "Cleaning build artifacts..."
	@dotnet clean


## Build project
clean-build: clean restore ${BUILD_TARGETS}


## Run clean-build and test as a step before committing.
precommit: clean-build test


## Sync dependencies
restore:
	@echo "Restoring dependencies..."
	@dotnet restore


## Run unit and integration tests (conditioned by OS)
test: ${TEST_TARGETS}


## Run unit and integration tests on the .NET 6.0 runtime
test-dotnet6:
	@echo "Running tests on .NET 6.0..."
	@dotnet test ${TEST_LOGGER_OPTIONS} -f ${DOTNET_VERSION}


## Run unit and integration tests on the .NET Framework runtime (Windows only)
test-dotnet-framework:
	@echo "Running tests on .NET Framework 4.62 (Windows only)..."
	@dotnet test ${TEST_LOGGER_OPTIONS} -f net462


## Run example applications and snippets
run-examples:
	@dotnet run --project examples/MomentoApplication
	@dotnet run --project examples/DocExampleApis

# See <https://gist.github.com/klmr/575726c7e05d8780505a> for explanation.
help:
	@echo "$$(tput bold)Available rules:$$(tput sgr0)";echo;sed -ne"/^## /{h;s/.*//;:d" -e"H;n;s/^## //;td" -e"s/:.*//;G;s/\\n## /---/;s/\\n/ /g;p;}" ${MAKEFILE_LIST}|LC_ALL='C' sort -f|awk -F --- -v n=$$(tput cols) -v i=19 -v a="$$(tput setaf 6)" -v z="$$(tput sgr0)" '{printf"%s%*s%s ",a,-i,$$1,z;m=split($$2,w," ");l=n-i;for(j=1;j<=m;j++){l-=length(w[j])+1;if(l<= 0){l=n-i-length(w[j])-1;printf"\n%*s ",-i," ";}printf"%s ",w[j];}printf"\n";}'|more $(shell test $(shell uname) == Darwin && echo '-Xr')
