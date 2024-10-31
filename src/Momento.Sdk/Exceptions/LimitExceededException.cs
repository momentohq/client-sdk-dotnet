namespace Momento.Sdk.Exceptions;

using System;
using Grpc.Core;

/// <summary>
/// Requested operation couldn't be completed because system limits were hit.
/// </summary>
public class LimitExceededException : SdkException
{
    /// <include file="../docs.xml" path='docs/class[@name="SdkException"]/constructor/*' />
    public LimitExceededException(string message, MomentoErrorTransportDetails transportDetails, RpcException? e = null) : base(MomentoErrorCode.LIMIT_EXCEEDED_ERROR, message, transportDetails, e)
    {
        var errMetadata = e?.Trailers.Get("err")?.Value;
        if (errMetadata != null) {
            this.MessageWrapper = errMetadata switch
            {
                "topic_subscriptions_limit_exceeded" => LimitExceededMessageWrapper.TopicSubscriptionsLimitExceeded.Value,
                "operations_rate_limit_exceeded" => LimitExceededMessageWrapper.OperationsRateLimitExceeded.Value,
                "throughput_rate_limit_exceeded" => LimitExceededMessageWrapper.ThroughputRateLimitExceeded.Value,
                "request_size_limit_exceeded" => LimitExceededMessageWrapper.RequestSizeLimitExceeded.Value,
                "item_size_limit_exceeded" => LimitExceededMessageWrapper.ItemSizeLimitExceeded.Value,
                "element_size_limit_exceeded" => LimitExceededMessageWrapper.ElementSizeLimitExceeded.Value,
                _ => LimitExceededMessageWrapper.UnknownLimitExceeded.Value,
            };
        } else {
            var lowerCasedMessage = message.ToLower();
            this.MessageWrapper = LimitExceededMessageWrapper.UnknownLimitExceeded.Value;
            if (lowerCasedMessage.Contains("subscribers")) {
                this.MessageWrapper = LimitExceededMessageWrapper.TopicSubscriptionsLimitExceeded.Value;
            } else if (lowerCasedMessage.Contains("operations")) {
                this.MessageWrapper = LimitExceededMessageWrapper.OperationsRateLimitExceeded.Value;
            } else if (lowerCasedMessage.Contains("throughput")) {
                this.MessageWrapper = LimitExceededMessageWrapper.ThroughputRateLimitExceeded.Value;
            } else if (lowerCasedMessage.Contains("request limit")) {
                this.MessageWrapper = LimitExceededMessageWrapper.RequestSizeLimitExceeded.Value;
            } else if (lowerCasedMessage.Contains("item size")) {
                this.MessageWrapper = LimitExceededMessageWrapper.ItemSizeLimitExceeded.Value;
            } else if (lowerCasedMessage.Contains("element size")) {
                this.MessageWrapper = LimitExceededMessageWrapper.ElementSizeLimitExceeded.Value;
            } else {
                this.MessageWrapper = LimitExceededMessageWrapper.UnknownLimitExceeded.Value;
            }
        }
    }
}

/// <summary>
/// Provides a specific reason for the limit exceeded error.
/// </summary>
public sealed class LimitExceededMessageWrapper
{
    /// <summary>
    /// Topic subscriptions limit exceeded for this account.
    /// </summary>
    public static readonly LimitExceededMessageWrapper TopicSubscriptionsLimitExceeded = new LimitExceededMessageWrapper("Topic subscriptions limit exceeded for this account");
    /// <summary>
    /// Request rate limit exceeded for this account.
    /// </summary>
    public static readonly LimitExceededMessageWrapper OperationsRateLimitExceeded = new LimitExceededMessageWrapper("Request rate limit exceeded for this account");

    /// <summary>
    /// Bandwidth limit exceeded for this account.
    /// </summary>
    public static readonly LimitExceededMessageWrapper ThroughputRateLimitExceeded = new LimitExceededMessageWrapper("Bandwidth limit exceeded for this account");
    /// <summary>
    /// Request size limit exceeded for this account.
    /// </summary>
    public static readonly LimitExceededMessageWrapper RequestSizeLimitExceeded = new LimitExceededMessageWrapper("Request size limit exceeded for this account");
    /// <summary>
    /// Item size limit exceeded for this account.
    /// </summary>
    public static readonly LimitExceededMessageWrapper ItemSizeLimitExceeded = new LimitExceededMessageWrapper("Item size limit exceeded for this account");
    /// <summary>
    /// Element size limit exceeded for this account.
    /// </summary>
    public static readonly LimitExceededMessageWrapper ElementSizeLimitExceeded = new LimitExceededMessageWrapper("Element size limit exceeded for this account");
    /// <summary>
    /// Unknown limit exceeded for this account.
    /// </summary>
    public static readonly LimitExceededMessageWrapper UnknownLimitExceeded = new LimitExceededMessageWrapper("Limit exceeded for this account");

    private LimitExceededMessageWrapper(string value)
    {
        Value = value;
    }

    /// <summary>
    /// The value of the LimitExceededMessageWrapper.
    /// </summary>
    public string Value { get; private set; }
}