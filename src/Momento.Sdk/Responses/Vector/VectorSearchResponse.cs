using System.Collections.Generic;
using System.Linq;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Responses.Vector;

/// <summary>
/// Parent response type for a list vector indexes request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>VectorSearchResponse.Success</description></item>
/// <item><description>VectorSearchResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is VectorSearchResponse.Success successResponse)
/// {
///     return successResponse.Hits;
/// }
/// else if (response is VectorSearchResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class VectorSearchResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : VectorSearchResponse
    {
        /// <summary>
        /// The list of hits returned by the search.
        /// </summary>
        public List<SearchHit> Hits { get; }

        /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
        /// <param name="hits">the search results</param>
        public Success(List<SearchHit> hits)
        {
            Hits = hits;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var displayedHits = Hits.Take(5).Select(hit => $"{hit.Id} ({hit.Distance})");
            return $"{base.ToString()}: {string.Join(", ", displayedHits)}...";
        }

    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : VectorSearchResponse, IError
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
