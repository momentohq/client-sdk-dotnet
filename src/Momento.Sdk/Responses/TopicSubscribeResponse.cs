#if NETSTANDARD2_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Momento.Sdk.Exceptions;

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
        private readonly Func<CancellationToken, ValueTask<TopicMessage?>> _moveNextFunction;
        private CancellationTokenSource _subscriptionCancellationToken = new();
        private readonly Action _disposalAction;

        /// <summary>
        /// Constructs a Subscription with a wrapped topic iterator and an action to dispose of it.
        /// </summary>
        public Subscription(Func<CancellationToken, ValueTask<TopicMessage?>> moveNextFunction, Action disposalAction)
        {
            _moveNextFunction = moveNextFunction;
            _disposalAction = disposalAction;
        }

        /// <summary>
        /// Gets the enumerator for this topic. This subscription represents a single view on a topic, so multiple
        /// enumerators will interfere with each other.
        /// </summary>
        public IAsyncEnumerator<TopicMessage?> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TopicMessageEnumerator(_moveNextFunction, _subscriptionCancellationToken.Token, cancellationToken);
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
        private readonly Func<CancellationToken, ValueTask<TopicMessage?>> _moveNextFunction;
        private readonly CancellationToken _subscriptionCancellationToken;
        private readonly CancellationToken _enumeratorCancellationToken;

        public TopicMessageEnumerator(
            Func<CancellationToken, ValueTask<TopicMessage?>> moveNextFunction,
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
            if (_subscriptionCancellationToken.IsCancellationRequested || _enumeratorCancellationToken.IsCancellationRequested)
            {
                Current = null;
                return false;
            }
            
            var nextMessage = await _moveNextFunction.Invoke(_enumeratorCancellationToken);
            switch (nextMessage)
            {
                case TopicMessage.Text:
                case TopicMessage.Binary:
                    Current = nextMessage;
                    return true;
                case TopicMessage.Error:
                    Current = nextMessage;
                    return true;
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

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : TopicSubscribeResponse, IError
    {
        private readonly SdkException _error;

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
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
#endif