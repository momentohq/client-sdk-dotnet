using System.Threading.Tasks;
using Momento.Sdk.Config.Retry;

namespace Momento.Sdk.Config.Middleware;

/// <summary>
/// The Middleware interface allows the Configuration to provide a higher-order function that wraps all requests.
/// This allows future support for things like client-side metrics or other diagnostics helpers.
/// </summary>
public interface IMiddleware
{
    public delegate Task<IGrpcResponse> MiddlewareFn(IGrpcRequest request);

    /// <summary>
    /// Called as a wrapper around each request; can be used to time the request and collect metrics etc.
    /// </summary>
    /// <param name="middlewareFn"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task<IGrpcResponse> wrapRequest(MiddlewareFn middlewareFn, IGrpcRequest request);
}
