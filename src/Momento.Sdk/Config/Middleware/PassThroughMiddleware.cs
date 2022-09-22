using System.Threading.Tasks;
using Momento.Sdk.Config.Retry;

namespace Momento.Sdk.Config.Middleware;

public class PassThroughMiddleware : IMiddleware
{
    public PassThroughMiddleware()
    {

    }

    public async Task<IGrpcResponse> wrapRequest(IMiddleware.MiddlewareFn middlewareFn, IGrpcRequest request)
    {
        return await middlewareFn(request);
    }
}
