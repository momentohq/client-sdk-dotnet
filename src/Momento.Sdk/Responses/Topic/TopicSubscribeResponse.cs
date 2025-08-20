using Momento.Sdk.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a topic subscribe request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>TopicSubscribeResponse.Subscription</description></item>
/// <item><description>TopicSubscribeResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is TopicSubscribeResponse.Subscription subscription)
/// {
///     await foreach (var item in subscription.WithCancellation(ct))
///     {
///         // iterate through the messages
///     }
/// }
/// else if (response is TopicSubscribeResponse.Error error)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class TopicSubscribeResponse
{
    /// <summary>
    /// A subscription to a Momento topic. As an IAsyncEnumerable, it can be iterated over to read messages from the
    /// topic. The iterator will return a TopicMessage representing a message or error from the stream, or null if the
    /// stream is closed.
    /// </summary>
    public class Subscription : TopicSubscribeResponse, IDisposable, IAsyncEnumerable<TopicMessage?>
    {
        private readonly Func<CancellationToken, ValueTask<ITopicEvent?>> _moveNextFunction;
        private CancellationTokenSource _subscriptionCancellationToken = new();
        private readonly Action _disposalAction;

        /// <summary>
        /// Constructs a Subscription with a wrapped topic iterator and an action to dispose of it.
        /// </summary>
        public Subscription(Func<CancellationToken, ValueTask<ITopicEvent?>> moveNextFunction, Action disposalAction)
        {
            _moveNextFunction = moveNextFunction;
            _disposalAction = disposalAction;
        }

        /// <summary>
        /// Gets the message enumerator for this topic. Includes text and binary messages, but excludes system events.
        /// </summary>
        public IAsyncEnumerable<TopicMessage?> WithCancellation(CancellationToken cancellationToken)
        {
            return new AsyncEnumerableWrapper<TopicMessage?>(GetAsyncEnumerator(cancellationToken));
        }

        /// <summary>
        /// Gets the event enumerator with cancellation for all topic events, including system events.
        /// </summary>
        public IAsyncEnumerable<ITopicEvent?> WithCancellationForAllEvents(CancellationToken cancellationToken)
        {
            return new AsyncEnumerableWrapper<ITopicEvent?>(GetAllEventsAsyncEnumerator(cancellationToken));
        }

        /// <summary>
        /// Gets the message enumerator for this topic. Includes text and binary messages, but excludes system events.
        ///
        /// This subscription represents a single view on a topic, so multiple
        /// enumerators will interfere with each other.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cancel the enumeration.</param>
        /// <returns>An async enumerator for the topic messages.</returns>
        public IAsyncEnumerator<TopicMessage?> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TopicMessageEnumerator(_moveNextFunction, _subscriptionCancellationToken.Token, cancellationToken);
        }

        /// <summary>
        /// Gets an enumerator for all events on this topic, including text and binary messages, and system events.
        ///
        /// This subscription represents a single view on a topic, so multiple
        /// enumerators will interfere with each other.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cancel the enumeration.</param>
        /// <returns>An async enumerator for all events on the topic.</returns>
        IAsyncEnumerator<ITopicEvent?> GetAllEventsAsyncEnumerator(CancellationToken cancellationToken)
        {
            return new AllTopicEventsEnumerator(_moveNextFunction, _subscriptionCancellationToken.Token, cancellationToken);
        }

        /// <summary>
        /// Unsubscribe from this topic.
        /// </summary>
        public void Dispose()
        {
            _subscriptionCancellationToken.Cancel();
            _disposalAction.Invoke();
        }
    }

    private class TopicMessageEnumerator : IAsyncEnumerator<TopicMessage?>
    {
        private readonly Func<CancellationToken, ValueTask<ITopicEvent?>> _moveNextFunction;
        private readonly CancellationToken _subscriptionCancellationToken;
        private readonly CancellationToken _enumeratorCancellationToken;

        public TopicMessageEnumerator(
            Func<CancellationToken, ValueTask<ITopicEvent?>> moveNextFunction,
            CancellationToken subscriptionCancellationToken,
            CancellationToken enumeratorCancellationToken)
        {
            _moveNextFunction = moveNextFunction;
            _subscriptionCancellationToken = subscriptionCancellationToken;
            _enumeratorCancellationToken = enumeratorCancellationToken;
        }

        public TopicMessage? Current { get; private set; }

        public async ValueTask<bool> MoveNextAsync()
        {
            // We iterate over the stream until we get a TopicMessage, an error, or the stream is closed.
            // We skip over system events like heartbeats and discontinuities.
            while (true)
            {
                if (_subscriptionCancellationToken.IsCancellationRequested || _enumeratorCancellationToken.IsCancellationRequested)
                {
                    Current = null;
                    return false;
                }

                var nextEvent = await _moveNextFunction.Invoke(_enumeratorCancellationToken);
                switch (nextEvent)
                {
                    case TopicMessage.Text:
                    case TopicMessage.Binary:
                        Current = (TopicMessage)nextEvent;
                        return true;
                    case TopicMessage.Error:
                        Current = (TopicMessage)nextEvent;
                        return false;
                    // This enumerator excludes system events from the stream
                    case TopicSystemEvent.Discontinuity:
                    case TopicSystemEvent.Heartbeat:
                        continue;
                    default:
                        Current = null;
                        return false;
                }
            }
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }
    }

    private class AllTopicEventsEnumerator : IAsyncEnumerator<ITopicEvent?>
    {
        private readonly Func<CancellationToken, ValueTask<ITopicEvent?>> _moveNextFunction;
        private readonly CancellationToken _subscriptionCancellationToken;
        private readonly CancellationToken _enumeratorCancellationToken;

        public AllTopicEventsEnumerator(
            Func<CancellationToken, ValueTask<ITopicEvent?>> moveNextFunction,
            CancellationToken subscriptionCancellationToken,
            CancellationToken enumeratorCancellationToken)
        {
            _moveNextFunction = moveNextFunction;
            _subscriptionCancellationToken = subscriptionCancellationToken;
            _enumeratorCancellationToken = enumeratorCancellationToken;
        }

        public ITopicEvent? Current { get; private set; }

        public async ValueTask<bool> MoveNextAsync()
        {
            if (_subscriptionCancellationToken.IsCancellationRequested || _enumeratorCancellationToken.IsCancellationRequested)
            {
                Current = null;
                return false;
            }

            var nextEvent = await _moveNextFunction.Invoke(_enumeratorCancellationToken);

            switch (nextEvent)
            {
                case TopicMessage.Text:
                case TopicMessage.Binary:
                case TopicSystemEvent.Discontinuity:
                case TopicSystemEvent.Heartbeat:
                    Current = nextEvent;
                    return true;
                case TopicMessage.Error:
                    Current = nextEvent;
                    return false;
                default:
                    Current = null;
                    return false;
            }
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }
    }

    // Helper class to wrap async enumerators into async enumerable
    // This is necessary to support multiple enumerators on the same subscription.
    private class AsyncEnumerableWrapper<T> : IAsyncEnumerable<T>
    {
        private readonly IAsyncEnumerator<T> _asyncEnumerator;

        public AsyncEnumerableWrapper(IAsyncEnumerator<T> asyncEnumerator)
        {
            _asyncEnumerator = asyncEnumerator;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return _asyncEnumerator;
        }
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : TopicSubscribeResponse, IError
    {
        private readonly SdkException _error;

        /// <include file="../../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(SdkException error)
        {
            _error = error;
        }

        /// <inheritdoc />
        public SdkException InnerException => _error;

        /// <inheritdoc />
        public MomentoErrorCode ErrorCode => _error.ErrorCode;

        /// <inheritdoc />
        public string Message => $"{_error.MessageWrapper}: {_error.Message}";

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: {this.Message}";
        }
    }
}
