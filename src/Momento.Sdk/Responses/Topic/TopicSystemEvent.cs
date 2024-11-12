namespace Momento.Sdk.Responses;

/// <summary>
/// Represents a system event that can be published to a topic.
/// </summary>
public abstract class TopicSystemEvent : ITopicEvent
{
    /// <summary>
    /// Represents a heartbeat event.
    /// </summary>
    public class Heartbeat : TopicSystemEvent
    {
        /// <summary>
        /// Constructs a new heartbeat event.
        /// </summary>
        public Heartbeat()
        {

        }

        /// <inheritdoc/>
        public override string ToString() => base.ToString() ?? "Heartbeat";
    }

    /// <summary>
    /// Represents a discontinuity event.
    /// </summary>
    public class Discontinuity : TopicSystemEvent
    {
        /// <summary>
        /// Constructs a new discontinuity event.
        /// </summary>
        /// <param name="lastKnownSequenceNumber">The last known sequence number before the discontinuity.</param>
        /// <param name="sequenceNumber">The sequence number of the discontinuity.</param>
        /// <param name="sequencePage">The sequence page of the discontinuity.</param>
        public Discontinuity(ulong lastKnownSequenceNumber, ulong sequenceNumber, ulong sequencePage)
        {
            LastKnownSequenceNumber = lastKnownSequenceNumber;
            SequenceNumber = sequenceNumber;
            SequencePage = sequencePage;
        }

        /// <summary>
        /// The last known sequence number before the discontinuity.
        /// </summary>
        public ulong LastKnownSequenceNumber { get; }

        /// <summary>
        /// The sequence number of the discontinuity.
        /// </summary>
        public ulong SequenceNumber { get; }

        /// <summary>
        /// The sequence page of the discontinuity.
        /// </summary>
        public ulong SequencePage { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}: LastKnownSequenceNumber: {LastKnownSequenceNumber} SequenceNumber: {SequenceNumber} SequencePage: {SequencePage}";
        }
    }
}
