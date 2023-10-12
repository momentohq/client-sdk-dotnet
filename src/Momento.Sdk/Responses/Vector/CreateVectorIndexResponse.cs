namespace Momento.Sdk.Responses.Vector;

using Exceptions;

/// <summary>
/// Parent response type for a create vector index request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CreateVectorIndexResponse.Success</description></item>
/// <item><description>CreateVectorIndexResponse.IndexAlreadyExists</description></item>
/// <item><description>CreateVectorIndexResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CreateVectorIndexResponse.Success successResponse)
/// {
///     // handle success if needed
/// }
/// else if (response is CreateVectorIndexResponse.IndexAlreadyExists alreadyExistsResponse)
/// {
///     // handle already exists as appropriate
/// }
/// else if (response is CreateVectorIndexResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// </code>
/// </summary>
public abstract class CreateVectorIndexResponse
{

    /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : CreateVectorIndexResponse { }

    /// <summary>
    /// Class <c>IndexAlreadyExists</c> indicates that an index with the requested name
    /// has already been created in the requesting account.
    /// </summary>
    public class IndexAlreadyExists : CreateVectorIndexResponse { }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CreateVectorIndexResponse, IError
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
