using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Momento.Protos.CacheClient;
using Momento.Sdk.Requests;

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
            if (_logger.IsEnabled(LogLevel.Trace))
            {
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

        /// <summary>
        /// Logs a message at TRACE level that indicates that a request is about to be executed
        /// </summary>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        public static void LogTraceExecutingCollectionRequest(this ILogger _logger, string requestType, string cacheName, string collectionName)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Executing '{}' request: cacheName: {}; collectionName: {}", requestType, cacheName, collectionName);
            }
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a request is about to be executed
        /// </summary>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="field"></param>
        /// <param name="ttl"></param>
        public static void LogTraceExecutingCollectionRequest(this ILogger _logger, string requestType, string cacheName, string collectionName, string field, CollectionTtl? ttl)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Executing '{}' request: cacheName: {}; collectionName: {}; field: {}; ttl: {}", requestType, cacheName, collectionName, field, ttl);
            }
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a request is about to be executed
        /// </summary>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="field"></param>
        /// <param name="ttl"></param>
        public static void LogTraceExecutingCollectionRequest(this ILogger _logger, string requestType, string cacheName, string collectionName, ByteString field, CollectionTtl? ttl)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                var readableField = ReadableByteString(field);
                _logger.LogTrace("Executing '{}' request: cacheName: {}; collectionName: {}; fields: {}; ttl: {}", requestType, cacheName, collectionName, readableField, ttl);
            }
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a request is about to be executed
        /// </summary>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="fields"></param>
        /// <param name="ttl"></param>
        public static void LogTraceExecutingCollectionRequest(this ILogger _logger, string requestType, string cacheName, string collectionName, IEnumerable<ByteString> fields, CollectionTtl? ttl)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                var readableFields = String.Join(", ", fields.Select(k => ReadableByteString(k)));
                _logger.LogTrace("Executing '{}' request: cacheName: {}; collectionName: {}; fields: {}; ttl: {}", requestType, cacheName, collectionName, readableFields, ttl);
            }
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a request is about to be executed
        /// </summary>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="items"></param>
        /// <param name="ttl"></param>
        public static void LogTraceExecutingCollectionRequest(this ILogger _logger, string requestType, string cacheName, string collectionName, IEnumerable<_DictionaryFieldValuePair> items, CollectionTtl? ttl)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                var readableItems = String.Join(", ", items.Select(i => "(" + ReadableByteString(i.Field) + " -> " + ReadableByteString(i.Value) + ")"));
                _logger.LogTrace("Executing '{}' request: cacheName: {}; collectionName: {}; items: {}; ttl: {}", requestType, cacheName, collectionName, readableItems, ttl);
            }
        }


        /// <summary>
        /// Logs a message at TRACE level that indicates that a collection request resulted in an error.
        /// </summary>
        /// <typeparam name="TError"></typeparam>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static TError LogTraceCollectionRequestError<TError>(this ILogger _logger, string requestType, string cacheName, string collectionName, TError error)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("An error occurred while executing a '{}' request: cacheName: {}; collectionName: {}; error: {}", requestType, cacheName, collectionName, error);
            }
            return error;
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a collection request resulted in an error.
        /// </summary>
        /// <typeparam name="TError"></typeparam>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="field"></param>
        /// <param name="ttl"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static TError LogTraceCollectionRequestError<TError>(this ILogger _logger, string requestType, string cacheName, string collectionName, string field, CollectionTtl ttl, TError error)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("An error occurred while executing a '{}' request: cacheName: {}; collectionName: {}; field: {}; ttl: {}; error: {}", requestType, cacheName, collectionName, field, ttl, error);
            }
            return error;
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a collection request resulted in an error.
        /// </summary>
        /// <typeparam name="TError"></typeparam>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="field"></param>
        /// <param name="ttl"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static TError LogTraceCollectionRequestError<TError>(this ILogger _logger, string requestType, string cacheName, string collectionName, ByteString field, CollectionTtl? ttl, TError error)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                var readableField = ReadableByteString(field);
                _logger.LogTrace("An error occurred while executing a '{}' request: cacheName: {}; collectionName: {}; field: {}; ttl: {}; error: {}", requestType, cacheName, collectionName, readableField, ttl, error);
            }
            return error;
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a collection request resulted in an error.
        /// </summary>
        /// <typeparam name="TError"></typeparam>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="fields"></param>
        /// <param name="ttl"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static TError LogTraceCollectionRequestError<TError>(this ILogger _logger, string requestType, string cacheName, string collectionName, IEnumerable<ByteString> fields, CollectionTtl? ttl, TError error)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                var readableFields = String.Join(", ", fields.Select(k => ReadableByteString(k)));
                _logger.LogTrace("An error occurred while executing a '{}' request: cacheName: {}; collectionName: {}; fields: {}; ttl: {}; error: {}", requestType, cacheName, collectionName, readableFields, ttl, error);
            }
            return error;
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a collection request resulted in an error.
        /// </summary>
        /// <typeparam name="TError"></typeparam>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="items"></param>
        /// <param name="ttl"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static TError LogTraceCollectionRequestError<TError>(this ILogger _logger, string requestType, string cacheName, string collectionName, IEnumerable<_DictionaryFieldValuePair> items, CollectionTtl ttl, TError error)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                var readableItems = String.Join(", ", items.Select(i => "(" + ReadableByteString(i.Field) + " -> " + ReadableByteString(i.Value) + ")"));
                _logger.LogTrace("An error occurred while executing a '{}' request: cacheName: {}; collectionName: {}; fields: {}; ttl: {}; error: {}", requestType, cacheName, collectionName, readableItems, ttl, error);
            }
            return error;
        }



        /// <summary>
        /// Logs a message at TRACE level that indicates that a request was successfully executed.
        /// </summary>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static TSuccess LogTraceCollectionRequestSuccess<TSuccess>(this ILogger _logger, string requestType, string cacheName, string collectionName, TSuccess success)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Successfully executed '{}' request: cacheName: {}; collectionName: {}; success: {}", requestType, cacheName, collectionName, success);
            }
            return success;
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a request was successfully executed.
        /// </summary>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="field"></param>
        /// <param name="ttl"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static TSuccess LogTraceCollectionRequestSuccess<TSuccess>(this ILogger _logger, string requestType, string cacheName, string collectionName, string field, CollectionTtl ttl, TSuccess success)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Successfully executed '{}' request: cacheName: {}; collectionName: {}; field: {}; ttl: {}; success: {}", requestType, cacheName, collectionName, field, ttl, success);
            }
            return success;
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a request was successfully executed.
        /// </summary>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="field"></param>
        /// <param name="ttl"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static TSuccess LogTraceCollectionRequestSuccess<TSuccess>(this ILogger _logger, string requestType, string cacheName, string collectionName, ByteString field, CollectionTtl? ttl, TSuccess success)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                var readableField = ReadableByteString(field);
                _logger.LogTrace("Successfully executed '{}' request: cacheName: {}; collectionName: {}; field: {}; ttl: {}; success: {}", requestType, cacheName, collectionName, readableField, ttl, success);
            }
            return success;
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a request was successfully executed.
        /// </summary>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="fields"></param>
        /// <param name="ttl"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static TSuccess LogTraceCollectionRequestSuccess<TSuccess>(this ILogger _logger, string requestType, string cacheName, string collectionName, IEnumerable<ByteString> fields, CollectionTtl? ttl, TSuccess success)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                var readableFields = String.Join(", ", fields.Select(k => ReadableByteString(k)));
                _logger.LogTrace("Successfully executed '{}' request: cacheName: {}; collectionName: {}; fields: {}; ttl: {}; success: {}", requestType, cacheName, collectionName, readableFields, ttl, success);
            }
            return success;
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a request was successfully executed.
        /// </summary>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="_logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="collectionName"></param>
        /// <param name="items"></param>
        /// <param name="ttl"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static TSuccess LogTraceCollectionRequestSuccess<TSuccess>(this ILogger _logger, string requestType, string cacheName, string collectionName, IEnumerable<_DictionaryFieldValuePair> items, CollectionTtl ttl, TSuccess success)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                var readableItems = String.Join(", ", items.Select(i => "(" + ReadableByteString(i.Field) + " -> " + ReadableByteString(i.Value) + ")"));
                _logger.LogTrace("Successfully executed '{}' request: cacheName: {}; collectionName: {}; fields: {}; ttl: {}; success: {}", requestType, cacheName, collectionName, readableItems, ttl, success);
            }
            return success;
        }
        
