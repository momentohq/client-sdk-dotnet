using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses.Vector;

/// <summary>
/// Parent response type for a count items request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CountItemsResponse.Success</description></item>
/// <item><description>CountItemsResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CountItemsResponse.Success successResponse)
/// {
///     return successResponse.ItemCount;
/// }
/// else if (response is CountItemsResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CountItemsResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : CountItemsResponse
    {
        /// <summary>
        /// The number of items in the vector index.
        /// </summary>
        public long ItemCount { get; }

        /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
        /// <param name="itemCount">The number of items in the vector index.</param>
        public Success(long itemCount)
        {
            ItemCount = itemCount;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: {ItemCount}";
        }

    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CountItemsResponse, IError
    {
        /// <include file="../../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(SdkException error)
        {
            InnerException = error;
        }

        /// <inheritdoc />
        public SdkException InnerException { get; }

        /// <inheritdoc />
        public MomentoErrorCode ErrorCode => InnerException.ErrorCode;

        /// <inheritdoc />
        public string Message => $"{InnerException.MessageWrapper}: {InnerException.Message}";

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: {Message}";
        }

    }
}
