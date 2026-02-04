using System;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;


static class Program
{

    private static readonly ICredentialProvider authProvider;
    private static readonly IAuthClient client;

    static Program()
    {
        authProvider = new EnvMomentoTokenProvider("V1_API_KEY");
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
        var token = await GenerateTokenWithEnumeratedPermissions();
        PrintTokenInfo(token);
        token = await GenerateTokenWithPredefinedScope();
        PrintTokenInfo(token);
    }
}
