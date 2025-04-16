#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Momento.Sdk.Tests.Integration.Topics;

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
        var topicName = Utils.NewGuidString();
        var valuesToSend = new List<byte[]>
        {
            new byte[] { 0x00 },
            new byte[] { 0x01 },
            new byte[] { 0x02 },
            new byte[] { 0x03 },
            new byte[] { 0x04 },
            new byte[] { 0x05 }
        };

        var produceCancellation = new CancellationTokenSource();
        produceCancellation.CancelAfter(5000);

        // we don't need to put this on a different thread
        var consumeTask = ConsumeMessages(topicName, produceCancellation.Token);
        await Task.Delay(1000);

        await ProduceMessages(topicName, valuesToSend);
        await Task.Delay(1000);

        produceCancellation.Cancel();

        var consumedMessages = await consumeTask;
        Assert.Equal(valuesToSend.Count, consumedMessages.Count);
        for (var i = 0; i < valuesToSend.Count; ++i)
        {
            Assert.Equal(((TopicMessage.Binary)consumedMessages[i]).Value, valuesToSend[i]);
        }
    }

    [Fact(Timeout = 5000)]
    public async Task PublishAndSubscribe_String_Succeeds()
    {
        var topicName = Utils.NewGuidString();
        var valuesToSend = new List<string>
        {
            "one",
            "two",
            "three",
            "four",
            "five"
        };

        var produceCancellation = new CancellationTokenSource();
        produceCancellation.CancelAfter(2000);

        // we don't need to put this on a different thread
        var consumeTask = ConsumeMessages(topicName, produceCancellation.Token);
        await Task.Delay(500);

        await ProduceMessages(topicName, valuesToSend);
        await Task.Delay(500);

        produceCancellation.Cancel();

        var consumedMessages = await consumeTask;
        Assert.Equal(valuesToSend.Count, consumedMessages.Count);
        for (var i = 0; i < valuesToSend.Count; ++i)
        {
            var textMessage = (TopicMessage.Text)consumedMessages[i];
            Assert.Equal(textMessage.Value, valuesToSend[i]);
            Assert.Equal(textMessage.TopicSequenceNumber, checked((ulong)(i + 1)));
        }
    }

    [Fact(Timeout = 15000)]
    public async Task PublishAndSubscribe_AllEventsString_Succeeds()
    {
        var topicName = Utils.NewGuidString();
        var valuesToSend = new List<string>
        {
            "one",
            "two",
            "three",
            "four",
            "five"
        };

        var produceCancellation = new CancellationTokenSource();

        // we don't need to put this on a different thread
        var consumeTask = ConsumeAllEvents(topicName, produceCancellation.Token);
        await Task.Delay(500);

        await ProduceMessages(topicName, valuesToSend);
        await Task.Delay(10000);

        produceCancellation.Cancel();

        var consumedEvents = await consumeTask;
        var messageCount = 0;
        var heartbeatCount = 0;
        var discontinuityCount = 0;
        foreach (var topicEvent in consumedEvents)
        {
            switch (topicEvent)
            {
                case TopicMessage.Text textMessage:
                    Assert.Equal(textMessage.Value, valuesToSend[messageCount]);
                    Assert.Equal(textMessage.TopicSequenceNumber, checked((ulong)(messageCount + 1)));
                    messageCount++;
                    break;
                case TopicSystemEvent.Heartbeat:
                    heartbeatCount++;
                    break;
                case TopicSystemEvent.Discontinuity:
                    discontinuityCount++;
                    break;
                default:
                    throw new Exception("bad message received");
            }
        }

        Assert.Equal(valuesToSend.Count, messageCount);
        Assert.True(heartbeatCount > 0);
        Assert.Equal(0, discontinuityCount);
    }


    private async Task ProduceMessages(string topicName, List<byte[]> valuesToSend)
    {
        foreach (var value in valuesToSend)
        {
            var publishResponse = await topicClient.PublishAsync(cacheName, topicName, value);
            switch (publishResponse)
            {
                case TopicPublishResponse.Success:
                    await Task.Delay(100);
                    break;
                default:
                    throw new Exception("publish error");
            }
        }
    }

    private async Task ProduceMessages(string topicName, List<string> valuesToSend)
    {
        foreach (var value in valuesToSend)
        {
            var publishResponse = await topicClient.PublishAsync(cacheName, topicName, value);
            switch (publishResponse)
            {
                case TopicPublishResponse.Success:
                    await Task.Delay(100);
                    break;
                default:
                    throw new Exception("publish error");
            }
        }
    }

    private async Task<List<TopicMessage>> ConsumeMessages(string topicName, CancellationToken token)
    {
        var subscribeResponse = await topicClient.SubscribeAsync(cacheName, topicName);
        switch (subscribeResponse)
        {
            case TopicSubscribeResponse.Subscription subscription:
                var cancellableSubscription = subscription.WithCancellation(token);
                var receivedSet = new List<TopicMessage>();
                await foreach (var message in cancellableSubscription)
                {
                    switch (message)
                    {
                        case TopicMessage.Binary:
                        case TopicMessage.Text:
                            Console.WriteLine($"Received message {receivedSet.Count}: {message.GetType().Name}");
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

    private async Task<List<ITopicEvent>> ConsumeAllEvents(string topicName, CancellationToken token)
    {
        var subscribeResponse = await topicClient.SubscribeAsync(cacheName, topicName);
        switch (subscribeResponse)
        {
            case TopicSubscribeResponse.Subscription subscription:
                var cancellableSubscription = subscription.WithCancellationForAllEvents(token);
                var receivedSet = new List<ITopicEvent>();
                await foreach (var topicEvent in cancellableSubscription)
                {
                    switch (topicEvent)
                    {
                        case TopicMessage.Binary:
                        case TopicMessage.Text:
                        case TopicSystemEvent:
                            receivedSet.Add(topicEvent);
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

    [Fact]
    public async Task MultipleSubscriptions_HappyPath()
    {
        var numTopics = 20;

        var subscriptionResponses = await Task.WhenAll(
            Enumerable.Range(1, numTopics).Select(i =>
                topicClient.SubscribeAsync(cacheName, $"topic{i}")
                    .ContinueWith(r => Tuple.Create(i, r.Result))).ToList());

        var subscriptions = subscriptionResponses.Select(t =>
        {
            var (topicNum, subscriptionResponse) = t;
            if (subscriptionResponse is TopicSubscribeResponse.Subscription subscription)
            {
                return Tuple.Create(topicNum, subscription);
            }

            throw new Exception($"Got an unexpected subscription response: {subscriptionResponse}");
        }).ToList();

        var subscribers = subscriptions.Select(t => Task.Run(async () =>
        {
            var (topicNum, subscription) = t;

            int messageCount = 0;
            await foreach (var message in subscription)
            {
                switch (message)
                {
                    case TopicMessage.Text:
                        messageCount++;
                        break;
                }
            }

            return messageCount;
        })).ToList();

        await Task.Delay(100);

        const int numMessagesToPublish = 50;
        foreach (var i in Enumerable.Range(0, numMessagesToPublish))
        {
            var randomTopic = Random.Shared.NextInt64(numTopics) + 1;
            var messageId = $"message{i}";
            var topic = $"topic{randomTopic}";
            var publishResponse = await topicClient.PublishAsync(cacheName, topic, messageId);
            if (publishResponse is not TopicPublishResponse.Success)
            {
                throw new Exception($"Publish did not succeed: {publishResponse}");
            }

            await Task.Delay(100);
        }

        await Task.Delay(1_000);

        foreach (var subscriptionTuple in subscriptions)
        {
            var (_, subscription) = subscriptionTuple;
            subscription.Dispose();
        }

        var subscriberResults = await Task.WhenAll(subscribers);
        var numMessagesReceived = subscriberResults.Sum();
        Assert.Equal(numMessagesToPublish, numMessagesReceived);
    }
}
#endif
