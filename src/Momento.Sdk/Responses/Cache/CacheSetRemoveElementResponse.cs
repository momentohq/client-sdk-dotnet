﻿using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache set remove element request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheSetRemoveElementResponse.Success</description></item>
/// <item><description>CacheSetRemoveElementResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheSetRemoveElementResponse.Success successResponse)
/// {
///     // handle success as appropriate
/// }
/// else if (response is CacheSetRemoveElementResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheSetRemoveElementResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : CacheSetRemoveElementResponse
    {
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheSetRemoveElementResponse, IError
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
