<img src="https://docs.momentohq.com/img/logo.svg" alt="logo" width="400"/>

# Disposable Tokens Example

This example program demonstrates how to generate disposable Momento auth tokens.

# Usage

The program assumes that your Momento auth token is available in the `V1_API_KEY` environment variable:

```bash
V1_API_KEY=<YOUR_API_KEY> dotnet run
```

The example generates a disposable expiring auth token using the enumerated permissions and expiry defined in the program and prints its attributes to the console.

Note: this example uses a legacy API key because the AuthClient does not yet support v2 API keys.