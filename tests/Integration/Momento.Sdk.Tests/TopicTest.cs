#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Momento.Sdk.Tests;

public class TopicTest : IClassFixture<CacheClientFixture>, IClassFixture<TopicClientFixture>
{
    private readonly string cacheName;
    private readonly ITopicClient topicClient;

    public TopicTest(CacheClientFixture cacheFixture, TopicClientFixture topicFixture)
    {
        topicClient = topicFixture.Client;
        cacheName = cacheFixture.CacheName;
    }

    [Theory]
    [InlineData(null, "topic")]
    [InlineData("cache", null)]
    public async Task PublishAsync_NullChecksByteArray_IsError(string badCacheName, string badTopicName)
    {
        var response = await topicClient.PublishAsync(badCacheName, badTopicName, Array.Empty<byte>());
        Assert.True(response is TopicPublishResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((TopicPublishResponse.Error)response).ErrorCode);
    }
    
    [Fact]
    public async Task PublishAsync_PublishNullByteArray_IsError()
    {
        var response = await topicClient.PublishAsync(cacheName, "topic", (byte[])null!);
        Assert.True(response is TopicPublishResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((TopicPublishResponse.Error)response).ErrorCode);
    }
    
    [Fact]
    public async Task PublishAsync_BadCacheNameByteArray_IsError()
    {
        var response = await topicClient.PublishAsync("fake-" + cacheName, "topic", Array.Empty<byte>());
        Assert.True(response is TopicPublishResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, ((TopicPublishResponse.Error)response).ErrorCode);
    }
    
    [Theory]
    [InlineData(null, "topic")]
    [InlineData("cache", null)]
    public async Task PublishAsync_NullChecksString_IsError(string badCacheName, string badTopicName)
    {
        var response = await topicClient.PublishAsync(badCacheName, badTopicName, "value");
        Assert.True(response is TopicPublishResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((TopicPublishResponse.Error)response).ErrorCode);
    }
    
    [Fact]
    public async Task PublishAsync_PublishNullString_IsError()
    {
        var response = await topicClient.PublishAsync(cacheName, "topic", (string)null!);
        Assert.True(response is TopicPublishResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((TopicPublishResponse.Error)response).ErrorCode);
    }
    
    [Fact]
    public async Task PublishAsync_BadCacheNameString_IsError()
    {
        var response = await topicClient.PublishAsync("fake-" + cacheName, "topic", "value");
        Assert.True(response is TopicPublishResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, ((TopicPublishResponse.Error)response).ErrorCode);
    }
    
    [Theory]
    [InlineData(null, "topic")]
    [InlineData("cache", null)]
    public async Task SubscribeAsync_NullChecks_IsError(string badCacheName, string badTopicName)
    {
        var response = await topicClient.SubscribeAsync(badCacheName, badTopicName);
        Assert.True(response is TopicSubscribeResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((TopicSubscribeResponse.Error)response).ErrorCode);
    }

    [Fact(Timeout = 5000)]
    public async Task PublishAndSubscribe_ByteArray_Succeeds()
    {
        const string topicName = "topic_bytes";
        var valuesToSend = new HashSet<byte[]>
        {
            new byte[] { 0x01 },
            new byte[] { 0x02 },
            new byte[] { 0x03 },
            new byte[] { 0x04 },
            new byte[] { 0x05 }
        };
        
        var produceCtx = new CancellationTokenSource();

        var consumeTask = Task.Run(async () => await ConsumeMessages(topicName, produceCtx.Token));

        await Task.Delay(500);
        
        foreach (var value in valuesToSend)
        {
            var publishResponse = await topicClient.PublishAsync(cacheName, topicName, value);
            switch (publishResponse)
            {
                case TopicPublishResponse.Success success:
                    await Task.Delay(100);
                    break;
                default:
                    Assert.Fail("Unable to send message");
                    break;
            }
        }

        await Task.Delay(500);
        produceCtx.Cancel();
        
        var consumedMessages = await consumeTask;
        Assert.Equal(valuesToSend.Count, consumedMessages.Count);
        foreach (var message in consumedMessages)
        {
            Assert.Contains(((TopicMessage.Binary)message).Value, valuesToSend);
        }
    }

    [Fact(Timeout = 5000)]
    public async Task PublishAndSubscribe_String_Succeeds()
    {
        const string topicName = "topic_string";
        var valuesToSend = new List<string>
        {
            "one",
            "two",
            "three",
            "four",
            "five"
        };
        
        var produceCtx = new CancellationTokenSource();

        var consumeTask = Task.Run(async () => await ConsumeMessages(topicName, produceCtx.Token));

        await Task.Delay(500);
        
        foreach (var value in valuesToSend)
        {
            var publishResponse = await topicClient.PublishAsync(cacheName, topicName, value);
            switch (publishResponse)
            {
                case TopicPublishResponse.Success success:
                    await Task.Delay(100);
                    break;
                default:
                    Assert.Fail("Unable to send message");
                    break;
            }
        }

        await Task.Delay(500);
        produceCtx.Cancel();
        
        var consumedMessages = await consumeTask;
        Assert.Equal(valuesToSend.Count, consumedMessages.Count);
        foreach (var message in consumedMessages)
        {
            Assert.Contains(((TopicMessage.Text)message).Value, valuesToSend);
        }
    }
    
    private async Task<List<TopicMessage>> ConsumeMessages(string topicName, CancellationToken token)
    {
        var subscribeResponse = await topicClient.SubscribeAsync(cacheName, topicName);
        switch (subscribeResponse)
        {
            case TopicSubscribeResponse.Subscription subscription:
                var cancellableSub = subscription.WithCancellation(token);
                var receivedSet = new List<TopicMessage>();
                await foreach (var message in cancellableSub)
                {
                    switch (message)
                    {
                        case TopicMessage.Binary:
                        case TopicMessage.Text:
                            receivedSet.Add(message);
                            break;
                        default:
                            throw new Exception("bad message received");
                    }
                }
                subscription.Dispose();
                return receivedSet;
            default:
                throw new Exception("subscription error");
        }
    }
}
#endif