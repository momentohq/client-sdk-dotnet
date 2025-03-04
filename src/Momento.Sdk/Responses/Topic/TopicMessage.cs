using Momento.Protos.CacheClient.Pubsub;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.ExtensionMethods;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent type for a topic message. The message is resolved to a type-safe
/// object of one of the following subtypes:
/// <list type="bullet">
/// <item><description>TopicMessage.Item</description></item>
/// <item><description>TopicMessage.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (message is TopicMessage.Text text)
/// {
///     return text.Value();
/// }
/// else if (message is TopicMessage.Binary binary)
/// {
///     return text.Value();
/// }
/// else if (message is TopicMessage.Error error)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class TopicMessage : ITopicEvent
{
    /// <summary>
    /// A topic message containing a text value.
    /// </summary>
    public class Text : TopicMessage
    {
        /// <summary>
        /// A topic message containing a text value.
        /// </summary>
        public Text(_TopicValue topicValue, ulong topicSequenceNumber, ulong topicSequencePage, string? tokenId = null)
        {
            Value = topicValue.Text;
            TopicSequenceNumber = topicSequenceNumber;
            TopicSequencePage = topicSequencePage;
            TokenId = tokenId;
        }

        /// <summary>
        /// The text value of this message.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// The sequence number of this message.
        /// </summary>
        public ulong TopicSequenceNumber { get; }

        /// <summary>
        /// The sequence page of this message.
        /// </summary>
        public ulong TopicSequencePage { get; }

        /// <summary>
        /// The TokenId that was used to publish the message, or null if the token did not have an id.
        /// This can be used to securely identify the sender of a message.
        /// </summary>
        public string? TokenId { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: Value: \"{this.Value.Truncate()}\" SequenceNumber: {this.TopicSequenceNumber} SequencePage: {this.TopicSequencePage} TokenId: \"{this.TokenId}\"";
        }
    }

    /// <summary>
    /// A topic message containing a binary value.
    /// </summary>
    public class Binary : TopicMessage
    {
        /// <summary>
        /// A topic message containing a binary value.
        /// </summary>
        public Binary(_TopicValue topicValue, ulong topicSequenceNumber, ulong topicSequencePage, string? tokenId = null)
        {
            Value = topicValue.Binary.ToByteArray();
            TopicSequenceNumber = topicSequenceNumber;
            TopicSequencePage = topicSequencePage;
            TokenId = tokenId;
        }


        /// <summary>
        /// The binary value of this message.
        /// </summary>
        public byte[] Value { get; }

        /// <summary>
        /// The sequence number of this message.
        /// </summary>
        public ulong TopicSequenceNumber { get; }

        /// <summary>
        /// The sequence page of this message.
        /// </summary>
        public ulong TopicSequencePage { get; }

        /// <summary>
        /// The TokenId that was used to publish the message, or null if the token did not have an id.
        /// This can be used to securely identify the sender of a message.
        /// </summary>
        public string? TokenId { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: Value: \"{Value.ToPrettyHexString().Truncate()}\" SequenceNumber: {this.TopicSequenceNumber} SequencePage: {this.TopicSequencePage} TokenId: \"{this.TokenId}\"";
        }
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : TopicMessage, IError
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
