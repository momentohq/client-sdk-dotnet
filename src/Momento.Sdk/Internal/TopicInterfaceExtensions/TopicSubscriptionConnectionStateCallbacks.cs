namespace Momento.Sdk.Internal;

/// <summary>
/// Interface for defining callbacks to be invoked when the subscription stream is disconnected or re-established.
/// This is currently used by the MomentoLocalTestTopicConfiguration class, which is an internal wrapper used for
/// testing topics retry logic against momento-local.
/// </summary>
public interface ITopicSubscriptionConnectionStateCallbacks
{
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
