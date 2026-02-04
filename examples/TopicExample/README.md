# Topic Example

This example program demonstrates usage of Momento Topics.

## Prerequisites

* [`dotnet`](https://dotnet.microsoft.com/en-us/download) 6.0 or higher is required
* A Momento API key is required.  You can generate one using the [Momento Console](https://console.gomomento.com/api-keys).
* A Momento service endpoint is required. Choose the one for the [region](https://docs.momentohq.com/platform/regions) you'll be using, e.g. `cell-1-ap-southeast-1-1.prod.a.momentohq.com` for ap-southeast-1.

# Usage

To run the program, run either:

```bash
MOMENTO_API_KEY=<YOUR_API_KEY_HERE> MOMENTO_ENDPOINT=<YOUR_ENDPOINT> MOMENTO_CACHE_NAME=<YOUR_CACHE_NAME_HERE> dotnet run
```

If the cache name entered does not exist, the program will create it.

The example publishes one message per second to a topic for 10 seconds. It subscribes to the same topic and prints each message it receives.