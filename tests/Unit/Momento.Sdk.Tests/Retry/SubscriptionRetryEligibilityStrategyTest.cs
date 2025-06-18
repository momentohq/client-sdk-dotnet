using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Exceptions;
using Xunit;

namespace Momento.Sdk.Tests.Unit;

public class SubscriptionRetryEligibilityStrategyTest
{
    [Fact]
    public void IsEligibleForResubscribe_ShouldReturnTrueForInternalServerException()
    {
        var loggerFactory = new LoggerFactory();
        var strategy = new DefaultSubscriptionRetryEligibilityStrategy(loggerFactory.CreateLogger<DefaultSubscriptionRetryEligibilityStrategy>());
        var exception = new InternalServerException("Test exception");

        Assert.True(strategy.IsEligibleForResubscribe(exception));
    }

    [Fact]
    public void IsEligibleForResubscribe_ShouldReturnFalseForNotFoundException()
    {
        var loggerFactory = new LoggerFactory();
        var strategy = new DefaultSubscriptionRetryEligibilityStrategy(loggerFactory.CreateLogger<DefaultSubscriptionRetryEligibilityStrategy>());
        var transportDetails = new MomentoErrorTransportDetails(new MomentoGrpcErrorDetails(
            Grpc.Core.StatusCode.NotFound,
            "Not found error"
        ));
        var exception = new NotFoundException("Test exception", transportDetails);

        Assert.False(strategy.IsEligibleForResubscribe(exception));
    }

    [Fact]
    public void IsEligibleForResubscribe_ShouldReturnFalseForAuthenticationException()
    {
        var loggerFactory = new LoggerFactory();
        var strategy = new DefaultSubscriptionRetryEligibilityStrategy(loggerFactory.CreateLogger<DefaultSubscriptionRetryEligibilityStrategy>());
        var transportDetails = new MomentoErrorTransportDetails(new MomentoGrpcErrorDetails(
            Grpc.Core.StatusCode.Unauthenticated,
            "Authentication error"
        ));
        var exception = new AuthenticationException("Test exception", transportDetails);

        Assert.False(strategy.IsEligibleForResubscribe(exception));
    }

    [Fact]
    public void IsEligibleForResubscribe_ShouldReturnFalseForPermissionDeniedException()
    {
        var loggerFactory = new LoggerFactory();
        var strategy = new DefaultSubscriptionRetryEligibilityStrategy(loggerFactory.CreateLogger<DefaultSubscriptionRetryEligibilityStrategy>());
        var transportDetails = new MomentoErrorTransportDetails(new MomentoGrpcErrorDetails(
            Grpc.Core.StatusCode.PermissionDenied,
            "Permission denied error"
        ));
        var exception = new PermissionDeniedException("Test exception", transportDetails);

        Assert.False(strategy.IsEligibleForResubscribe(exception));
    }

    [Fact]
    public void IsEligibleForResubscribe_ShouldReturnFalseForCancelledException()
    {
        var loggerFactory = new LoggerFactory();
        var strategy = new DefaultSubscriptionRetryEligibilityStrategy(loggerFactory.CreateLogger<DefaultSubscriptionRetryEligibilityStrategy>());
        var transportDetails = new MomentoErrorTransportDetails(new MomentoGrpcErrorDetails(
            Grpc.Core.StatusCode.Cancelled,
            "Cancelled error"
        ));
        var exception = new CancelledException("Test exception", transportDetails);

        Assert.False(strategy.IsEligibleForResubscribe(exception));
    }
}