#if NETSTANDARD2_0_OR_GREATER

        /// <summary>
        /// Logs a message at TRACE level that indicates that a topic request is about to be executed.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="topicName"></param>
        public static void LogTraceExecutingTopicRequest(this ILogger logger, string requestType, string cacheName, string topicName)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Executing '{}' request: cacheName: {}; topicName: {}", requestType, cacheName, topicName);
            }
        }
        
        /// <summary>
        /// Logs a message at TRACE level that indicates that a topic request resulted in an error.
        /// </summary>
        /// <typeparam name="TError"></typeparam>
        /// <param name="logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="topicName"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static TError LogTraceTopicRequestError<TError>(this ILogger logger, string requestType, string cacheName, string topicName, TError error)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("An error occurred while executing a '{}' request: cacheName: {}; topicName: {}; error: {}", requestType, cacheName, topicName, error);
            }
            return error;
        }

        /// <summary>
        /// /// Logs a message at TRACE level that indicates that a topic request resulted in a success.
        /// </summary>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="logger"></param>
        /// <param name="requestType"></param>
        /// <param name="cacheName"></param>
        /// <param name="topicName"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static TSuccess LogTraceTopicRequestSuccess<TSuccess>(this ILogger logger, string requestType, string cacheName, string topicName, TSuccess success)
        {

            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Successfully executed '{}' request: cacheName: {}; topicName: {}; success: {}", requestType, cacheName, topicName, success);
            }
            return success;
        }
        
        /// <summary>
        /// Logs a message at TRACE level that indicates that a topic message was received.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="messageType"></param>
        /// <param name="cacheName"></param>
        /// <param name="topicName"></param>
        public static void LogTraceTopicMessageReceived(this ILogger logger, string messageType, string cacheName, string topicName)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Received '{}' message on: cacheName: {}; topicName: {}", messageType, cacheName, topicName);
            }
        }
        
        /// <summary>
        /// Logs a message at TRACE level that indicates that a discontinuity was received.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cacheName"></param>
        /// <param name="topicName"></param>
        /// <param name="lastSequenceNumber"></param>
        /// <param name="newSequenceNumber"></param>
        public static void LogTraceTopicDiscontinuityReceived(this ILogger logger, string cacheName, string topicName, ulong lastSequenceNumber, ulong newSequenceNumber)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Received discontinuity: cacheName: {}; topicName: {}, lastSequenceNumber: {}, newSequenceNumber: {}", cacheName, topicName, lastSequenceNumber, newSequenceNumber);
            }
        }
        
        /// <summary>
        /// Logs a message at TRACE level that indicates that a topic subscription received an error.
        /// </summary>
        /// <typeparam name="TError"></typeparam>
        /// <param name="logger"></param>
        /// <param name="cacheName"></param>
        /// <param name="topicName"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static TError LogTraceTopicSubscriptionError<TError>(this ILogger logger, string cacheName, string topicName, TError error)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("An error was received by a subscription: cacheName: {}; topicName: {}; error: {}", cacheName, topicName, error);
            }
            return error;
        }
#endif

        /// <summary>
        /// Logs a message at TRACE level that indicates that an auth request is about to be executed.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="requestType"></param>
        public static void LogTraceExecutingAuthRequest(this ILogger logger, string requestType)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Executing '{}' request", requestType);
            }
        }

        /// <summary>
        /// Logs a message at TRACE level that indicates that a topic request resulted in an error.
        /// </summary>
        /// <typeparam name="TError"></typeparam>
        /// <param name="logger"></param>
        /// <param name="requestType"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static TError LogTraceAuthRequestError<TError>(this ILogger logger, string requestType, TError error)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("An error occurred while executing a '{}' request: error: {}", requestType, error);
            }
            return error;
        }

        /// <summary>
        /// /// Logs a message at TRACE level that indicates that a topic request resulted in a success.
        /// </summary>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="logger"></param>
        /// <param name="requestType"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static TSuccess LogTraceAuthRequestSuccess<TSuccess>(this ILogger logger, string requestType, TSuccess success)
        {

            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Successfully executed '{}' request: success: {}", requestType, success);
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

