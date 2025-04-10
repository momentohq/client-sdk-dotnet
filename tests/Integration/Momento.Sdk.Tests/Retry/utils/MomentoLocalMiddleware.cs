using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Internal.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Momento.Sdk.Tests.Integration.Retry;

public class MomentoLocalMiddlewareArgs
{
    /// <summary>
    /// Error to return from momento-local.
    /// </summary>
    public string? ReturnError { get; set; } = null;
    /// <summary>
    /// The RPCs for which to return an error.
    /// </summary>
    public List<string>? ErrorRpcList { get; set; } = null;
    /// <summary>
    /// The number of requests for which to return the configured error.
    /// </summary>
    public int? ErrorCount { get; set; } = null;
    /// <summary>
    /// The RPCs for which to delay the response.
    /// </summary>
    public List<string>? DelayRpcList { get; set; } = null;
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
    public List<string>? StreamErrorRpcList { get; set; } = null;
    /// <summary>
    /// The error to return from within an active stream.
    /// </summary>
    public string? StreamError { get; set; } = null;
    /// <summary>
    /// The number of requests for which to return the configured error for stream methods.
    /// </summary>
    public int? StreamErrorMessageLimit { get; set; } = null;
}

public class MomentoLocalMiddleware : IMiddleware, ITopicMiddleware
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

    public int StreamEstablishedCounter { get; set; } = 0;
    public int StreamDisconnectedCounter { get; set; } = 0;

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
        headers.TryAddHeader("request-id", RequestId);
        headers.TryAddHeader("return-error", _args.ReturnError);
        headers.TryAddHeader("error-rpcs", _args.ErrorRpcList);
        headers.TryAddHeader("error-count", _args.ErrorCount);
        headers.TryAddHeader("delay-rpcs", _args.DelayRpcList);
        headers.TryAddHeader("delay-ms", _args.DelayMillis);
        headers.TryAddHeader("delay-count", _args.DelayCount);
        headers.TryAddHeader("stream-error-rpcs", _args.StreamErrorRpcList);
        headers.TryAddHeader("stream-error", _args.StreamError);
        headers.TryAddHeader("stream-error-message-limit", _args.StreamErrorMessageLimit);

        // Get the cache name from the metadata that should already exist on the request.
        var cacheName = headers.GetValue("cache") ?? "default-cache-name";

        // Request name appears as "Momento.Protos.CacheClient._GetRequest" so we need to extract the last part.
        var requestName = request.GetType().ToString().Split('.').Last();

        // Then convert to the appropriate enum
        var rpcMethod = MomentoRpcMethodExtensions.FromString(requestName);
        TestMetricsCollector.AddTimestamp(cacheName, rpcMethod, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

        var nextState = await continuation(request, callOptionsWithHeaders);
        return new MiddlewareResponseState<TResponse>(
            ResponseAsync: nextState.ResponseAsync,
            ResponseHeadersAsync: nextState.ResponseHeadersAsync,
            GetStatus: nextState.GetStatus,
            GetTrailers: nextState.GetTrailers
        );
    }

    public IList<Tuple<string, string>> WithHeaders()
    {
        var headerMappings = new Dictionary<string, string?>
        {
            { "request-id", RequestId },
            { "return-error", _args.ReturnError },
            { "error-rpcs", ConvertToMetadataList(_args.ErrorRpcList) },
            { "error-count", _args.ErrorCount?.ToString() },
            { "delay-rpcs", ConvertToMetadataList(_args.DelayRpcList) },
            { "delay-ms", _args.DelayMillis?.ToString() },
            { "delay-count", _args.DelayCount?.ToString() },
            { "stream-error-rpcs", ConvertToMetadataList(_args.StreamErrorRpcList) },
            { "stream-error", _args.StreamError },
            { "stream-error-message-limit", _args.StreamErrorMessageLimit?.ToString() }
        };

        var headers = new List<Tuple<string, string>>();
        foreach (var pair in headerMappings)
        {
            string key = pair.Key;
            string? value = pair.Value;
            {
                if (value != null)
                {
                    headers.Add(new Tuple<string, string>(key, value.ToString()));
                }
            }
        }
        return headers;
    }

    private string? ConvertToMetadataList(IList<string>? values)
    {
        if (values == null || values.Count == 0)
        {
            return null;
        }
        return string.Join(" ", values);
    }

    public void OnStreamDisconnected()
    {
        this.StreamDisconnectedCounter++;
    }

    public void onStreamEstablished()
    {
        this.StreamEstablishedCounter++;
    }
}
