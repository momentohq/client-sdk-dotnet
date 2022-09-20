using System;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Momento.Sdk.Internal;

class Header
{
    public const string AuthorizationKey = "Authorization";
    public const string AgentKey = "Agent";
    public const string RuntimeVersionKey = "Runtime_Version";
    public readonly List<string> onceOnlyHeaders = new List<string> { Header.AgentKey, Header.RuntimeVersionKey };
    public string Name;
    public string Value;
    public Header(String name, String value)
    {
        this.Name = name;
        this.Value = value;
    }

}
class HeaderInterceptor : Grpc.Core.Interceptors.Interceptor
{
    private readonly List<Header> headersToAddEveryTime = new List<Header> { };
    private readonly List<Header> headersToAddOnce = new List<Header> { };
    private volatile Boolean areOnlyOnceHeadersSent = false;
    public HeaderInterceptor(List<Header> headers)
    {
        this.headersToAddOnce = headers.Where(header => header.onceOnlyHeaders.Contains(header.Name)).ToList();
        this.headersToAddEveryTime = headers.Where(header => !header.onceOnlyHeaders.Contains(header.Name)).ToList();
    }

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        AddCallerMetadata(ref context);
        return continuation(request, context);
    }
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        AddCallerMetadata(ref context);
        return continuation(request, context);
    }
    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        AddCallerMetadata(ref context);
        return continuation(request, context);
    }
    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        AddCallerMetadata(ref context);
        return continuation(context);
    }
    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        AddCallerMetadata(ref context);
        return continuation(context);
    }

    private void AddCallerMetadata<TRequest, TResponse>(ref ClientInterceptorContext<TRequest, TResponse> context)
        where TRequest : class
        where TResponse : class
    {
        var headers = context.Options.Headers;

        // Call doesn't have a headers collection to add to.
        // Need to create a new context with headers for the call.
        if (headers == null)
        {
            headers = new Metadata();
            var options = context.Options.WithHeaders(headers);
            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, options);
        }
        foreach (Header header in this.headersToAddEveryTime)
        {
            headers.Add(header.Name, header.Value);
        }
        if (!areOnlyOnceHeadersSent)
        {
            foreach (Header header in this.headersToAddOnce)
            {
                headers.Add(header.Name, header.Value);
            }
            areOnlyOnceHeadersSent = true;
        }
    }
}
