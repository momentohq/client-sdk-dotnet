namespace Momento.Sdk.Responses.Vector;

using Exceptions;

/// <summary>
/// Parent response type for a vector delete item batch request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>VectorDeleteItemBatchResponse.Success</description></item>
/// <item><description>VectorDeleteItemBatchResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is VectorDeleteItemBatchResponse.Success successResponse)
/// {
///     // handle success if needed
/// }
/// else if (response is VectorDeleteItemBatchResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// </code>
/// </summary>
public abstract class VectorDeleteItemBatchResponse
{

    /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : VectorDeleteItemBatchResponse { }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : VectorDeleteItemBatchResponse, IError
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
