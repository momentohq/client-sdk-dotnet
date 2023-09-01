using Momento.Protos.CacheClient.Pubsub;
using Momento.Sdk.Exceptions;

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
/// if (message is TopicMessage.Item item)
/// {
///     return item.ValueString();
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
public abstract class TopicMessage
{
    /// <summary>
    /// A topic message containing a value. If the value is a string, ValueString
    /// will return it. If the value is binary, ValueByteArray will return it.
    /// </summary>
    public class Item : TopicMessage
    {
        private readonly _TopicValue _value;

        /// <summary>
        /// Constructs an Item from an internal _TopicItem
        /// </summary>
        /// <param name="topicItem">Containing the binary or string value.</param>
        public Item(_TopicItem topicItem)
        {
            _value = topicItem.Value;
            TopicSequenceNumber = topicItem.TopicSequenceNumber;
        }
        
        /// <summary>
        /// The number of this message in the topic sequence.
        /// </summary>
        public ulong TopicSequenceNumber { get; }

        /// <summary>
        /// The binary value of this message, if present.
        /// </summary>
        public byte[] ValueByteArray => _value.Binary.ToByteArray();
        
        /// <summary>
        /// The string value of this message, if present.
        /// </summary>
        public string ValueString => _value.Text;
    }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : TopicMessage, IError
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