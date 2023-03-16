using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache dictionary set if not exists field request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheSetIfNotExistsResponse.Stored</description></item>
/// <item><description>CacheSetIfNotExistsResponse.NotStored</description></item>
/// <item><description>CacheSetIfNotExistsResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheSetIfNotExistsResponse.Stored)
/// {
///     // handle successfully stored the field as appropriate
/// }
/// else if (response is CacheSetIfNotExistsResponse.NotStored)
/// {
///     // handle not stored the field due to existing value as appropriate
/// }
/// else if (response is CacheSetIfNotExistsResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheSetIfNotExistsResponse
{
    /// <include file="../docs.xml" path='docs/class[@name="Stored"]/description/*' />
    public class Stored : CacheSetIfNotExistsResponse
    {
    }

    /// <include file="../docs.xml" path='docs/class[@name="NotStored"]/description/*' />
    public class NotStored : CacheSetIfNotExistsResponse
    {
    }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheSetIfNotExistsResponse
    {
        private readonly SdkException _error;

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(SdkException error)
        {
            _error = error;
        }

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/prop[@name="InnerException"]/*' />
        public SdkException InnerException
        {
            get => _error;
        }

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/prop[@name="ErrorCode"]/*' />
        public MomentoErrorCode ErrorCode
        {
            get => _error.ErrorCode;
        }

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/prop[@name="Message"]/*' />
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
