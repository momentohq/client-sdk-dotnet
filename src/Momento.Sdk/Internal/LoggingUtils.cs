using System;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Momento.Sdk.Internal
{
    /// <summary>
    /// Utils for logging debug information about Momento requests and responses
    /// </summary>
    public static class LoggingUtils
    {
        /// <summary>
        /// Logs a message at TRACE level that indicates that a request is about to be executed.
        /// </summary>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttl"></param>
        public static void LogTraceExecutingRequest(this ILogger _logger, string requestType, string cacheName, ByteString key, ByteString? value, TimeSpan? ttl)
        {
            if (_logger.IsEnabled(LogLevel.Trace)) {
                var readableKey = ReadableByteString(key);
                var readableValue = ReadableByteString(value);
                _logger.LogTrace("Executing '{}' request: cacheName: {}; key: {}; value: {}; ttl: {}", requestType, cacheName, readableKey, readableValue, ttl);
            }
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a request resulted in an error.
        /// </summary>
        /// <typeparam name="TError"></typeparam>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttl"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static TError LogTraceRequestError<TError>(this ILogger _logger, string requestType, string cacheName, ByteString key, ByteString? value, TimeSpan? ttl, TError error)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                var readableKey = ReadableByteString(key);
                var readableValue = ReadableByteString(value);
                _logger.LogTrace("An error occurred while executing a '{}' request: cacheName: {}; key: {}; value: {}; ttl: {}; error: {}", requestType, cacheName, readableKey, readableValue, ttl, error);
            }
            return error;
        }

        /// <summary>
        /// /// Logs a message at TRACE level that indicates that a request resulted in a success.
        /// </summary>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttl"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static TSuccess LogTraceRequestSuccess<TSuccess>(this ILogger _logger, string requestType, string cacheName, ByteString key, ByteString? value, TimeSpan? ttl, TSuccess success)
        {

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                var readableKey = ReadableByteString(key);
                var readableValue = ReadableByteString(value);
                _logger.LogTrace("Successfully executed '{}' request: cacheName: {}; key: {}; value: {}; ttl: {}; success: {}", requestType, cacheName, readableKey, readableValue, ttl, success);
            }
            return success;
        }

        private static string ReadableByteString(ByteString? input)
        {
            if (input == null)
            {
                return "(null)";
            }
            var readable = input.ToStringUtf8();
            if (readable.Length <= 20)
            {
                return readable;
            }
            return readable.Substring(0, 20) + "...";
        }
    }
}

