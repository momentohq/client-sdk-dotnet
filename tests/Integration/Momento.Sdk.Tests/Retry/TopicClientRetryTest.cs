using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace Momento.Sdk.Tests.Integration.Retry;

public class TestAdminClient
{
    private readonly string _endpoint;
    private readonly HttpClient _httpClient;

    public TestAdminClient()
    {
        var hostname = Environment.GetEnvironmentVariable("TEST_ADMIN_HOSTNAME") ?? "127.0.0.1";
        var port = Environment.GetEnvironmentVariable("TEST_ADMIN_PORT") ?? "9090";
        _endpoint = $"http://{hostname}:{port}";
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };
    }

    public async Task BlockPort()
    {
        try
        {
            Console.WriteLine($"Attempting to block port at {_endpoint}/block");
            var response = await _httpClient.GetAsync($"{_endpoint}/block");
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Port blocked successfully");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Failed to block port: {ex.Message}");
            throw;
        }
    }

    public async Task UnblockPort()
    {
        try
        {
            Console.WriteLine($"Attempting to unblock port at {_endpoint}/unblock");
            var response = await _httpClient.GetAsync($"{_endpoint}/unblock");
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Port unblocked successfully");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Failed to unblock port: {ex.Message}");
            throw;
        }
    }
}

public class HeartbeatTimestampCollector
{
    private readonly List<DateTime> _timestamps;
    private readonly TimeSpan _timeout;

    public HeartbeatTimestampCollector(TimeSpan timeout)
    {
        _timestamps = new();
        _timeout = timeout;
    }

    public void AddTimestamp(DateTime timestamp)
    {
        _timestamps.Add(timestamp);
    }

    public int GetCountOfTimestamps()
    {
        return _timestamps.Count;
    }

    public int GetCountOfTimeouts()
    {
        // Count number of times timestamps were more than timeout seconds apart
        var count = 0;
        for (var i = 0; i < _timestamps.Count - 1; i++)
        {
            if (_timestamps[i + 1] - _timestamps[i] > _timeout)
            {
                count++;
            }
        }
        return count;
    }
}

[Collection("Retry")]
public class TopicClientRetryTests
{
    private readonly ICredentialProvider _authProvider;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IConfiguration _cacheConfig;
    private readonly ITopicConfiguration _topicConfig;

