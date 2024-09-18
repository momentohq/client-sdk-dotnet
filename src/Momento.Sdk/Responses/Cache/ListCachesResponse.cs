using System;
using System.Collections.Generic;
using Momento.Protos.ControlClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a list caches request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>ListCachesResponse.Success</description></item>
/// <item><description>ListCachesResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is ListCachesResponse.Success successResponse)
/// {
///     return successResponse.Caches;
/// }
/// else if (response is ListCachesResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class ListCachesResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : ListCachesResponse
    {
        /// <summary>
        /// The list of caches available to the user represented as
        /// <see cref="Momento.Sdk.Responses.CacheInfo" /> objects.
        /// </summary>
        public List<CacheInfo> Caches { get; }

        /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
        /// <param name="result">gRPC list caches request result</param>
        public Success(_ListCachesResponse result)
        {
            Caches = new List<CacheInfo>();
            foreach (_Cache c in result.Cache)
            {
                Caches.Add(new CacheInfo(c.CacheName));
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            List<string> cacheNames = new List<string>();
            foreach (CacheInfo cacheInfo in Caches)
            {
                cacheNames.Add(cacheInfo.Name);
            }
            return $"{base.ToString()}: {String.Join(", ", cacheNames)}";
        }

    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : ListCachesResponse, IError
    {
        private readonly SdkException _error;

        /// <include file="../../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(SdkException error)
        {
            _error = error;
        }

        /// <inheritdoc />
        public SdkException InnerException
        {
            get => _error;
        }

        /// <inheritdoc />
        public MomentoErrorCode ErrorCode
        {
            get => _error.ErrorCode;
        }

        /// <inheritdoc />
        public string Message
        {
            get => $"{_error.MessageWrapper}: {_error.Message}";
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: {this.Message}";
        }

    }
}
