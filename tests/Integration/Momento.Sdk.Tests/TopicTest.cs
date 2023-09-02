#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

// ReSharper disable once CheckNamespace
namespace Momento.Sdk.Tests;

public class TopicTest : IClassFixture<CacheClientFixture>, IClassFixture<TopicClientFixture>
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly string cacheName;
    private readonly ITopicClient topicClient;

    public TopicTest(CacheClientFixture cacheFixture, TopicClientFixture topicFixture,
        ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
        topicClient = topicFixture.Client;
        cacheName = cacheFixture.CacheName;
    }

    // [Theory]
    // [InlineData(null, "topic")]
    // [InlineData("cache", null)]
    // public async Task PublishAsync_NullChecksByteArray_IsError(string badCacheName, string badTopicName)
    // {
    //     var response = await topicClient.PublishAsync(badCacheName, badTopicName, Array.Empty<byte>());
    //     Assert.True(response is TopicPublishResponse.Error, $"Unexpected response: {response}");
    //     Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((TopicPublishResponse.Error)response).ErrorCode);
    // }
    //
    // [Fact]
    // public async Task PublishAsync_PublishNullByteArray_IsError()
    // {
    //     var response = await topicClient.PublishAsync(cacheName, "topic", (byte[])null!);
    //     Assert.True(response is TopicPublishResponse.Error, $"Unexpected response: {response}");
    //     Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((TopicPublishResponse.Error)response).ErrorCode);
    // }
    //
    // [Fact]
    // public async Task PublishAsync_BadCacheNameByteArray_IsError()
    // {
    //     var response = await topicClient.PublishAsync("fake-" + cacheName, "topic", Array.Empty<byte>());
    //     Assert.True(response is TopicPublishResponse.Error, $"Unexpected response: {response}");
    //     Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, ((TopicPublishResponse.Error)response).ErrorCode);
    // }
    //
    // [Theory]
    // [InlineData(null, "topic")]
    // [InlineData("cache", null)]
    // public async Task PublishAsync_NullChecksString_IsError(string badCacheName, string badTopicName)
    // {
    //     var response = await topicClient.PublishAsync(badCacheName, badTopicName, "value");
    //     Assert.True(response is TopicPublishResponse.Error, $"Unexpected response: {response}");
    //     Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((TopicPublishResponse.Error)response).ErrorCode);
    // }
    //
    // [Fact]
    // public async Task PublishAsync_PublishNullString_IsError()
    // {
    //     var response = await topicClient.PublishAsync(cacheName, "topic", (string)null!);
    //     Assert.True(response is TopicPublishResponse.Error, $"Unexpected response: {response}");
    //     Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((TopicPublishResponse.Error)response).ErrorCode);
    // }
    //
    // [Fact]
    // public async Task PublishAsync_BadCacheNameString_IsError()
    // {
    //     var response = await topicClient.PublishAsync("fake-" + cacheName, "topic", "value");
    //     Assert.True(response is TopicPublishResponse.Error, $"Unexpected response: {response}");
    //     Assert.Equal(MomentoErrorCode.NOT_FOUND_ERROR, ((TopicPublishResponse.Error)response).ErrorCode);
    // }
    //
    // [Theory]
    // [InlineData(null, "topic")]
    // [InlineData("cache", null)]
    // public async Task SubscribeAsync_NullChecks_IsError(string badCacheName, string badTopicName)
    // {
    //     var response = await topicClient.SubscribeAsync(badCacheName, badTopicName);
    //     Assert.True(response is TopicSubscribeResponse.Error, $"Unexpected response: {response}");
    //     Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((TopicSubscribeResponse.Error)response).ErrorCode);
    // }

    [Fact(Timeout = 5000)]
    public async Task PublishAndSubscribe_ByteArray_Succeeds()
    {
        Console.WriteLine("starting binary publish and subscribe test");
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

        var consumeTask = Task.Run(async () => await ConsumeItems(produceCtx.Token, topicName));

        Console.Error.WriteLine("consume task created");
        await Task.Delay(500);
        Console.Error.WriteLine("first delay finished");
        await ProduceItems(topicName, valuesToSend);

        Console.Error.WriteLine("messages produced");
        await Task.Delay(500);
        Console.Error.WriteLine("second delay finished");
        produceCtx.Cancel();
        Console.Error.WriteLine("consume task cancelled");
        var producedItems = await consumeTask;
        Assert.Equal(valuesToSend.Count, producedItems.Count);
        foreach (var value in valuesToSend)
        {
            Assert.Contains(value, valuesToSend);
        }
    }

    private async Task<HashSet<byte[]>> ConsumeItems(CancellationToken token, string topicName)
    {
        var subscribeResponse = await topicClient.SubscribeAsync(cacheName, topicName);
        switch (subscribeResponse)
        {
            case TopicSubscribeResponse.Subscription subscription:
                var cancellableSub = subscription.WithCancellation(token);
                var receivedSet = new HashSet<byte[]>();
                await foreach (var message in cancellableSub)
                {
                    switch (message)
                    {
                        case TopicMessage.Binary binary:
                            receivedSet.Add(binary.Value());
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

    private async Task ProduceItems(string topicName, HashSet<byte[]> itemsToSend)
    {
        foreach (var value in itemsToSend)
        {
            var publishResponse = await topicClient.PublishAsync(cacheName, topicName, value);
            switch (publishResponse)
            {
                case TopicPublishResponse.Success success:
                    await Task.Delay(100);
                    break;
                default:
                    throw new Exception("publish error");
            }
        }
    }

    // [Fact(Timeout = 5000)]
    // public async Task PublishAndSubscribe_String_Succeeds()
    // {
    //     testOutputHelper.WriteLine("starting string publish and subscribe test");
    //     const string topicName = "topic_string";
    //     var valuesToSend = new List<string>
    //     {
    //         "one",
    //         "two",
    //         "three",
    //         "four",
    //         "five"
    //     };
    //
    //     using var cts = new CancellationTokenSource();
    //     cts.CancelAfter(4000);
    //
    //     var subscribeResponse = await topicClient.SubscribeAsync(cacheName, topicName);
    //     Assert.True(subscribeResponse is TopicSubscribeResponse.Subscription,
    //         $"Unexpected response: {subscribeResponse}");
    //     var subscription = ((TopicSubscribeResponse.Subscription)subscribeResponse).WithCancellation(cts.Token);
    //
    //     var testTask = Task.Run(async () =>
    //     {
    //         var messageCount = 0;
    //         await foreach (var message in subscription)
    //         {
    //             Assert.NotNull(message);
    //             Assert.True(message is TopicMessage.Text, $"Unexpected message: {message}");
    //
    //             Assert.Equal(valuesToSend[messageCount], ((TopicMessage.Text)message).Value);
    //
    //             messageCount++;
    //             if (messageCount == valuesToSend.Count)
    //             {
    //                 break;
    //             }
    //         }
    //
    //         return messageCount;
    //     }, cts.Token);
    //
    //     await Task.Delay(1000);
    //
    //     foreach (var value in valuesToSend)
    //     {
    //         var publishResponse = await topicClient.PublishAsync(cacheName, topicName, value);
    //         Assert.True(publishResponse is TopicPublishResponse.Success, $"Unexpected response: {publishResponse}");
    //         await Task.Delay(100);
    //     }
    //
    //     Assert.Equal(valuesToSend.Count, await testTask);
    // }

    // [Fact(Timeout = 5000)]
    // public async Task Subscribe_EnumerateClosed_Succeeds()
    // {
    //     testOutputHelper.WriteLine("starting enumerate after closed test");
    //     const string topicName = "topic_closed";
    //     const string messageValue = "value";
    //
    //     using var cts = new CancellationTokenSource();
    //
    //     var subscribeResponse = await topicClient.SubscribeAsync(cacheName, topicName);
    //     Assert.True(subscribeResponse is TopicSubscribeResponse.Subscription,
    //         $"Unexpected response: {subscribeResponse}");
    //     var subscription = ((TopicSubscribeResponse.Subscription)subscribeResponse).WithCancellation(cts.Token);
    //     testOutputHelper.WriteLine("subscribed");
    //
    //     var enumerator = subscription.GetAsyncEnumerator();
    //     Assert.Null(enumerator.Current);
    //     testOutputHelper.WriteLine("enumerator gotten");
    //
    //     var publishResponse = await topicClient.PublishAsync(cacheName, topicName, messageValue);
    //     Assert.True(publishResponse is TopicPublishResponse.Success, $"Unexpected response: {publishResponse}");
    //     testOutputHelper.WriteLine("message published");
    //
    //     cts.Cancel();
    //
    //     Assert.False(await enumerator.MoveNextAsync());
    //
    //     Assert.Null(enumerator.Current);
    // }
}
#endif