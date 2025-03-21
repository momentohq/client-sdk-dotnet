using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;

// namespace Momento.Sdk.Tests.Integration.Cache;
namespace Momento.Sdk.Tests.Integration.Retry;

public class MomentoLocalMiddlewareArgs
{
    /// <summary>
    /// Error to return from momento-local.
    /// </summary>
    public MomentoErrorCode? ReturnError { get; set; } = null;
    /// <summary>
    /// The RPCs for which to return an error.
    /// </summary>
    public List<MomentoRpcMethod>? ErrorRpcList { get; set; } = null;
    /// <summary>
    /// The number of requests for which to return the configured error.
    /// </summary>
    public int? ErrorCount { get; set; } = null;
    /// <summary>
    /// The RPCs for which to delay the response.
    /// </summary>
    public List<MomentoRpcMethod>? DelayRpcList { get; set; } = null;
    /// <summary>
    /// How long to delay a response.
    /// </summary>
    public int? DelayMillis { get; set; } = null;
    /// <summary>
    /// The number of requests for which to delay the response.
    /// </summary>
    public int? DelayCount { get; set; } = null;
    /// <summary>
    /// The RPCs for which to return an error. Currently the only stream method is TopicSubscribe.
    /// </summary>
    public List<MomentoRpcMethod>? StreamErrorRpcList { get; set; } = null;
    /// <summary>
    /// The error to return from within an active stream.
    /// </summary>
    public MomentoErrorCode? StreamError { get; set; } = null;
    /// <summary>
    /// The number of requests for which to return the configured error for stream methods.
    /// </summary>
    public int? StreamErrorMessageLimit { get; set; } = null;
}

public class MomentoLocalMiddleware : IMiddleware
{
    /// <inheritdoc cref="Microsoft.Extensions.Logging.ILogger" />
    private readonly ILogger _logger;
    /// <summary>
    /// Arguments for configuring the behavior of momento-local for requests
    /// using the given request ID.
    /// </summary>
    private readonly MomentoLocalMiddlewareArgs _args;
    /// <summary>
    /// Metrics collector class for documenting timestamps of retries for test assertions.
    /// </summary>
    public TestRetryMetricsCollector TestMetricsCollector { get; set; } = new TestRetryMetricsCollector();
    /// <summary>
    /// ID to uniquely identify api calls in a test.
    /// </summary>
    public string RequestId { get; set; } = Utils.NewGuidString();

    public MomentoLocalMiddleware(ILoggerFactory loggerFactory, MomentoLocalMiddlewareArgs? args)
    {
        _logger = loggerFactory.CreateLogger<MomentoLocalMiddleware>();
        _args = args ?? new MomentoLocalMiddlewareArgs();
    }

    public async Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(
        TRequest request,
        CallOptions callOptions,
        Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuation
    ) where TRequest : class where TResponse : class
    {
        var callOptionsWithHeaders = callOptions;
        if (callOptionsWithHeaders.Headers == null)
        {
            callOptionsWithHeaders = callOptionsWithHeaders.WithHeaders(new Metadata());
        }

        var headers = callOptionsWithHeaders.Headers!;

        _logger.LogDebug($"MomentoLocalMiddleware: requestId={RequestId}");
        headers.Add("request-id", RequestId);

        if (_args.ReturnError != null)
        {
            _logger.LogDebug($"MomentoLocalMiddleware: return-error={_args.ReturnError}");
            headers.Add("return-error", MomentoErrorCodeMetadataConverter.ToStringValue((MomentoErrorCode)_args.ReturnError));
        }
        if (_args.ErrorRpcList != null && _args.ErrorRpcList.Count > 0)
        {
            var errorRpcList = string.Join(" ", _args.ErrorRpcList.Select(MomentoRpcMethodExtensions.ToStringValue));
            _logger.LogDebug($"MomentoLocalMiddleware: error-rpcs={errorRpcList}");
            headers.Add("error-rpcs", errorRpcList);
        }
        if (_args.ErrorCount != null)
        {
            _logger.LogDebug($"MomentoLocalMiddleware: error-count={_args.ErrorCount}");
            headers.Add("error-count", _args.ErrorCount.ToString()!);
        }
        if (_args.DelayRpcList != null && _args.DelayRpcList.Count > 0)
        {
            var delayRpcList = string.Join(" ", _args.DelayRpcList.Select(MomentoRpcMethodExtensions.ToStringValue));
            _logger.LogDebug($"MomentoLocalMiddleware: delay-rpcs={delayRpcList}");
            headers.Add("delay-rpcs", delayRpcList);
        }
        if (_args.DelayMillis != null)
        {
            _logger.LogDebug($"MomentoLocalMiddleware: delay-ms={_args.DelayMillis}");
            headers.Add("delay-ms", _args.DelayMillis.ToString()!);
        }
        if (_args.DelayCount != null)
        {
            _logger.LogDebug($"MomentoLocalMiddleware: delay-count={_args.DelayCount}");
            headers.Add("delay-count", _args.DelayCount.ToString()!);
        }
        if (_args.StreamErrorRpcList != null && _args.StreamErrorRpcList.Count > 0)
        {
            var streamErrorRpcList = string.Join(" ", _args.StreamErrorRpcList.Select(MomentoRpcMethodExtensions.ToStringValue));
            _logger.LogDebug($"MomentoLocalMiddleware: stream-error-rpcs={streamErrorRpcList}");
            headers.Add("stream-error-rpcs", streamErrorRpcList);
        }
        if (_args.StreamError != null)
        {
            _logger.LogDebug($"MomentoLocalMiddleware: stream-error={_args.StreamError}");
            headers.Add("stream-error", MomentoErrorCodeMetadataConverter.ToStringValue((MomentoErrorCode)_args.StreamError));
        }
        if (_args.StreamErrorMessageLimit != null)
        {
            _logger.LogDebug($"MomentoLocalMiddleware: stream-error-message-limit={_args.StreamErrorMessageLimit}");
            headers.Add("stream-error-message-limit", _args.StreamErrorMessageLimit.ToString()!);
        }

        // Get the cache name from the metadata that should already exist on the request.
        var cacheName = headers.GetValue("cache") ?? "default-cache-name";

        // Request name appears as "Momento.Protos.CacheClient._GetRequest" so we need to extract the last part.
        var requestName = request.GetType().ToString().Split('.').Last();

        // Then convert to the approrpriate enum
        var rpcMethod = MomentoRpcMethodExtensions.FromString(requestName);
        TestMetricsCollector.AddTimestamp(cacheName, rpcMethod, 1);

        var nextState = await continuation(request, callOptionsWithHeaders);
        return new MiddlewareResponseState<TResponse>(
            ResponseAsync: nextState.ResponseAsync,
            ResponseHeadersAsync: nextState.ResponseHeadersAsync,
            GetStatus: nextState.GetStatus,
            GetTrailers: nextState.GetTrailers
        );
    }
}
