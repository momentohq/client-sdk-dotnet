using Momento.Sdk.Responses;
using System;
using System.Threading.Tasks;

namespace Momento.Sdk;

/// <summary>
/// Minimum viable functionality of a topic client.
/// </summary>
public interface ITopicClient : IDisposable
{
    /// <summary>
    /// Publish a value to a topic in a cache.
    /// </summary>
    /// <param name="cacheName">Name of the cache containing the topic.</param>
    /// <param name="topicName">Name of the topic.</param>
    /// <param name="value">The value to be published.</param>
    /// <returns>
    /// Task object representing the result of the publish operation. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>TopicPublishResponse.Success</description></item>
    /// <item><description>TopicPublishResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is TopicPublishResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    /// </returns>
    public Task<TopicPublishResponse> PublishAsync(string cacheName, string topicName, byte[] value);

    /// <inheritdoc cref="PublishAsync(string, string, byte[])"/>
    public Task<TopicPublishResponse> PublishAsync(string cacheName, string topicName, string value);

    /// <summary>
    /// Subscribe to a topic. The returned value can be used to iterate over newly published messages on the topic.
    /// </summary>
    /// <param name="cacheName">Name of the cache containing the topic.</param>
    /// <param name="topicName">Name of the topic.</param>
    /// <param name="resumeAtSequenceNumber">The sequence number of the last message.</param>
    /// <param name="resumeAtSequencePage">The sequence page of the last message.</param>
    /// If provided, the client will attempt to start the stream from that sequence number.</param>
    /// <returns>
    /// Task object representing the result of the subscribe operation. The
    /// response object is resolved to a type-safe object of one of
    /// the following subtypes:
    /// <list type="bullet">
    /// <item><description>TopicSubscribeResponse.Subscription</description></item>
    /// <item><description>TopicSubscribeResponse.Error</description></item>
    /// </list>
    /// Pattern matching can be used to operate on the appropriate subtype.
    /// For example:
    /// <code>
    /// if (response is TopicSubscribeResponse.Error errorResponse)
    /// {
    ///     // handle error as appropriate
    /// }
    /// </code>
    /// </returns>
    public Task<TopicSubscribeResponse> SubscribeAsync(string cacheName, string topicName, ulong? resumeAtSequenceNumber = null, ulong? resumeAtSequencePage = null);
}
