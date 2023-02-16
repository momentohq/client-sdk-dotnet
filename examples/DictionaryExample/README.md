# Dictionary Example Program

This example program demonstrates usage of the dictionary data type.

# Usage

The program assumes the auth token and cache names are available in environment variables. The auth token is assumed to be in the variable `TEST_AUTH_TOKEN` and the cache name in `TEST_CACHE_NAME`. If either of these is missing, you will be prompted to enter the values on the terminal.

To run the program, run either:

```bash
TEST_AUTH_TOKEN=<YOUR_TOKEN_HERE> TEST_CACHE_NAME=<YOUR_CACHE_NAME_HERE> dotnet run
```

or

```bash
dotnet run
```

and you will be prompted to enter the auth token and cache name.

If the cache name entered does not exist, the program will create it.
