﻿using System.Collections.Generic;
using System.Linq;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Messages.Vector;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Responses.Vector;

/// <summary>
/// Parent response type for a get item metadata batch request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>GetItemMetadataBatch.Success</description></item>
/// <item><description>GetItemMetadataBatch.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is GetItemMetadataBatch.Success successResponse)
/// {
///     return successResponse.Values;
/// }
/// else if (response is GetItemMetadataBatch.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class GetItemMetadataBatchResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : GetItemMetadataBatchResponse
    {
        /// <summary>
        /// The metadata for found items by ID.
        /// </summary>
        public Dictionary<string, Dictionary<string, MetadataValue>> Values { get; }

        /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
        /// <param name="values">the found items</param>
        public Success(Dictionary<string, Dictionary<string, MetadataValue>> values)
        {
            Values = values;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var displayedHits = Values.Take(5).Select(value => $"{value.Key} {value.Value.ToString()}");
            return $"{base.ToString()}: {string.Join(", ", displayedHits)}...";
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (!(obj is Success other))
            {
                return false;
            }

            if (Values.Count != other.Values.Count)
            {
                return false;
            }

            return Values.All(kv => other.Values.ContainsKey(kv.Key) && kv.Value.MetadataEquals(other.Values[kv.Key]));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var kv in Values)
            {
                hash = hash * 23 + kv.Key.GetHashCode();
                hash = hash * 23 + kv.Value.MetadataHashCode();
            }
            return hash;
        }
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : GetItemMetadataBatchResponse, IError
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
