using System.Threading.Tasks;
using Momento.Sdk.Auth.AccessControl;

namespace Momento.Sdk.Tests.Integration;

[Collection("AuthClient")]
public class AuthClientTest : IClassFixture<AuthClientFixture>
{
    private readonly IAuthClient authClient;

    public AuthClientTest(AuthClientFixture authFixture)
    {
        authClient = authFixture.Client;
    }

    [Fact]
    public async Task GenerateDisposableAuthToken_HappyPath()
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishSubscribe("cache", "topic"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
    }

}
