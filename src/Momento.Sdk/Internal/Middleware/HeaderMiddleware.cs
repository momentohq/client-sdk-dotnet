using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Middleware;

namespace Momento.Sdk.Internal.Middleware
{
    internal class Header
    {
        public const string AuthorizationKey = "authorization";
        public const string AgentKey = "agent";
        public const string RuntimeVersionKey = "runtime-version";
        public const string ReadConcern = "read-concern";
        public readonly List<string> onceOnlyHeaders = new List<string> { Header.AgentKey, Header.RuntimeVersionKey };
        public string Name;
        public string Value;
        public Header(String name, String value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            var otherHeader = (Header)obj;
            return Name.Equals(otherHeader.Name) && Value.Equals(otherHeader.Value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal class HeaderMiddleware : IMiddleware
    {
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

        /// <inheritdoc/>
        public IList<Tuple<string, string>> AddStreamRequestHeaders()
        {
            return _headers.Select(header => new Tuple<string, string>(header.Name, header.Value)).ToList();
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            var other = (HeaderMiddleware)obj;
            return _headers.SequenceEqual(other._headers) &&
                headersToAddEveryTime.SequenceEqual(other.headersToAddEveryTime) &&
                headersToAddOnce.SequenceEqual(other.headersToAddOnce) &&
                areOnlyOnceHeadersSent == other.areOnlyOnceHeadersSent;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

