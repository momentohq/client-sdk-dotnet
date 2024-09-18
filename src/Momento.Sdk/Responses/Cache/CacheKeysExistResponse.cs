using System;

namespace Momento.Sdk.Responses;

using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;

/// <summary>
/// Parent response type for a cache KeysExist request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>CacheKeysExistResponse.Success</description></item>
/// <item><description>CacheKeysExistResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is CacheKeysExistResponse.Success successResponse)
/// {
///     // handle success as appropriate
/// }
/// else if (response is CacheKeysExistResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class CacheKeysExistResponse
{

    /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : CacheKeysExistResponse
    {
        private readonly List<ByteString> keys;

        /// <summary>
        /// An ordered collection of bools representing whether each key in the original
        /// request exists in the cache or not.  The order is the same as the order that
        /// the keys were specified in.
        /// </summary>
        public IEnumerable<bool> ExistsEnumerable { get; private set; }

        private readonly Lazy<IDictionary<string, bool>> _existsDictionary;
        /// <summary>
        /// A dictionary whose keys are the cache keys from the request, and whose values are
        ///  True if the specified key exists in the cache, false otherwise.
        /// </summary>
        public IDictionary<string, bool> ExistsDictionary { get => _existsDictionary.Value; }

        /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
        public Success(IEnumerable<ByteString> keys, _KeysExistResponse response)
        {
            this.keys = keys.ToList();
            ExistsEnumerable = response.Exists;
            _existsDictionary = new(() =>
            {
                Dictionary<string, bool> result = new();

                var i = 0;
                foreach (var exists in ExistsEnumerable)
                {
                    result[this.keys[i].ToStringUtf8()] = exists;
                    i++;
                }
                return result;
            });
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: Exists: {ExistsEnumerable}";
        }
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : CacheKeysExistResponse, IError
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
