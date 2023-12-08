using System.Collections.Generic;
using System.Linq;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Requests.Vector;

namespace Momento.Sdk.Responses.Vector;

/// <summary>
/// Parent response type for a get item batch request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>GetItemBatch.Success</description></item>
/// <item><description>GetItemBatch.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is GetItemBatch.Success successResponse)
/// {
///     return successResponse.Values;
/// }
/// else if (response is GetItemBatch.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class GetItemBatchResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : GetItemBatchResponse
    {
        /// <summary>
        /// The found items by ID.
        /// </summary>
        public Dictionary<string, Item> Values { get; }

        /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
        /// <param name="values">the found items</param>
        public Success(Dictionary<string, Item> values)
        {
            Values = values;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var displayedHits = Values.Take(5).Select(value => $"{value.Value.ToString()}");
            return $"{base.ToString()}: {string.Join(", ", displayedHits)}...";
        }

    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : GetItemBatchResponse, IError
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
