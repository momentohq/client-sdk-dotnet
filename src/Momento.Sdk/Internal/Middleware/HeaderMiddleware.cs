using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Middleware;

namespace Momento.Sdk.Internal.Middleware
{
    internal class Header
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

    internal class HeaderMiddleware : IMiddleware
    {
        public ILoggerFactory? LoggerFactory { get; }

        private readonly List<Header> _headers;
        private readonly List<Header> headersToAddEveryTime = new List<Header> { };
        private readonly List<Header> headersToAddOnce = new List<Header> { };
        private volatile Boolean areOnlyOnceHeadersSent = false;

        public HeaderMiddleware(ILoggerFactory loggerFactory, List<Header> headers)
        {
            _headers = headers;
            this.headersToAddOnce = headers.Where(header => header.onceOnlyHeaders.Contains(header.Name)).ToList();
            this.headersToAddEveryTime = headers.Where(header => !header.onceOnlyHeaders.Contains(header.Name)).ToList();
        }

        public HeaderMiddleware WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            return new(loggerFactory, _headers);
        }

        IMiddleware IMiddleware.WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            return WithLoggerFactory(loggerFactory);
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


            var nextState = await continuation(request, callOptionsWithHeaders);
            return new MiddlewareResponseState<TResponse>(
                ResponseAsync: nextState.ResponseAsync,
                ResponseHeadersAsync: nextState.ResponseHeadersAsync,
                GetStatus: nextState.GetStatus,
                GetTrailers: nextState.GetTrailers
            );
        }
    }
}

