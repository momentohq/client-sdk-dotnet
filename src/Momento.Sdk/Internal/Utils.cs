using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using Grpc.Net.Client;
using System.Net.Http;
#if USE_GRPC_WEB
using Grpc.Net.Client.Web;
#endif

using Momento.Sdk.Config.Transport;
using Momento.Sdk.Exceptions;

namespace Momento.Sdk.Internal;

/// <summary>
/// Ad-hoc utility methods.
/// </summary>
public static class Utils
{
    /// <summary>
    /// The default value for max_send_message_length is 4mb.  We need to increase this to 5mb in order to support cases where users have requested a limit increase up to our maximum item size of 5mb.
    /// </summary>
    public const int DEFAULT_MAX_MESSAGE_SIZE = 5_243_000;

    /// <summary>
    /// Create a GrpcChannelOptions object from an IGrpcConfiguration object.
    /// </summary>
    /// <param name="grpcConfig">The IGrpcConfiguration object specifying underlying grpc options</param>
    /// <param name="loggerFactory"></param>
    /// <returns></returns>
    public static GrpcChannelOptions GrpcChannelOptionsFromGrpcConfig(IGrpcConfiguration? grpcConfig, ILoggerFactory loggerFactory) {
        var channelOptions = grpcConfig?.GrpcChannelOptions ?? new GrpcChannelOptions();
        channelOptions.Credentials = ChannelCredentials.SecureSsl;
        channelOptions.LoggerFactory ??= loggerFactory;
        // TODO: how to know if user set the value or if max receive size is just set to the default 4mb or null, respectively?
        // Currently just overwriting to 5mb
        channelOptions.MaxReceiveMessageSize = DEFAULT_MAX_MESSAGE_SIZE;
        channelOptions.MaxSendMessageSize = DEFAULT_MAX_MESSAGE_SIZE;
#if NET5_0_OR_GREATER
        channelOptions.HttpHandler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = grpcConfig.SocketsHttpHandlerOptions.EnableMultipleHttp2Connections,
            PooledConnectionIdleTimeout = grpcConfig.SocketsHttpHandlerOptions.PooledConnectionIdleTimeout,
            KeepAlivePingDelay = grpcConfig.SocketsHttpHandlerOptions.KeepAlivePingDelay,
            KeepAlivePingPolicy = grpcConfig.SocketsHttpHandlerOptions.KeepAlivePingPolicy,
            KeepAlivePingTimeout = grpcConfig.SocketsHttpHandlerOptions.KeepAlivePingTimeout,
        };
#elif USE_GRPC_WEB
        channelOptions.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
#endif
        return channelOptions;
    }

    /// <summary>
    /// Convert a UTF-8 encoded string to a byte array.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>The string as a byte array.</returns>
    public static byte[] Utf8ToByteArray(string s) => Encoding.UTF8.GetBytes(s);

    /// <summary>
    /// Throw an exception if the argument is <see langword="null"/>.
    /// </summary>
    /// <param name="argument">The instance to check for <see langword="null"/>.</param>
    /// <param name="paramName">The name of the object to propagate to the exception.</param>
    /// <exception cref="ArgumentNullException"><paramref name="argument"/> is <see langword="null"/>.</exception>
    public static void ArgumentNotNull(object? argument, string paramName)
    {
        if (argument == null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    /// <summary>
    /// Throw an exception if any of the keys or values is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="argument">Enumerable to check for <see langword="null"/> keys/values.</param>
    /// <param name="paramName">Name of the enumerable to propagate to the exception.</param>
    /// <exception cref="ArgumentNullException">Any of <paramref name="argument"/> keys or values is <see langword="null"/>.</exception>
    public static void KeysAndValuesNotNull<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> argument, string paramName)
    {
        if (argument.Any(kv => kv.Key == null || kv.Value == null))
        {
            throw new ArgumentNullException(paramName, "Each key and value must be non-null");
        }
    }

    /// <summary>
    /// Throw an exception if any of the elements of the enumerable is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">Enumerable element type.</typeparam>
    /// <param name="argument">Enumerable to check for <see langword="null"/> elements.</param>
    /// <param name="paramName">Name of the eumerable to propagate to the exception.</param>
    /// <exception cref="ArgumentNullException">Any of <paramref name="argument"/> elements is <see langword="null"/>.</exception>
    public static void ElementsNotNull<T>(IEnumerable<T> argument, string paramName)
    {
        if (argument.Any(value => value == null))
        {
            throw new ArgumentNullException(paramName, "Each value must be non-null");
        }
    }

    /// <summary>
    /// Throw an exception if the time span is zero or negative.
    /// </summary>
    /// <param name="argument">The time span to test.</param>
    /// <param name="paramName">Name of the time span to propagate to the exception.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="argument"/> is zero or negative.</exception>
    public static void ArgumentStrictlyPositive(TimeSpan? argument, string paramName)
    {
        if (argument is null)
        {
            return;
        }

        if (argument <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(paramName, "TimeSpan must be strictly positive.");
        }
    }

    /// <summary>
    /// Throw an exception if the value is zero or negative.
    /// </summary>
    /// <param name="argument">The value to test.</param>
    /// <param name="paramName">Name of the value to propagate to the exception.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="argument"/> is zero or negative.</exception>
    public static void ArgumentStrictlyPositive(int? argument, string paramName)
    {
        if (argument is null)
        {
            return;
        }

        if (argument <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "int must be strictly positive.");
        }
    }
    
    
    /// <summary>
    /// Throw an exception if the value is negative.
    /// </summary>
    /// <param name="argument">The value to test.</param>
    /// <param name="paramName">Name of the value to propagate to the exception.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="argument"/> is negative.</exception>
    public static void ArgumentNonNegative(int? argument, string paramName)
    {
        if (argument is null)
        {
            return;
        }

        if (argument < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "int must be strictly positive.");
        }
    }

    /// <summary>
    /// Throw an exception if the supplied ExpiresIn object is invalid.
    /// </summary>
    /// <param name="expiresIn">The value to test.</param>
    /// <exception cref="InvalidArgumentException"></exception>
    public static void CheckValidDisposableTokenExpiry(ExpiresIn expiresIn)
    {
        if (!expiresIn.DoesExpire())
        {
            throw new InvalidArgumentException("Disposable tokens must have an expiry");
        }
        else if (expiresIn.Seconds() < 0)
        {
            throw new InvalidArgumentException("Disposable token expiry must be positive");
        }
        else if (expiresIn.Seconds() > 60 * 60)
        {
            throw new InvalidArgumentException("Disposable token must expire within 1 hour");
        }
    }

    /// <summary>
    /// Defines methods to support comparing containers of reference items by their
    /// contents (structure) instead of by reference.
    /// </summary>
    public class StructuralEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <inheritdoc />
        public bool Equals(T x, T y)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
        }

        /// <inheritdoc />
        public int GetHashCode(T obj)
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
        }
    }

    /// <summary>
    /// Comprarer to use in byte array containers (Set, Dictionary, List)
    /// so comparisons operate on byte-array content instead of reference.
    /// </summary>
    public static StructuralEqualityComparer<byte[]> ByteArrayComparer = new();

    /// <summary>
    /// Thrown an exception if the end index is larger than the start index.
    /// </summary>
    /// <param name="startIndex">Starting inclusive index of operation.</param>
    /// <param name="endIndex">Ending exclusive index of operation.</param>
    /// <exception cref="Momento.Sdk.Exceptions.InvalidArgumentException"><paramref name="endIndex"/> is less than or equal to <paramref name="startIndex"/></exception>
    public static void ValidateStartEndIndex(int? startIndex, int? endIndex)
    {
        if (startIndex.HasValue && endIndex.HasValue)
        {
            if (startIndex >= 0 && endIndex >= 0 && startIndex >= endIndex)
            {
                throw new Momento.Sdk.Exceptions.InvalidArgumentException("endIndex (exclusive) must be larger than startIndex (inclusive)");
            }
            if (startIndex < 0 && endIndex < 0 && startIndex >= endIndex)
            {
                throw new Momento.Sdk.Exceptions.InvalidArgumentException("endIndex (exclusive) must be larger than startIndex (inclusive)");
            }
            return;
        }
        return;
    }
}
