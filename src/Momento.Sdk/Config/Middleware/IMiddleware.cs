using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Retry;
using System;
using System.Threading.Tasks;

namespace Momento.Sdk.Config.Middleware;

/// <summary>
/// Contains the state of a Response object during the life cycle of a Middleware.
/// Middlewares may augment the Tasks herein, or call and replace the Funcs herein,
/// to alter the response before it proceeds to the next Middleware.
/// </summary>
/// <typeparam name="TResponse"></typeparam>
/// <param name="ResponseAsync">A Task that will be completed when the previous Middleware is done processing
/// the reponse.  Middlewares may use, e.g. `.ContinueWith` to access and modify
/// the response.</param>
/// <param name="ResponseHeadersAsync">A Task that will be completed when the previous Middleware is done processing
/// the reponse headers.  Middlewares may use, e.g. `.ContinueWith` to access and modify
/// the response headers.</param>
/// <param name="GetStatus">Returns the gRPC Status of the response.  Middlewares may override to alter
/// the Status.</param>
/// <param name="GetTrailers">Returns the trailers of the response.  Middlewares may override to alter
/// the trailers.</param>
public record struct MiddlewareResponseState<TResponse>(
    Task<TResponse> ResponseAsync,
    Task<Metadata> ResponseHeadersAsync,
    Func<Status> GetStatus,
    Func<Metadata> GetTrailers
);

/// <summary>
/// The Middleware interface allows the Configuration to provide a higher-order function that wraps all requests.
/// This allows future support for things like client-side metrics or other diagnostics helpers.
/// </summary>
public interface IMiddleware
{
    /// <summary>
    /// Called as a wrapper around each request; can be used to time the request and collect metrics etc.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="request"></param>
    /// <param name="callOptions"></param>
    /// <param name="continuation"></param>
    /// <returns></returns>
    /// 
    public Task<MiddlewareResponseState<TResponse>> WrapRequest<TRequest, TResponse>(
        TRequest request,
        CallOptions callOptions,
        Func<TRequest, CallOptions, Task<MiddlewareResponseState<TResponse>>> continuation
    ) where TRequest : class where TResponse : class;

}
