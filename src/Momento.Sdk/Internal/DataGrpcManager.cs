using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Momento.Protos.CacheClient;
using Momento.Sdk.Config.Middleware;
using static System.Reflection.Assembly;
using static Grpc.Core.Interceptors.Interceptor;

namespace Momento.Sdk.Internal;

public class DataGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;
    public Scs.ScsClient Client { get; }

    private readonly string version = "dotnet:" + GetAssembly(typeof(Momento.Sdk.Responses.CacheGetResponse)).GetName().Version.ToString();
    // Some System.Environment.Version remarks to be aware of
    // https://learn.microsoft.com/en-us/dotnet/api/system.environment.version?view=netstandard-2.0#remarks
    private readonly string runtimeVersion = "dotnet:" + System.Environment.Version;

    internal DataGrpcManager(IList<IMiddleware> middlewares, string authToken, string host)
    {
        var url = $"https://{host}";
        this.channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
        List<Header> headers = new List<Header> { new Header(name: Header.AuthorizationKey, value: authToken), new Header(name: Header.AgentKey, value: version), new Header(name: Header.RuntimeVersionKey, value: runtimeVersion) };


        CallInvoker invokerWithMiddlewares = middlewares.Aggregate(
            this.channel.CreateCallInvoker(),
            (invoker, middleware) =>
            {
                Console.WriteLine($"Adding an interceptor: {middleware}");
                return invoker.Intercept(new MiddlewareInterceptor(middleware));
            }
        );

        CallInvoker invoker = invokerWithMiddlewares
            .Intercept(new HeaderInterceptor(headers));
        Client = new Scs.ScsClient(invoker);
    }

    public void Dispose()
    {
        this.channel.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Creates an interceptor from our IMiddleware interface.  Note that this
/// only works for AsyncUnary requests; the signatures for any of the other
/// types of requests are too variable.
/// </summary>
internal class MiddlewareInterceptor : Interceptor
{
    private readonly IMiddleware _middleware;

    internal MiddlewareInterceptor(IMiddleware middleware)
    {
        _middleware = middleware;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        TaskCompletionSource<AsyncUnaryCall<TResponse>> theAsyncUnaryCallTaskSource = new TaskCompletionSource<AsyncUnaryCall<TResponse>>();
        Task<AsyncUnaryCall<TResponse>> theAsyncUnaryCallTask = theAsyncUnaryCallTaskSource.Task;
        var middlewareState = _middleware.WrapRequest(request, context, (r, c) =>
        {
            AsyncUnaryCall<TResponse> theAsyncUnaryCall = continuation(r, c);
            theAsyncUnaryCallTaskSource.SetResult(theAsyncUnaryCall);
            return new MiddlewareResponseState<TResponse>(
                ResponseAsync: theAsyncUnaryCall.ResponseAsync,
                ResponseHeadersAsync: theAsyncUnaryCall.ResponseHeadersAsync,
                GetStatus: theAsyncUnaryCall.GetStatus,
                GetTrailers: theAsyncUnaryCall.GetTrailers
                );
        });

        return new AsyncUnaryCall<TResponse>(
            responseAsync: middlewareState.ResponseAsync,
            responseHeadersAsync: middlewareState.ResponseHeadersAsync,
            getStatusFunc: middlewareState.GetStatus,
            getTrailersFunc: middlewareState.GetTrailers,
            disposeAction: theAsyncUnaryCallTask.Result.Dispose
        );
    }
}
