﻿using Google.Protobuf;
using Google.Protobuf.Collections;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a cache list fetch request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheListFetchResponse.Hit</description></item>
/// <item><description>CacheListFetchResponse.Miss</description></item>
/// <item><description>CacheListFetchResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheListFetchResponse.Hit hitResponse)
/// {
///     return hitResponse.ValueListString;
/// }
/// else if (response is CacheListFetchResponse.Miss missResponse)
/// {
///     // handle miss as appropriate
/// }
/// else if (response is CacheListFetchResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheListFetchResponse
{
    /// <include file="../../docs.xml" path='docs/class[@name="Hit"]/description/*' />
    public class Hit : CacheListFetchResponse
    {
#pragma warning disable 1591
        protected readonly RepeatedField<ByteString> values;
        protected readonly Lazy<IList<byte[]>> _byteArrayList;
        protected readonly Lazy<IList<string>> _stringList;
#pragma warning restore 1591

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response">The cache response.</param>
        public Hit(_ListFetchResponse response)
        {
            values = response.Found.Values;
            _byteArrayList = new(() =>
            {
                return new List<byte[]>(values.Select(v => v.ToByteArray()));
            });

            _stringList = new(() =>
            {
                return new List<string>(values.Select(v => v.ToStringUtf8()));
            });
        }

        /// <summary>
        /// The cached list as a <see cref="IList{T}"/> of <see cref="byte"/> arrays.
        /// </summary>
        public IList<byte[]> ValueListByteArray { get => _byteArrayList.Value; }

        /// <summary>
        /// The cached list as a <see cref="IList{T}"/> of <see cref="string"/>s.
        /// </summary>
        public IList<string> ValueListString { get => _stringList.Value; }

        /// <inheritdoc />
        public override string ToString()
        {
            var stringRepresentation = String.Join(", ", ValueListString.Select(value => $"\"{value}\""));
            var byteArrayRepresentation = String.Join(", ", ValueListByteArray.Select(value => $"\"{value.ToPrettyHexString()}\""));
            return $"{base.ToString()}: ValueListString: [{stringRepresentation.Truncate()}] ValueListByteArray: [{byteArrayRepresentation.Truncate()}]";
        }
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Miss"]/description/*' />
    public class Miss : CacheListFetchResponse
    {

    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheListFetchResponse, IError
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
