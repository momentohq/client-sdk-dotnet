using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache set sample request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheSetSampleResponse.Success</description></item>
/// <item><description>CacheSetSampleResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheSetSampleResponse.Hit hitResponse)
/// {
///     return hitResponse.ValueSetString;
/// }
/// else if (response is CacheSetSampleResponse.Miss missResponse)
/// {
///     // handle miss as appropriate
/// }
/// else if (response is CacheSetSampleResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheSetSampleResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Hit"]/description/*' />
    public class Hit : CacheSetSampleResponse
    {
#pragma warning disable 1591
        protected readonly RepeatedField<ByteString> elements;
        protected readonly Lazy<ISet<byte[]>> _byteArraySet;
        protected readonly Lazy<ISet<string>> _stringSet;
#pragma warning restore 1591

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response">Cache set sample response.</param>
        public Hit(_SetSampleResponse response)
        {
            elements = response.Found.Elements;
            _byteArraySet = new(() =>
            {
                return new HashSet<byte[]>(
                    elements.Select(element => element.ToByteArray()),
                    Utils.ByteArrayComparer);
            });

            _stringSet = new(() =>
            {

                return new HashSet<string>(elements.Select(element => element.ToStringUtf8()));
            });
        }

        /// <summary>
        /// Randomly sample elements from the Set as a <see cref="ISet{T}" /> of <see cref="byte" /> arrays.
        /// </summary>
        public ISet<byte[]> ValueSetByteArray { get => _byteArraySet.Value; }

        /// <summary>
        /// Randomly sample elements from the Set as a <see cref="ISet{T}" /> of <see cref="string" />s.
        /// </summary>
        public ISet<string> ValueSetString { get => _stringSet.Value; }

        /// <inheritdoc />
        public override string ToString()
        {
            var stringRepresentation = String.Join(", ", ValueSetString.Select(value => $"\"{value}\""));
            var byteArrayRepresentation = String.Join(", ", ValueSetByteArray.Select(value => $"\"{value.ToPrettyHexString()}\""));
            return $"{base.ToString()}: ValueSetString: {{{stringRepresentation.Truncate()}}} ValueSetByteArray: {{{byteArrayRepresentation.Truncate()}}}";
        }
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Miss"]/description/*' />
    public class Miss : CacheSetSampleResponse
    {

    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheSetSampleResponse, IError
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
