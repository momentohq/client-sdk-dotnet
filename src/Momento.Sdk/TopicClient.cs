using System;
using System.Threading.Tasks;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;
using Momento.Sdk.Responses;

namespace Momento.Sdk;

/// <summary>
/// Client to perform operations against Momento topics.
/// </summary>
public class TopicClient : ITopicClient
{
    private readonly ScsTopicClient scsTopicClient;


    /// <summary>
    /// Client to perform operations against Momento topics.
    /// </summary>
    /// <param name="config">Configuration to use for the client.</param>
    /// <param name="authProvider">Momento auth provider.</param>
    public TopicClient(ITopicConfiguration config, ICredentialProvider authProvider)
    {
        scsTopicClient = new ScsTopicClient(config, authProvider.AuthToken, authProvider.CacheEndpoint);
    }

    /// <inheritdoc />
    public async Task<TopicPublishResponse> PublishAsync(string cacheName, string topicName, byte[] value)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(topicName, nameof(topicName));
            Utils.ArgumentNotNull(value, nameof(value));
        }
        catch (Exception e)
        {
            return new TopicPublishResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await scsTopicClient.Publish(cacheName, topicName, value);
    }

    /// <inheritdoc />
    public async Task<TopicPublishResponse> PublishAsync(string cacheName, string topicName, string value)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(topicName, nameof(topicName));
            Utils.ArgumentNotNull(value, nameof(value));
        }
        catch (Exception e)
        {
            return new TopicPublishResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await scsTopicClient.Publish(cacheName, topicName, value);
    }

    /// <inheritdoc />
    public async Task<TopicSubscribeResponse> SubscribeAsync(string cacheName, string topicName, ulong? resumeAtSequenceNumber = null, ulong? resumeAtSequencePage = null)
    {
        try
        {
            Utils.ArgumentNotNull(cacheName, nameof(cacheName));
            Utils.ArgumentNotNull(topicName, nameof(topicName));
        }
        catch (Exception e)
        {
            return new TopicSubscribeResponse.Error(new InvalidArgumentException(e.Message));
        }
        return await scsTopicClient.Subscribe(cacheName, topicName, resumeAtSequenceNumber, resumeAtSequencePage);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        scsTopicClient.Dispose();
    }
}
