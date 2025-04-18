﻿using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache list remove value request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheListRemoveValueResponse.Success</description></item>
/// <item><description>CacheListRemoveValueResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheListRemoveValueResponse.Success successResponse)
/// {
///     // handle success as appropriate
/// }
/// else if (response is CacheListRemoveValueResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheListRemoveValueResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : CacheListRemoveValueResponse
    {
        /// <summary>
        /// The length of the list post-remove operation.
        /// </summary>
        public int ListLength { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response">The cache response</param>
        public Success(_ListRemoveResponse response)
        {
            if (response.ListCase == _ListRemoveResponse.ListOneofCase.Found)
            {
                ListLength = checked((int)response.Found.ListLength);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: ListLength: {ListLength}";
        }
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheListRemoveValueResponse, IError
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
