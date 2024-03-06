# Topic Example

This example program demonstrates usage of Momento Topics.

# Usage

The program assumes the auth token and cache names are available in environment variables. The auth token is assumed to be in the variable `MOMENTO_API_KEY` and the cache name in `MOMENTO_CACHE_NAME`. If either of these is missing, you will be prompted to enter the values on the terminal.

To run the program, run either:

```bash
MOMENTO_API_KEY=<YOUR_API_KEY_HERE> MOMENTO_CACHE_NAME=<YOUR_CACHE_NAME_HERE> dotnet run
```

or

```bash
dotnet run
```

and you will be prompted to enter the auth token and cache name.

If the cache name entered does not exist, the program will create it.

The example publishes one message per second to a topic for 10 seconds. It subscribes to the same topic and prints each message it receives.