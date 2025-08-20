using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using System.Collections.Generic;

namespace Momento.Sdk.Tests.Integration.Retry;

public class MomentoLocalCacheAndCacheClient
{
    public ICacheClient CacheClient { get; }
    public string CacheName { get; }
    public TestRetryMetricsCollector TestMetricsCollector { get; }

    public MomentoLocalCacheAndCacheClient(ICredentialProvider authProvider, ILoggerFactory loggerFactory, IConfiguration cacheConfig, MomentoLocalMiddlewareArgs? args, IRetryStrategy retryStrategy)
    {
        var middleware = new MomentoLocalMiddleware(loggerFactory, args);
        TestMetricsCollector = middleware.TestMetricsCollector;
        var config = cacheConfig.WithRetryStrategy(retryStrategy).WithAdditionalMiddlewares(new List<IMiddleware>() { middleware });
        CacheClient = new CacheClient(config, authProvider, TimeSpan.FromSeconds(60));
        CacheName = Utils.NewGuidString();
        CacheClient.CreateCacheAsync(CacheName).Wait();
    }
}

public class MomentoLocalCacheAndTopicClient
{
    public ICacheClient CacheClient { get; }
    public ITopicClient TopicClient { get; }
    public string CacheName { get; }
    public TestRetryMetricsCollector TestMetricsCollector { get; }
    public MomentoLocalTopicConfiguration TestTopicConfig { get; }

    public MomentoLocalCacheAndTopicClient(ICredentialProvider authProvider, ILoggerFactory loggerFactory, IConfiguration cacheConfig, ITopicConfiguration topicConfig, MomentoLocalMiddlewareArgs? args)
    {
        TestTopicConfig = new MomentoLocalTopicConfiguration(loggerFactory, topicConfig.TransportStrategy, topicConfig.SubscriptionRetryStrategy, args)
        {
            RequestId = Utils.NewGuidString()
        };
        TestMetricsCollector = new TestRetryMetricsCollector();
        CacheClient = new CacheClient(cacheConfig, authProvider, TimeSpan.FromSeconds(60));
        CacheName = Utils.NewGuidString();
        CacheClient.CreateCacheAsync(CacheName).Wait();
        TopicClient = new TopicClient(TestTopicConfig, authProvider);
    }
}
