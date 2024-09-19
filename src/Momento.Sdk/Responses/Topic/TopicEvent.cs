namespace Momento.Sdk.Responses;

/// <summary>
/// Represents a system event that can be published to a topic.
/// </summary>
public abstract class TopicEvent : ITopicEvent
{
    /// <summary>
    /// Represents a heartbeat event.
    /// </summary>
    public class Heartbeat : TopicEvent
    {
        /// <summary>
        /// Constructs a new heartbeat event.
        /// </summary>
        public Heartbeat()
        {

        }
    }

    /// <summary>
    /// Represents a discontinuity event.
    /// </summary>
    public class Discontinuity : TopicEvent
    {
        /// <summary>
        /// Constructs a new discontinuity event.
        /// </summary>
        /// <param name="lastKnownSequenceNumber">The last known sequence number before the discontinuity.</param>
        /// <param name="sequenceNumber">The sequence number of the discontinuity.</param>
        public Discontinuity(long lastKnownSequenceNumber, long sequenceNumber)
        {
            LastKnownSequenceNumber = lastKnownSequenceNumber;
            SequenceNumber = sequenceNumber;
        }

        /// <summary>
        /// The last known sequence number before the discontinuity.
        /// </summary>
        public long LastKnownSequenceNumber { get; }
        /// <summary>
        /// The sequence number of the discontinuity.
        /// </summary>
        public long SequenceNumber { get; }
    }
}
