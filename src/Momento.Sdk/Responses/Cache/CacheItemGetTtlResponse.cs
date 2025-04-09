namespace Momento.Sdk.Responses;

using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using System;

/// <summary>
/// Parent response type for a cache item get ttl request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheItemGetTtlResponse.Hit</description></item>
/// <item><description>CacheItemGetTtlResponse.Miss</description></item>
/// <item><description>CacheItemGetTtlResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheItemGetTtlResponse.Hit hitResponse)
/// {
///    // handle ttl value as appropriate
/// }
/// else if (response is CacheItemGetTtlResponse.Miss missResponse)
/// {
///    // handle key was not found as appropriate
/// }
/// else if (response is CacheItemGetTtlResponse.Error errorResponse)
/// {
///   // handle error as appropriate
/// }
/// else
/// {
///   // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheItemGetTtlResponse
{
    /// <summary>
    /// Indicates the key was found in the cache and the ttl was returned.
    /// </summary>
    public class Hit : CacheItemGetTtlResponse
    {
        /// <summary>
        /// The value of the ttl.
        /// </summary>
        protected readonly ulong value;

        /// <include file="../../docs.xml" path='docs/class[@name="Hit"]/description/*' />
        public Hit(_ItemGetTtlResponse response)
        {
            value = response.Found.RemainingTtlMillis;
        }

        /// <summary>
        /// The value of the ttl.
        /// </summary>
        public TimeSpan Value
        {
            get => TimeSpan.FromMilliseconds(value);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: Value: {Value}";
        }
    }

    /// <summary>
    /// Indicates the key was not found in the cache, hence the ttl was not returned.
    /// </summary>
    public class Miss : CacheItemGetTtlResponse { }

    /// <include file = "../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheItemGetTtlResponse, IError
    {
        private readonly SdkException _error;

        /// <include file = "../../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
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
            get => _error.Message;
        }
    }
}
