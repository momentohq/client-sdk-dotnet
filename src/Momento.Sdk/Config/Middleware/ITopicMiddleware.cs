using System;
using System.Collections.Generic;

namespace Momento.Sdk.Config.Middleware;

/// <summary>
/// TODO
/// </summary>
public interface ITopicMiddleware
{
    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    public IList<Tuple<string, string>> WithHeaders();

    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    public void OnStreamDisconnected();

    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    public void onStreamEstablished();
}
