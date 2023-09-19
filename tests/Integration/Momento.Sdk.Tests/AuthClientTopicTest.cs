#if NET6_0_OR_GREATER

using System.Threading.Tasks;
using System.Threading;
using Momento.Sdk.Auth;
using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Config;

namespace Momento.Sdk.Tests;

[Collection("AuthClient")]
public class AuthClientTopicTest : IClassFixture<CacheClientFixture>, IClassFixture<AuthClientFixture>, IClassFixture<TopicClientFixture>
{
    private readonly IAuthClient authClient;
    private readonly ITopicClient topicClient;
    private readonly string cacheName;
    private readonly string topicName = "topic";

    public AuthClientTopicTest(
        CacheClientFixture cacheFixture, AuthClientFixture authFixture, TopicClientFixture topicFixture
    )
    {
        authClient = authFixture.Client;
        cacheName = cacheFixture.CacheName;
        topicClient = topicFixture.Client;
    }

    private async Task<ITopicClient> GetClientForTokenScope(DisposableTokenScope scope)
    {
        var response = await authClient.GenerateDisposableTokenAsync(scope, ExpiresIn.Minutes(2));
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        string authToken = "";
        if (response is GenerateDisposableTokenResponse.Success token)
        {
            Assert.False(string.IsNullOrEmpty(token.AuthToken));
            authToken = token.AuthToken;
        }
        var authProvider = new StringMomentoTokenProvider(authToken);
        return new TopicClient(TopicConfigurations.Laptop.latest(), authProvider);
    }

    private async Task PublishToTopic(string cache, string topic, string value, ITopicClient? client = null)
    {
        client ??= topicClient;
        var response = await client.PublishAsync(cache, topic, value);
        Assert.True(response is TopicPublishResponse.Success);
    }

