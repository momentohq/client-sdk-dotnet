namespace Momento.Sdk.Exceptions;

using Grpc.Core;

/// <summary>
/// Requested operation couldn't be completed because system limits were hit.
/// </summary>
public class LimitExceededException : SdkException
{
    /// <include file="../docs.xml" path='docs/class[@name="SdkException"]/constructor/*' />
    public LimitExceededException(string messageWrapper, string message, MomentoErrorTransportDetails transportDetails, RpcException? e = null) : base(MomentoErrorCode.LIMIT_EXCEEDED_ERROR, message, transportDetails, e)
    {
        this.MessageWrapper = messageWrapper;
    }

    /// <summary>
    /// Creates a LimitExceededException with a message wrapper that specifies the limit that was exceeded.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="transportDetails"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public static LimitExceededException CreateWithMessageWrapper(string message, MomentoErrorTransportDetails transportDetails, RpcException? e = null)
    {
        var messageWrapper = LimitExceededMessageWrapper.UnknownLimitExceeded;
        var errMetadata = e?.Trailers.Get("err")?.Value;
        if (errMetadata != null)
        {
            messageWrapper = errMetadata switch
            {
                "topic_subscriptions_limit_exceeded" => LimitExceededMessageWrapper.TopicSubscriptionsLimitExceeded,
                "operations_rate_limit_exceeded" => LimitExceededMessageWrapper.OperationsRateLimitExceeded,
                "throughput_rate_limit_exceeded" => LimitExceededMessageWrapper.ThroughputRateLimitExceeded,
                "request_size_limit_exceeded" => LimitExceededMessageWrapper.RequestSizeLimitExceeded,
                "item_size_limit_exceeded" => LimitExceededMessageWrapper.ItemSizeLimitExceeded,
                "element_size_limit_exceeded" => LimitExceededMessageWrapper.ElementSizeLimitExceeded,
                _ => LimitExceededMessageWrapper.UnknownLimitExceeded,
            };
        }
        else
        {
            var lowerCasedMessage = message.ToLower();
            if (lowerCasedMessage.Contains("subscribers"))
            {
                messageWrapper = LimitExceededMessageWrapper.TopicSubscriptionsLimitExceeded;
            }
            else if (lowerCasedMessage.Contains("operations"))
            {
                messageWrapper = LimitExceededMessageWrapper.OperationsRateLimitExceeded;
            }
            else if (lowerCasedMessage.Contains("throughput"))
            {
                messageWrapper = LimitExceededMessageWrapper.ThroughputRateLimitExceeded;
            }
            else if (lowerCasedMessage.Contains("request limit"))
            {
                messageWrapper = LimitExceededMessageWrapper.RequestSizeLimitExceeded;
            }
            else if (lowerCasedMessage.Contains("item size"))
            {
                messageWrapper = LimitExceededMessageWrapper.ItemSizeLimitExceeded;
            }
            else if (lowerCasedMessage.Contains("element size"))
            {
                messageWrapper = LimitExceededMessageWrapper.ElementSizeLimitExceeded;
            }
            else
            {
                messageWrapper = LimitExceededMessageWrapper.UnknownLimitExceeded;
            }
        }

        return new LimitExceededException(messageWrapper, message, transportDetails, e);
    }
}

/// <summary>
/// Provides a specific reason for the limit exceeded error.
/// </summary>
public static class LimitExceededMessageWrapper
{
    /// <summary>
    /// Topic subscriptions limit exceeded for this account.
    /// </summary>
    public static string TopicSubscriptionsLimitExceeded = "Topic subscriptions limit exceeded for this account";
    /// <summary>
    /// Request rate limit exceeded for this account.
    /// </summary>
    public static string OperationsRateLimitExceeded = "Request rate limit exceeded for this account";

    /// <summary>
    /// Bandwidth limit exceeded for this account.
    /// </summary>
    public static string ThroughputRateLimitExceeded = "Bandwidth limit exceeded for this account";
    /// <summary>
    /// Request size limit exceeded for this account.
    /// </summary>
    public static string RequestSizeLimitExceeded = "Request size limit exceeded for this account";
    /// <summary>
    /// Item size limit exceeded for this account.
    /// </summary>
    public static string ItemSizeLimitExceeded = "Item size limit exceeded for this account";
    /// <summary>
    /// Element size limit exceeded for this account.
    /// </summary>
    public static string ElementSizeLimitExceeded = "Element size limit exceeded for this account";
    /// <summary>
    /// Unknown limit exceeded for this account.
    /// </summary>
    public static string UnknownLimitExceeded = "Limit exceeded for this account";
}
