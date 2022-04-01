using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Collections.Generic;

namespace MomentoSdk
{
    class Header
    {
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
        private readonly List<Header> headersToAddEveryTime;
        private readonly List<Header> headersToAddOnce;
        private volatile Boolean isUserAgentSent = false;
        public HeaderInterceptor(List<Header> headers)
        {
            foreach(Header header in headers) {
                if (header.Name == "Authorization") {
                    this.headersToAddEveryTime.Add(new Header(name: header.Name, value: header.Value));
                } else {
                    this.headersToAddOnce.Add(new Header(name: header.Name, value: header.Value));
                }
            }
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
            if (!isUserAgentSent) {
                foreach (Header header in this.headersToAddOnce)
                {
                    headers.Add(header.Name, header.Value);
                }
                isUserAgentSent = true;
            } else {
                // Only add Authorization metadata
                foreach (Header header in this.headersToAddEveryTime)
                {
                    headers.Add(header.Name, header.Value);
                }
            }
        }
    }
}