    private async Task ExpectTextFromSubscription(
        TopicSubscribeResponse.Subscription subscription, string expectedText
    )
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(5000);
        var cancellableSubscription = subscription.WithCancellation(cts.Token);
        var gotText = false;
        await foreach (var message in cancellableSubscription)
        {
            Assert.True(message is TopicMessage.Text);
            if (message is TopicMessage.Text textMsg) {
                Assert.Equal(expectedText, textMsg.Value);
                gotText = true;
            } 
            cts.Cancel();
            break;
        }
        if (!gotText) {
            Assert.True(false, "didn't recieve expected text: " +  expectedText);
        }
    }

    private void AssertPermissionError<TResp, TErrResp>(TResp response)
    {
        Assert.True(response is TErrResp);
        if (response is IError err)
        {
            Assert.Equal(err.ErrorCode, MomentoErrorCode.PERMISSION_ERROR);
        }
    }

    [Fact]
    public async Task GenerateDisposableTopicAuthToken_HappyPath()
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishSubscribe("cache", "topic"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishSubscribe(
                CacheSelector.ByName("cache"), "topic"),
                ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishSubscribe(
                "cache", TopicSelector.ByName("topic")),
                ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishSubscribe(
                CacheSelector.ByName("cache"), TopicSelector.ByName("topic")),
                ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);

        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishOnly("cache", "topic"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishOnly(
                CacheSelector.ByName("cache"), "topic"),
                ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishOnly(
                "cache", TopicSelector.ByName("topic")),
                ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishOnly(
                CacheSelector.ByName("cache"), TopicSelector.ByName("topic")),
                ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);

        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicSubscribeOnly("cache", "topic"), ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicSubscribeOnly(
                CacheSelector.ByName("cache"), "topic"),
                ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicSubscribeOnly(
                "cache", TopicSelector.ByName("topic")),
                ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicSubscribeOnly(
                CacheSelector.ByName("cache"), TopicSelector.ByName("topic")),
                ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Success);
    }

    [Theory]
    [InlineData(null, "topic")]
    [InlineData("cache", null)]
    public async Task GenerateDisposableTopicAuthToken_ErrorsOnNull(string cacheName, string topicName)
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishOnly(cacheName, topicName),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicSubscribeOnly(cacheName, topicName),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishSubscribe(cacheName, topicName),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData("", "topic")]
    [InlineData("cache", "")]
    public async Task GenerateDisposableTopicAuthToken_ErrorsOnEmpty(string cacheName, string topicName)
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishOnly(cacheName, topicName),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicSubscribeOnly(cacheName, topicName),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishSubscribe(cacheName, topicName),
            ExpiresIn.Minutes(10)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((GenerateDisposableTokenResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task GenerateDisposableTopicAuthToken_ErrorsOnBadExpiry()
    {
        var response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishSubscribe(cacheName, "foo"), ExpiresIn.Minutes(0)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        if (response is GenerateDisposableTokenResponse.Error err)
        {
            Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, err.ErrorCode);
        }
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishSubscribe(cacheName, "foo"), ExpiresIn.Minutes(-50)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        if (response is GenerateDisposableTokenResponse.Error err2)
        {
            Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, err2.ErrorCode);
        }
        response = await authClient.GenerateDisposableTokenAsync(
            DisposableTokenScopes.TopicPublishSubscribe(cacheName, "foo"), ExpiresIn.Days(365)
        );
        Assert.True(response is GenerateDisposableTokenResponse.Error);
        if (response is GenerateDisposableTokenResponse.Error err3)
        {
            Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, err3.ErrorCode);
        }
    }

    [Fact]
    public async Task GenerateDisposableTopicAuthToken_ReadWrite_HappyPath()
    {
        const string messageValue = "hello";
        var readwriteTopicClient = await GetClientForTokenScope(
            DisposableTokenScopes.TopicPublishSubscribe(cacheName, topicName)
        );
        var subscribeResponse = await readwriteTopicClient.SubscribeAsync(cacheName, topicName);
        Assert.True(subscribeResponse is TopicSubscribeResponse.Subscription);
        var publishResponse = await readwriteTopicClient.PublishAsync(cacheName, topicName, messageValue);
        Assert.True(publishResponse is TopicPublishResponse.Success);
        if (subscribeResponse is TopicSubscribeResponse.Subscription subscription)
        {
            await PublishToTopic(cacheName, topicName, messageValue, readwriteTopicClient);
            await ExpectTextFromSubscription(subscription, messageValue);
        }
    }

    [Fact]
    public async Task GenerateDisposableTopicAuthToken_ReadOnly_HappyPath()
    {
        const string messageValue = "hello";
        var readonlyTopicClient = await GetClientForTokenScope(
            DisposableTokenScopes.TopicSubscribeOnly(cacheName, topicName)
        );
        var subscribeResponse = await readonlyTopicClient.SubscribeAsync(cacheName, topicName);
        Assert.True(subscribeResponse is TopicSubscribeResponse.Subscription);
        var publishResponse = await readonlyTopicClient.PublishAsync(cacheName, topicName, messageValue);
        AssertPermissionError<TopicPublishResponse, TopicPublishResponse.Error>(publishResponse);
        await PublishToTopic(cacheName, topicName, messageValue);
        if (subscribeResponse is TopicSubscribeResponse.Subscription subscription)
        {
            await ExpectTextFromSubscription(subscription, messageValue);
        }
    }

    [Fact(Skip = "Enable when SubscribeAsync returns an error")]
    public async Task GenerateDisposableTopicAuthToken_WriteOnly_CantSubscribe()
    {
        var writeOnlyTopicClient = await GetClientForTokenScope(
            DisposableTokenScopes.TopicPublishOnly(cacheName, topicName)
        );
        var subscribeResponse = await writeOnlyTopicClient.SubscribeAsync(cacheName, topicName);
        Assert.True(subscribeResponse is TopicSubscribeResponse.Error, $"expected error got {subscribeResponse}");
    }

    [Fact]
    public async Task GenerateDisposableTopicAuthToken_WriteOnly_CanPublish()
    {
        const string messageValue = "hello";
        var writeOnlyTopicClient = await GetClientForTokenScope(
            DisposableTokenScopes.TopicPublishOnly(cacheName, topicName)
        );

        var subscribeResponse = await topicClient.SubscribeAsync(cacheName, topicName);
        Assert.True(subscribeResponse is TopicSubscribeResponse.Subscription);

        // `PublishToTopic` asserts a successful publish response
        await PublishToTopic(cacheName, topicName, messageValue, writeOnlyTopicClient);

        if (subscribeResponse is TopicSubscribeResponse.Subscription subscription)
        {
            var subTask = ExpectTextFromSubscription(subscription, messageValue);
            await PublishToTopic(cacheName, topicName, messageValue, writeOnlyTopicClient);
            await subTask;
        }
    }

    [Fact]
    public async Task GenerateDisposableTopicAuthToken_NoCachePerms_CantPublish()
    {
        var noCachePermsClient = await GetClientForTokenScope(
            DisposableTokenScopes.TopicPublishSubscribe("notthecacheyourelookingfor", topicName)
        );
        var response = await noCachePermsClient.PublishAsync(cacheName, topicName, "iamdoomed");
        AssertPermissionError<TopicPublishResponse, TopicPublishResponse.Error>(response);
    }

    [Fact(Skip = "Enable when SubscribeAsync returns an error")]
    public async Task GenerateDisposableTopicAuthToken_NoCachePerms_CantSubscribe()
    {
        Assert.True(false);
        var noCachePermsClient = await GetClientForTokenScope(
            DisposableTokenScopes.TopicPublishSubscribe("notthecacheyourelookingfor", topicName)
        );
        var response = await noCachePermsClient.SubscribeAsync(cacheName, topicName);
        AssertPermissionError<TopicSubscribeResponse, TopicSubscribeResponse.Error>(response);
    }

    [Fact]
    public async Task GenerateDisposableTopicAuthToken_NoTopicPerms_CantPublish()
    {
        var noCachePermsClient = await GetClientForTokenScope(
            DisposableTokenScopes.TopicPublishSubscribe(cacheName, "notthetopicyourelookingfor")
        );
        var response = await noCachePermsClient.PublishAsync(cacheName, topicName, "iamdoomed");
        AssertPermissionError<TopicPublishResponse, TopicPublishResponse.Error>(response);
    }

    [Fact(Skip = "Enable when SubscribeAsync returns an error")]
    public async Task GenerateDisposableTopicAuthToken_NoTopicPerms_CantSubscribe()
    {
        Assert.True(false);
        var noCachePermsClient = await GetClientForTokenScope(
            DisposableTokenScopes.TopicPublishSubscribe(cacheName, "notthetopicyourelookingfor")
        );
        var response = await noCachePermsClient.SubscribeAsync(cacheName, topicName);
        AssertPermissionError<TopicSubscribeResponse, TopicSubscribeResponse.Error>(response);
    }
}
#endif
