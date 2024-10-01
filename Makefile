# Note on the structure of this Makefile:
# - We build the project on both .NET 6.0 and .NET Framework 4.62. The latter of which is only available on Windows.
#   - We still test on Linux. That means we can't run the .NET Framework tests on Linux, but we need to run both .NET 6.0 and .NET Framework tests on Windows.
# - Because of this, we must conditionally run certain build and test targets based on the operating system.
#   - We split the build and test targets are split into two categories: .NET 6.0 and .NET Framework.
#   - At the top we detect the operating system and set the appropriate build and test targets.
#   - On Windows `make build` (test) runs both .NET 6.0 and .NET Framework build (test) targets.
#   - On other operating systems `make build` (test) we only runs the .NET 6.0 build (test) targets.
# - We also have a GRPC_WEB flag that can be set to true to enable gRPC-Web support.
#   - The caller can run `make GRPC_WEB=true build` to enable gRPC-Web support.
# - We additionally group the integration tests by service (auth, cache, topics).
#   - This is to allow for more granular testing by service.
#   - Similar to `build` and `test` targets, we have `test-cache-service`, `test-topics-service`, `test-auth-service` targets
#	  that are conditionally run based on the operating system.

.PHONY: all build build-dotnet6 build-dotnet-framework clean clean-build precommit restore test \
	test-dotnet6 test-dotnet6-integration test-dotnet6-cache-service test-dotnet6-topics-service test-dotnet6-auth-service \
	test-dotnet-framework test-dotnet-framework-integration test-dotnet-framework-cache-service test-dotnet-framework-topics-service test-dotnet-framework-auth-service \
	test-auth-service test-cache-service test-leaderboard-service test-storage-service test-topics-service  \
	run-examples help

# Determine the operating system
OS := $(shell uname)

# Set the default .NET version to .NET 6.0
DOTNET_VERSION := net6.0
DOTNET_FRAMEWORK_VERSION := net462
TEST_LOGGER_OPTIONS := --logger "console;verbosity=detailed"

# Windows-specific settings
# This tests if "NT" is in the OS string, which would indicate Windows.
ifneq (,$(findstring NT,$(OS)))
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

# Various test filters
CACHE_SERVICE_TESTS_FILTER := "FullyQualifiedName~Momento.Sdk.Tests.Integration.Cache"
TOPICS_SERVICE_TESTS_FILTER := "FullyQualifiedName~Momento.Sdk.Tests.Integration.Topics"
AUTH_SERVICE_TESTS_FILTER := "FullyQualifiedName~Momento.Sdk.Tests.Integration.Auth"


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


## Run unit and integration tests with consistent reads (conditioned by OS)
prod-test:
	@echo "running tests with consistent reads..."
ifeq (,$(findstring NT,$(OS)))
	@CONSISTENT_READS=1 $(MAKE) ${TEST_TARGETS}
else
	@set CONSISTENT_READS=1 && $(MAKE) ${TEST_TARGETS}
endif


## Run unit and integration tests on the .NET 6.0 runtime
test-dotnet6:
	@echo "Running unit and integration tests on the .NET 6.0 runtime..."
	@dotnet test ${TEST_LOGGER_OPTIONS} -f ${DOTNET_VERSION}


## Run integration tests on the .NET 6.0 runtime against the cache service
test-dotnet6-cache-service:
	@echo "Running integration tests on the .NET 6.0 runtime against the cache service..."
	@dotnet test ${TEST_LOGGER_OPTIONS} -f ${DOTNET_VERSION} --filter ${CACHE_SERVICE_TESTS_FILTER}


## Run integration tests on the .NET 6.0 runtime against the topics service
test-dotnet6-topics-service:
	@echo "Running integration tests on the .NET 6.0 runtime against the topics service..."
	@dotnet test ${TEST_LOGGER_OPTIONS} -f ${DOTNET_VERSION} --filter ${TOPICS_SERVICE_TESTS_FILTER}


