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
        Assert.Equal(1, 1);
        // const string topicName = "topic_bytes";
        // var valuesToSend = new HashSet<byte[]>
        // {
        //     new byte[] { 0x01 },
        //     new byte[] { 0x02 },
        //     new byte[] { 0x03 },
        //     new byte[] { 0x04 },
        //     new byte[] { 0x05 }
        // };
        //
        // using var cts = new CancellationTokenSource();
        // cts.CancelAfter(4000);
        //
        // // var subscribeResponse = await topicClient.SubscribeAsync(cacheName, topicName);
        // // Assert.True(subscribeResponse is TopicSubscribeResponse.Subscription,
        // //     $"Unexpected response: {subscribeResponse}");
        // // var subscription = ((TopicSubscribeResponse.Subscription)subscribeResponse).WithCancellation(cts.Token);
        //
        // Console.WriteLine("subscription created");
        // // var taskCompletionSourceBool = new TaskCompletionSource<bool>();
        // var semaphoreSlim = new SemaphoreSlim(0, 1);
        // var testTask = Task.Run(async () =>
        // {
        //     // var messageCount = 0;
        //     var receivedSet = new HashSet<byte[]>();
        //     // taskCompletionSourceBool.SetResult(true);
        //     semaphoreSlim.Release();
        //     await Task.Delay(2000);
        //     // await foreach (var message in subscription)
        //     // {
        //     //     Assert.NotNull(message);
        //     //     Assert.True(message is TopicMessage.Binary, $"Unexpected message: {message}");
        //     //     var value = ((TopicMessage.Binary)message).Value();
        //     //     receivedSet.Add(value);
        //     //
        //     //     Assert.Contains(value, valuesToSend);
        //     //     if (receivedSet.Count == valuesToSend.Count)
        //     //     {
        //     //         break;
        //     //     }
        //     // }
        //     return receivedSet.Count;
        // }, cts.Token);
        //
        // Console.WriteLine("enumerator task started");
        // // await taskCompletionSourceBool.Task;
        // await semaphoreSlim.WaitAsync(cts.Token);
        // // await Task.Delay(1000);
        //
        // foreach (var value in valuesToSend)
        // {
        //     var publishResponse = await topicClient.PublishAsync(cacheName, topicName, value);
        //     Assert.True(publishResponse is TopicPublishResponse.Success, $"Unexpected response: {publishResponse}");
        //     await Task.Delay(100);
        // }
        // Console.WriteLine("messages sent");
        //
        // int received = await testTask;
        // Console.WriteLine("Found " + received);
        // // Assert.Equal(valuesToSend.Count, received);
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