    public TopicClientRetryTests()
    {
        _authProvider = new MomentoLocalProvider();
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "hh:mm:ss ";
            });
            builder.AddFilter("Grpc.Net.Client", LogLevel.Error);
            builder.SetMinimumLevel(LogLevel.Information);
        });
        _cacheConfig = Configurations.Laptop.Latest(_loggerFactory);
        _topicConfig = TopicConfigurations.Laptop.latest(_loggerFactory);
    }

    [Fact]
    public async Task SubscriptionResumesAfterKeepalivesStopThenResume()
    {
        var testProps = new MomentoLocalCacheAndTopicClient(_authProvider, _loggerFactory, _cacheConfig, _topicConfig, null);
        var testAdmin = new TestAdminClient();
        var heartbeatTimeout = TimeSpan.FromSeconds(3);
        var heartbeatCounter = new HeartbeatTimestampCollector(heartbeatTimeout);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(10_000);

        var subscribeResponse = await testProps.TopicClient.SubscribeAsync(testProps.CacheName, "topic");
        var subscriptionTask = Task.Run(async () =>
        {
            switch (subscribeResponse) 
            {
                case TopicSubscribeResponse.Subscription subscription:
                    try
                    {
                        var cancellableSubscription = subscription.WithCancellationForAllEvents(cts.Token);
                        await foreach (var topicEvent in cancellableSubscription)
                        {
                            switch (topicEvent)
                            {
                                case TopicSystemEvent.Heartbeat:
                                    heartbeatCounter.AddTimestamp(DateTime.Now);
                                    break;
                                case TopicMessage.Error error:
                                    Assert.Fail("Received error message from topic: {error.ToString()}");
                                    break;
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected when the test times out
                    }
                    catch (IOException)
                    {
                        // Expected when connection is reset
                    }
                    finally
                    {
                        subscription.Dispose();
                    }
                    break;
                default:
                    Assert.Fail("Expected subscription response, got error: {subscribeResponse.ToString()}");
                    cts.Cancel();
                    break;
            }
        });
        
        // After 3 seconds, stop the keepalives, wait 3 seconds, then resume them
        await Task.Delay(3000);
        await testAdmin.BlockPort();
        await Task.Delay(heartbeatTimeout);
        await testAdmin.UnblockPort();

        // Wait for the subscription task to complete
        await subscriptionTask;

        Assert.InRange(heartbeatCounter.GetCountOfTimestamps(), 5, 10);
        Assert.Equal(1, heartbeatCounter.GetCountOfTimeouts());
    }

    [Fact]
    public async Task SubscriptionReconnectsAfterRecoverableError() 
    {
        var momentoLocalArgs = new MomentoLocalMiddlewareArgs {
            StreamError = MomentoErrorCode.SERVER_UNAVAILABLE.ToStringValue(),
            StreamErrorRpcList = new List<string> { MomentoRpcMethod.TopicSubscribe.ToMomentoLocalMetadataString() },
            StreamErrorMessageLimit = 3 // Receive an error after every 2 heartbeats
        };
        var testProps = new MomentoLocalCacheAndTopicClient(_authProvider, _loggerFactory, _cacheConfig, _topicConfig, momentoLocalArgs);

        var heartbeatTimeout = TimeSpan.FromSeconds(3);
        var heartbeatCounter = new HeartbeatTimestampCollector(heartbeatTimeout);
        
        var cts = new CancellationTokenSource();
        cts.CancelAfter(10_000);

        var subscribeResponse = await testProps.TopicClient.SubscribeAsync(testProps.CacheName, "topic");
        switch (subscribeResponse) 
        {
            case TopicSubscribeResponse.Subscription subscription:
                try
                {
                    var cancellableSubscription = subscription.WithCancellationForAllEvents(cts.Token);
                    await foreach (var topicEvent in cancellableSubscription)
                    {
                        switch (topicEvent)
                        {
                            case TopicSystemEvent.Heartbeat:
                                heartbeatCounter.AddTimestamp(DateTime.Now);
                                break;
                            case TopicMessage.Error error:
                                Assert.Fail("Received error message from topic: {error.ToString()}");
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected when the test times out
                }
                finally
                {
                    subscription.Dispose();
                }
                break;
            default:
                Assert.Fail("Expected subscription response, got error: {subscribeResponse.ToString()}");
                cts.Cancel();
                break;
        }
        
        Assert.Equal(1, heartbeatCounter.GetCountOfTimeouts()); // 1 timeout for the resubscribe period
        Assert.InRange(heartbeatCounter.GetCountOfTimestamps(), 2, 8); // counts heartbeats before and after the resubscribe period
    }

    [Fact]
    public async Task SubscriptionDoesNotReconnectAfterUnrecoverableError() 
    {
        var momentoLocalArgs = new MomentoLocalMiddlewareArgs {
            StreamError = MomentoErrorCode.AUTHENTICATION_ERROR.ToStringValue(),
            StreamErrorRpcList = new List<string> { MomentoRpcMethod.TopicSubscribe.ToMomentoLocalMetadataString() },
            StreamErrorMessageLimit = 3 // Receive an error after every 2 heartbeats
        };
        var testProps = new MomentoLocalCacheAndTopicClient(_authProvider, _loggerFactory, _cacheConfig, _topicConfig, momentoLocalArgs);

        var heartbeatTimeout = TimeSpan.FromSeconds(3);
        var heartbeatCounter = new HeartbeatTimestampCollector(heartbeatTimeout);
        
        var cts = new CancellationTokenSource();
        cts.CancelAfter(10_000);

        var subscribeResponse = await testProps.TopicClient.SubscribeAsync(testProps.CacheName, "topic");
        switch (subscribeResponse) 
        {
            case TopicSubscribeResponse.Subscription subscription:
                try
                {
                    var cancellableSubscription = subscription.WithCancellationForAllEvents(cts.Token);
                    await foreach (var topicEvent in cancellableSubscription)
                    {
                        switch (topicEvent)
                        {
                            case TopicSystemEvent.Heartbeat:
                                heartbeatCounter.AddTimestamp(DateTime.Now);
                                break;
                            case TopicMessage.Error error:
                                Assert.Equal(MomentoErrorCode.AUTHENTICATION_ERROR, error.ErrorCode);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected when the test times out
                }
                finally
                {
                    subscription.Dispose();
                }
                break;
            default:
                Assert.Fail("Expected subscription response, got error: {subscribeResponse.ToString()}");
                cts.Cancel();
                break;
        }
        
        Assert.Equal(0, heartbeatCounter.GetCountOfTimeouts()); // After the error, no heartbeats should be received, so no timeout period
        Assert.InRange(heartbeatCounter.GetCountOfTimestamps(), 1, 3); // Count of heartbeats before the error
    }

    [Fact]
    public void PublishDoesNotRetryOnAnyError() 
    {
        var testProps = new MomentoLocalCacheAndTopicClient(_authProvider, _loggerFactory, _cacheConfig, _topicConfig, null);
        testProps.CacheClient.IncrementAsync(testProps.CacheName, "key").Wait();
        Assert.Equal(0, testProps.TestMetricsCollector.GetTotalRetryCount(testProps.CacheName, MomentoRpcMethod.Increment));
    }
    
    [Fact]
    public async Task TopicSubscribe_ReturnsTimeoutError_IfFirstMessageNotReceivedBeforeDeadline() 
    {
        var momentoLocalArgs = new MomentoLocalMiddlewareArgs {
            DelayRpcList = new List<string> { MomentoRpcMethod.TopicSubscribe.ToMomentoLocalMetadataString() },
            DelayMillis = 1000 
        };
        var testProps = new MomentoLocalCacheAndTopicClient(_authProvider, _loggerFactory, _cacheConfig, _topicConfig.WithClientTimeout(TimeSpan.FromMilliseconds(500)), momentoLocalArgs);

        var cts = new CancellationTokenSource();
        cts.CancelAfter(10_000);
        
        var subscribeResponse = await testProps.TopicClient.SubscribeAsync(testProps.CacheName, "topic");
        switch (subscribeResponse) 
        {
            case TopicSubscribeResponse.Subscription subscription:
                try
                {
                    var cancellableSubscription = subscription.WithCancellationForAllEvents(cts.Token);
                    await foreach (var topicEvent in cancellableSubscription)
                    {
                        switch (topicEvent)
                        {
                            case TopicSystemEvent.Heartbeat:
                                break;
                            case TopicMessage.Error error:
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected when the test times out
                }
                finally
                {
                    subscription.Dispose();
                }
                break;
            case TopicSubscribeResponse.Error error:
                Assert.Equal(MomentoErrorCode.TIMEOUT_ERROR, error.ErrorCode);
                break;
            default:
                Assert.Fail("Expected subscription response, got error: {subscribeResponse.ToString()}");
                cts.Cancel();
                break;
        }
        
    }
}