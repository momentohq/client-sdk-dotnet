using Microsoft.Extensions.Logging;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;
using System.Timers;

namespace TopicExample;

public class Driver
{
    private const string CacheNameEnvVar = "MOMENTO_CACHE_NAME";
    private const string TopicName = "example-topic";
    private static readonly ILogger Logger;
    private static readonly ILoggerFactory LoggerFactory;
    private static System.Timers.Timer _heartbeatTimer;

    private static TimeSpan HeartbeatTimeout;

    static Driver()
    {
        LoggerFactory = InitializeLogging();
        Logger = LoggerFactory.CreateLogger<Driver>();

        // Experiment with values less than and greater than 10
        HeartbeatTimeout = TimeSpan.FromSeconds(5);

        // Initialize and configure the heartbeat timer
        _heartbeatTimer = new(HeartbeatTimeout.TotalMilliseconds);
        _heartbeatTimer.Elapsed += HeartbeatTimeoutHandler;
        // Only trigger once if no heartbeat is received.
        _heartbeatTimer.AutoReset = false;

    }

    public static async Task Main()
    {
        var authToken = new EnvMomentoV2TokenProvider();
        var cacheName = ReadCacheName();

        // Set up the client
        using ICacheClient client =
            new CacheClient(Configurations.Laptop.V1(LoggerFactory), authToken, TimeSpan.FromSeconds(60));
        await EnsureCacheExistsAsync(client, cacheName);
        using ITopicClient topicClient = new TopicClient(TopicConfigurations.Laptop.latest(LoggerFactory), authToken);
        try
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(30_000);

            // Subscribe and begin receiving messages
            var subscriptionTask = Task.Run(async () =>
            {
                var subscribeResponse = await topicClient.SubscribeAsync(cacheName, TopicName);
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
                                    case TopicMessage.Binary:
                                        Logger.LogInformation("Received unexpected binary message from topic.");
                                        break;
                                    case TopicMessage.Text text:
                                        Logger.LogInformation("Received string message from topic: {message}",
                                            text.Value);
                                        break;
                                    case TopicSystemEvent.Heartbeat:
                                        Logger.LogInformation("Received heartbeat from topic.");
                                        ResetHeartbeatTimer();  // Reset the timer on heartbeat
                                        break;
                                    case TopicSystemEvent.Discontinuity discontinuity:
                                        Logger.LogInformation($"Received discontinuity from topic: {discontinuity}");
                                        break;
                                    case TopicMessage.Error error:
                                        Logger.LogInformation("Received error message from topic: {error}",
                                            error.Message);
                                        cts.Cancel();
                                        break;
                                }
                            }
                        }
                        finally
                        {
                            subscription.Dispose();
                        }

                        break;
                    case TopicSubscribeResponse.Error error:
                        Logger.LogInformation("Error subscribing to a topic: {error}", error.Message);
                        cts.Cancel();
                        break;
                }
            });

            // Publish messages
            var publishTask = Task.Run(async () =>
            {
                var messageCounter = 0;
                while (!cts.IsCancellationRequested)
                {
                    var publishResponse =
                        await topicClient.PublishAsync(cacheName, TopicName, $"message {messageCounter}");
                    switch (publishResponse)
                    {
                        case TopicPublishResponse.Success:
                            break;
                        case TopicPublishResponse.Error error:
                            Logger.LogInformation("Error publishing a message to the topic: {error}", error.Message);
                            cts.Cancel();
                            break;
                    }

                    await Task.Delay(1_000);
                    messageCounter++;
                }
            });

            // Start the heartbeat timer initially
            _heartbeatTimer.Start();

            await Task.WhenAll(subscriptionTask, publishTask);
        }
        finally
        {
            client.Dispose();
            topicClient.Dispose();
        }
    }

    private static void HeartbeatTimeoutHandler(object? sender, ElapsedEventArgs e)
    {
        Logger.LogWarning($"No heartbeat received in the last {HeartbeatTimeout} seconds.");
    }

    private static void ResetHeartbeatTimer()
    {
        _heartbeatTimer.Stop();
        _heartbeatTimer.Start();
    }

    private static ILoggerFactory InitializeLogging()
    {
        return Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "hh:mm:ss ";
            });
            builder.SetMinimumLevel(LogLevel.Information);
        });
    }

    private static string ReadCacheName()
    {
        var cacheName = Environment.GetEnvironmentVariable(CacheNameEnvVar);
        return cacheName ?? "default-cache";
    }

    private static async Task EnsureCacheExistsAsync(ICacheClient client, string cacheName)
    {
        Logger.LogInformation("Creating cache {cacheName} if it doesn't already exist.", cacheName);
        var createCacheResponse = await client.CreateCacheAsync(cacheName);
        switch (createCacheResponse)
        {
            case CreateCacheResponse.Success:
                Logger.LogInformation("Created cache {cacheName}.", cacheName);
                break;
            case CreateCacheResponse.CacheAlreadyExists:
                Logger.LogInformation("Cache {cacheName} already exists.", cacheName);
                break;
            case CreateCacheResponse.Error:
                Logger.LogInformation("Error creating cache: {error.Message}", cacheName);
                Environment.Exit(1);
                break;
        }
    }
}
