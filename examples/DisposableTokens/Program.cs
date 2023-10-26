using System;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

static class Program
{

    private static readonly ICredentialProvider authProvider;
    private static readonly IAuthClient client;
    private const int Calls = 100;

    static Program()
    {
        authProvider = new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN");
        client = new AuthClient(AuthConfigurations.Default.Latest(), authProvider);
    }

    private static async Task<GenerateDisposableTokenResponse.Success> GenerateTokenWithEnumeratedPermissions()
    {
        var scope = new DisposableTokenScope(Permissions: new List<DisposableTokenPermission>
        {
            new DisposableToken.CacheItemPermission(
                CacheRole.ReadWrite,
                CacheSelector.ByName("cache"),
                CacheItemSelector.AllCacheItems
            ),
            new DisposableToken.CachePermission(
                CacheRole.ReadOnly,
                CacheSelector.ByName("topsecret")
            ),
            new DisposableToken.TopicPermission(
                TopicRole.PublishSubscribe,
                CacheSelector.ByName("cache"),
                TopicSelector.ByName("topic")
            )
        });

        var tokenResponse = await client.GenerateDisposableTokenAsync(
            scope,
            ExpiresIn.Minutes(30)
        );
        return ProcessResponse(tokenResponse);
    }

    private static async Task<GenerateDisposableTokenResponse.Success> GenerateTokenWithPredefinedScope()
    {
        var tokenResponse = await client.GenerateDisposableTokenAsync(
            // many predefined scopes are available to easily control access to caches, cache keys, and topics
            DisposableTokenScopes.CacheKeyReadWrite("cache", "cache-key"),
            ExpiresIn.Minutes(30)
        );
        return ProcessResponse(tokenResponse);
    }

    private static GenerateDisposableTokenResponse.Success ProcessResponse(GenerateDisposableTokenResponse tokenResponse)
    {
        if (tokenResponse is GenerateDisposableTokenResponse.Success token)
        {
            return token;
        }
        if (tokenResponse is GenerateDisposableTokenResponse.Error err)
        {
            throw err.InnerException;
        }
        throw new Exception("Unknown response type: " + tokenResponse);
    }

    private static void PrintTokenInfo(GenerateDisposableTokenResponse.Success token)
    {
        Console.WriteLine(
            "The generated disposable token (truncated): "
            + token.AuthToken.Substring(0, 10)
            + "..."
            + token.AuthToken.Substring((token.AuthToken.Length - 10), 10)
        );
        Console.WriteLine("The token expires at (epoch timestamp): " + token.ExpiresAt.Epoch() + "\n");
    }

    public async static Task Main()
        {
            List<double> elapsedTimes = new List<double>();

            for (int i = 0; i < Calls; i++)
            {
                var stopwatch = Stopwatch.StartNew();
                var token = await GenerateTokenWithEnumeratedPermissions();
                stopwatch.Stop();
                elapsedTimes.Add(stopwatch.Elapsed.TotalMilliseconds);
            }

            // Calculate statistics
            double minTime = elapsedTimes.Min();
            double maxTime = elapsedTimes.Max();
            double avgTime = elapsedTimes.Average();
            double p99Time = Percentile(elapsedTimes, 0.99);

            // Print statistics
            Console.WriteLine($"Min time: {minTime} ms");
            Console.WriteLine($"Avg time: {avgTime} ms");
            Console.WriteLine($"99th percentile time: {p99Time} ms");
            Console.WriteLine($"Max time: {maxTime} ms");
        }

        private static double Percentile(List<double> sequence, double percentile)
        {
            sequence.Sort();
            int N = sequence.Count;
            double n = (N - 1) * percentile + 1;

            if (n == 1d)
            {
                return sequence[0];
            }
            else if (n == N)
            {
                return sequence[N - 1];
            }
            else
            {
                int k = (int)n;
                double d = n - k;
                return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
            }
        }
}