## Run integration tests on the .NET 6.0 runtime against the auth service
test-dotnet6-auth-service:
	@echo "Running integration tests on the .NET 6.0 runtime against the auth service..."
	@dotnet test ${TEST_LOGGER_OPTIONS} -f ${DOTNET_VERSION} --filter ${AUTH_SERVICE_TESTS_FILTER}


## Run unit and integration tests on the .NET Framework runtime (Windows only)
test-dotnet-framework:
	@echo "Running unit and integration tests on the .NET Framework runtime..."
	@dotnet test ${TEST_LOGGER_OPTIONS} -f ${DOTNET_FRAMEWORK_VERSION}


## Run integration tests on the .NET Framework runtime against the cache service (Windows only)
test-dotnet-framework-cache-service:
	@echo "Running integration tests on the .NET Framework runtime against the cache service..."
	@dotnet test ${TEST_LOGGER_OPTIONS} -f ${DOTNET_FRAMEWORK_VERSION} --filter ${CACHE_SERVICE_TESTS_FILTER}


## Run integration tests on the .NET Framework runtime against the topics service (Windows only)
test-dotnet-framework-topics-service:
	@echo "Running integration tests on the .NET Framework runtime against the topics service..."
	@dotnet test ${TEST_LOGGER_OPTIONS} -f ${DOTNET_FRAMEWORK_VERSION} --filter ${TOPICS_SERVICE_TESTS_FILTER}


## Run integration tests on the .NET Framework runtime against the auth service (Windows only)
test-dotnet-framework-auth-service:
	@echo "Running integration tests on the .NET Framework runtime against the auth service..."
	@dotnet test ${TEST_LOGGER_OPTIONS} -f ${DOTNET_FRAMEWORK_VERSION} --filter ${AUTH_SERVICE_TESTS_FILTER}


## Run auth service tests
test-auth-service:
ifeq (,$(findstring NT,$(OS)))
	@CONSISTENT_READS=1 $(MAKE) test-dotnet6-auth-service
else
	@set CONSISTENT_READS=1 && $(MAKE) test-dotnet6-auth-service test-dotnet-framework-auth-service
endif


## Run cache service tests
test-cache-service:
ifeq (,$(findstring NT,$(OS)))
	@CONSISTENT_READS=1 $(MAKE) test-dotnet6-cache-service
else
	@set CONSISTENT_READS=1 && $(MAKE) test-dotnet6-cache-service test-dotnet-framework-cache-service
endif


## Run leaderboard service tests
test-leaderboard-service:
	@echo "Leaderboard client not implemented yet."


## Run storage service tests
test-storage-service:
	@echo "Storage client not implemented yet."


## Run topics service tests
test-topics-service:
ifeq (,$(findstring NT,$(OS)))
	@CONSISTENT_READS=1 $(MAKE) test-dotnet6-topics-service
else
	@set CONSISTENT_READS=1 && $(MAKE) test-dotnet6-topics-service test-dotnet-framework-topics-service
endif


## Run example applications and snippets
run-examples:
	@dotnet run --project examples/MomentoApplication
	@dotnet run --project examples/DocExampleApis

# See <https://gist.github.com/klmr/575726c7e05d8780505a> for explanation.
help:
	@echo "$$(tput bold)Available rules:$$(tput sgr0)";echo;sed -ne"/^## /{h;s/.*//;:d" -e"H;n;s/^## //;td" -e"s/:.*//;G;s/\\n## /---/;s/\\n/ /g;p;}" ${MAKEFILE_LIST}|LC_ALL='C' sort -f|awk -F --- -v n=$$(tput cols) -v i=19 -v a="$$(tput setaf 6)" -v z="$$(tput sgr0)" '{printf"%s%*s%s ",a,-i,$$1,z;m=split($$2,w," ");l=n-i;for(j=1;j<=m;j++){l-=length(w[j])+1;if(l<= 0){l=n-i-length(w[j])-1;printf"\n%*s ",-i," ";}printf"%s ",w[j];}printf"\n";}'|more $(shell test $(shell uname) == Darwin && echo '-Xr')
