using System;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;

ICredentialProvider authProvider = new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN");

IAuthClient client = new AuthClient(AuthConfigurations.Default.Latest(), authProvider);
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
        TopicSelector.ByName("example-topic")
    )
});
var tokenResponse = await client.GenerateDisposableTokenAsync(
    scope,
    ExpiresIn.Minutes(5)
);

if (tokenResponse is GenerateDisposableTokenResponse.Success token)
{
    Console.WriteLine("The generated disposable token is: " + token.AuthToken);
    Console.WriteLine("The token endpoint is: " + token.Endpoint);
    Console.WriteLine("The token expires at (epoch timestamp): " + token.ExpiresAt.Epoch());
}
else if (tokenResponse is GenerateDisposableTokenResponse.Error err)
{
    Console.WriteLine("Error generating disposable token: " + err.Message);
}
