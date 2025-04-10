using System;
using System.Collections.Generic;

namespace Momento.Sdk.Config.Middleware;

/// <summary>
/// Interface for TopicClient middleware.
/// </summary>
public interface ITopicMiddleware
{
    /// <summary>
    /// Add arbitrary headers to each request.
    /// </summary>
    /// <returns></returns>
    public IList<Tuple<string, string>> WithHeaders();

    /// <summary>
    /// Take action when a subscription stream is disconnected.
    /// </summary>
    /// <returns></returns>
    public void OnStreamDisconnected();

    /// <summary>
    /// Take action when a subscription stream is (re)established.
    /// </summary>
    /// <returns></returns>
    public void onStreamEstablished();
}
