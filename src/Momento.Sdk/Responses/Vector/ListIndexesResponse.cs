using System.Collections.Generic;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses.Vector;

/// <summary>
/// Parent response type for a list vector indexes request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>ListIndexesResponse.Success</description></item>
/// <item><description>ListIndexesResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is ListIndexesResponse.Success successResponse)
/// {
///     return successResponse.IndexNames;
/// }
/// else if (response is ListIndexesResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class ListIndexesResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : ListIndexesResponse
    {
        /// <summary>
        /// The list of vector available to the user.
        /// </summary>
        public List<string> IndexNames { get; }

        /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
        /// <param name="indexNames">the list of index names</param>
        public Success(List<string> indexNames)
        {
            IndexNames = indexNames;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: {string.Join(", ", IndexNames)}";
        }

    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : ListIndexesResponse, IError
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
