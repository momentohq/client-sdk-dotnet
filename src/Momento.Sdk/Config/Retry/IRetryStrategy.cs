using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Momento.Sdk.Config.Retry;

/// <summary>
/// Defines a contract for how and when to retry a request
/// </summary>
public interface IRetryStrategy
{
    public ILoggerFactory? LoggerFactory { get; }
    public IRetryStrategy WithLoggerFactory(ILoggerFactory loggerFactory);

    /// <summary>
    /// Calculates whether or not to retry a request based on the type of request and number of attempts.
    /// </summary>
    /// <param name="grpcStatus"></param>
    /// <param name="grpcRequest"></param>
    /// <param name="attemptNumber"></param>
    /// <returns>Returns number of milliseconds after which the request should be retried, or <see langword="null"/> if the request should not be retried.</returns>
    public int? DetermineWhenToRetryRequest<TRequest>(Status grpcStatus, TRequest grpcRequest, int attemptNumber) where TRequest : class;
}
