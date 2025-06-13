using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Exceptions;
using System;
using Xunit;

namespace Momento.Sdk.Tests.Unit;

public class SubscriptionRetryStrategyTest
{
    [Fact]
    public void DefaultSubscriptionRetryStrategy_ShouldReturn500MillisForDefaultStrategy()
    {
        var loggerFactory = new LoggerFactory();
        var strategy = new DefaultSubscriptionRetryStrategy(loggerFactory.CreateLogger<DefaultSubscriptionRetryStrategy>());

        Assert.NotNull(strategy.SubscriptionRetryEligibilityStrategy);
        Assert.Equal(TimeSpan.FromMilliseconds(500), strategy.RetryDelayInterval);

        var exception = new InternalServerException("Test exception");
        Assert.Equal(500, strategy.DetermineWhenToResubscribe(exception));
    }

    [Fact]
    public void DefaultSubscriptionRetryStrategy_ShouldReturn1000MillisForConfiguredStrategy()
    {
        var loggerFactory = new LoggerFactory();
        var strategy = new DefaultSubscriptionRetryStrategy(loggerFactory.CreateLogger<DefaultSubscriptionRetryStrategy>(), retryDelayInterval: TimeSpan.FromMilliseconds(1000));

        Assert.NotNull(strategy.SubscriptionRetryEligibilityStrategy);
        Assert.Equal(TimeSpan.FromMilliseconds(1000), strategy.RetryDelayInterval);

        var exception = new InternalServerException("Test exception");
        Assert.Equal(1000, strategy.DetermineWhenToResubscribe(exception));
    }

    [Fact]
    public void DefaultSubscriptionRetryStrategy_ShouldReturn0MillisForConfiguredStrategy()
    {
        var loggerFactory = new LoggerFactory();
        var strategy = new DefaultSubscriptionRetryStrategy(loggerFactory.CreateLogger<DefaultSubscriptionRetryStrategy>(), retryDelayInterval: TimeSpan.FromMilliseconds(0));

        Assert.NotNull(strategy.SubscriptionRetryEligibilityStrategy);
        Assert.Equal(TimeSpan.FromMilliseconds(0), strategy.RetryDelayInterval);

        var exception = new InternalServerException("Test exception");
        Assert.Equal(0, strategy.DetermineWhenToResubscribe(exception));
    }

    [Fact]
    public void DetermineWhenToResubscribe_ShouldReturnNullForNonRetryableException()
    {
        var loggerFactory = new LoggerFactory();
        var strategy = new DefaultSubscriptionRetryStrategy(loggerFactory.CreateLogger<DefaultSubscriptionRetryStrategy>());

        var transportDetails = new MomentoErrorTransportDetails(new MomentoGrpcErrorDetails(
            Grpc.Core.StatusCode.NotFound,
            "Not found error"
        ));
        var exception = new NotFoundException("Test exception", transportDetails);
        Assert.Null(strategy.DetermineWhenToResubscribe(exception));
    }
}
