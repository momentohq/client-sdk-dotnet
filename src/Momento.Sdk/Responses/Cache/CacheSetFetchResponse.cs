﻿using Google.Protobuf;
using Google.Protobuf.Collections;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using Momento.Sdk.Internal.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache set fetch request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheSetFetchResponse.Success</description></item>
/// <item><description>CacheSetFetchResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheSetFetchResponse.Hit hitResponse)
/// {
///     return hitResponse.ValueSetString;
/// }
/// else if (response is CacheSetFetchResponse.Miss missResponse)
/// {
///     // handle miss as appropriate
/// }
/// else if (response is CacheSetFetchResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheSetFetchResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Hit"]/description/*' />
    public class Hit : CacheSetFetchResponse
    {
#pragma warning disable 1591
        protected readonly RepeatedField<ByteString> elements;
        protected readonly Lazy<ISet<byte[]>> _byteArraySet;
        protected readonly Lazy<ISet<string>> _stringSet;
#pragma warning restore 1591

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response">Cache set fetch response.</param>
        public Hit(_SetFetchResponse response)
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
        /// The cached set as a <see cref="ISet{T}" /> of <see cref="byte" /> arrays.
        /// </summary>
        public ISet<byte[]> ValueSetByteArray { get => _byteArraySet.Value; }

        /// <summary>
        /// The cached set as a <see cref="ISet{T}" /> of <see cref="string" />s.
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
    public class Miss : CacheSetFetchResponse
    {

    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheSetFetchResponse, IError
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
