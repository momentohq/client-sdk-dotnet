#pragma warning disable 1591

namespace Momento.Sdk.Tests.Integration.Retry;

public enum MomentoRpcMethod
{
    Get,
    Set,
    Delete,
    Increment,
    SetIf,
    SetIfNotExists,
    GetBatch,
    SetBatch,
    KeysExist,
    UpdateTtl,
    ItemGetTtl,
    ItemGetType,
    DictionaryGet,
    DictionarySet,
    DictionaryIncrement,
    DictionaryDelete,
    DictionaryLength,
    SetFetch,
    SetSample,
    SetUnion,
    SetDifference,
    SetContains,
    SetLength,
    SetPop,
    ListPushFront,
    ListPushBack,
    ListPopFront,
    ListPopBack,
    ListErase,
    ListRemove,
    ListFetch,
    ListLength,
    ListConcatenateFront,
    ListConcatenateBack,
    ListRetain,
    SortedSetPut,
    SortedSetFetch,
    SortedSetGetScore,
    SortedSetRemove,
    SortedSetIncrement,
    SortedSetGetRank,
    SortedSetLength,
    SortedSetLengthByScore,
    TopicPublish,
    TopicSubscribe
}

/// <summary>
/// Extension methods for the MomentoRpcMethod enum.
/// </summary>
public static class MomentoRpcMethodExtensions
{
    /// <summary>
    /// Converts the rpc method to a string value.
    /// </summary>
    /// <param name="rpcMethod">to convert to a string</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">if given an unknown rpc method</exception>
    public static string ToStringValue(this MomentoRpcMethod rpcMethod)
    {
        return rpcMethod switch
        {
            MomentoRpcMethod.Get => "_GetRequest",
            MomentoRpcMethod.Set => "_SetRequest",
            MomentoRpcMethod.Delete => "_DeleteRequest",
            MomentoRpcMethod.Increment => "_IncrementRequest",
            MomentoRpcMethod.SetIf => "_SetIfRequest",
            MomentoRpcMethod.SetIfNotExists => "_SetIfNotExistsRequest",
            MomentoRpcMethod.GetBatch => "_GetBatchRequest",
            MomentoRpcMethod.SetBatch => "_SetBatchRequest",
            MomentoRpcMethod.KeysExist => "_KeysExistRequest",
            MomentoRpcMethod.UpdateTtl => "_UpdateTtlRequest",
            MomentoRpcMethod.ItemGetTtl => "_ItemGetTtlRequest",
            MomentoRpcMethod.ItemGetType => "_ItemGetTypeRequest",
            MomentoRpcMethod.DictionaryGet => "_DictionaryGetRequest",
            MomentoRpcMethod.DictionarySet => "_DictionarySetRequest",
            MomentoRpcMethod.DictionaryIncrement => "_DictionaryIncrementRequest",
            MomentoRpcMethod.DictionaryDelete => "_DictionaryDeleteRequest",
            MomentoRpcMethod.DictionaryLength => "_DictionaryLengthRequest",
            MomentoRpcMethod.SetFetch => "_SetFetchRequest",
            MomentoRpcMethod.SetSample => "_SetSampleRequest",
            MomentoRpcMethod.SetUnion => "_SetUnionRequest",
            MomentoRpcMethod.SetDifference => "_SetDifferenceRequest",
            MomentoRpcMethod.SetContains => "_SetContainsRequest",
            MomentoRpcMethod.SetLength => "_SetLengthRequest",
            MomentoRpcMethod.SetPop => "_SetPopRequest",
            MomentoRpcMethod.ListPushFront => "_ListPushFrontRequest",
            MomentoRpcMethod.ListPushBack => "_ListPushBackRequest",
            MomentoRpcMethod.ListPopFront => "_ListPopFrontRequest",
            MomentoRpcMethod.ListPopBack => "_ListPopBackRequest",
            MomentoRpcMethod.ListRemove => "_ListRemoveRequest",
            MomentoRpcMethod.ListFetch => "_ListFetchRequest",
            MomentoRpcMethod.ListLength => "_ListLengthRequest",
            MomentoRpcMethod.ListConcatenateFront => "_ListConcatenateFrontRequest",
            MomentoRpcMethod.ListConcatenateBack => "_ListConcatenateBackRequest",
            MomentoRpcMethod.ListRetain => "_ListRetainRequest",
            MomentoRpcMethod.SortedSetPut => "_SortedSetPutRequest",
            MomentoRpcMethod.SortedSetFetch => "_SortedSetFetchRequest",
            MomentoRpcMethod.SortedSetGetScore => "_SortedSetGetScoreRequest",
            MomentoRpcMethod.SortedSetRemove => "_SortedSetRemoveRequest",
            MomentoRpcMethod.SortedSetIncrement => "_SortedSetIncrementRequest",
            MomentoRpcMethod.SortedSetGetRank => "_SortedSetGetRankRequest",
            MomentoRpcMethod.SortedSetLength => "_SortedSetLengthRequest",
            MomentoRpcMethod.SortedSetLengthByScore => "_SortedSetLengthByScoreRequest",
            MomentoRpcMethod.TopicPublish => "_PublishRequest",
            MomentoRpcMethod.TopicSubscribe => "_SubscribeRequest",
            _ => throw new ArgumentOutOfRangeException(nameof(rpcMethod), "Unknown rpc method to convert to string")
        };
    }

    /// <summary>
    /// Converts a string value to a MomentoRpcMethod.
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static MomentoRpcMethod FromString(string methodName)
    {
        return methodName switch
        {
            "_GetRequest" => MomentoRpcMethod.Get,
            "_SetRequest" => MomentoRpcMethod.Set,
            "_DeleteRequest" => MomentoRpcMethod.Delete,
            "_IncrementRequest" => MomentoRpcMethod.Increment,
            "_SetIfRequest" => MomentoRpcMethod.SetIf,
            "_SetIfNotExistsRequest" => MomentoRpcMethod.SetIfNotExists,
            "_GetBatchRequest" => MomentoRpcMethod.GetBatch,
            "_SetBatchRequest" => MomentoRpcMethod.SetBatch,
            "_KeysExistRequest" => MomentoRpcMethod.KeysExist,
            "_UpdateTtlRequest" => MomentoRpcMethod.UpdateTtl,
            "_ItemGetTtlRequest" => MomentoRpcMethod.ItemGetTtl,
            "_ItemGetTypeRequest" => MomentoRpcMethod.ItemGetType,
            "_DictionaryGetRequest" => MomentoRpcMethod.DictionaryGet,
            "_DictionarySetRequest" => MomentoRpcMethod.DictionarySet,
            "_DictionaryIncrementRequest" => MomentoRpcMethod.DictionaryIncrement,
            "_DictionaryDeleteRequest" => MomentoRpcMethod.DictionaryDelete,
            "_DictionaryLengthRequest" => MomentoRpcMethod.DictionaryLength,
            "_SetFetchRequest" => MomentoRpcMethod.SetFetch,
            "_SetSampleRequest" => MomentoRpcMethod.SetSample,
            "_SetUnionRequest" => MomentoRpcMethod.SetUnion,
            "_SetDifferenceRequest" => MomentoRpcMethod.SetDifference,
            "_SetContainsRequest" => MomentoRpcMethod.SetContains,
            "_SetLengthRequest" => MomentoRpcMethod.SetLength,
            "_SetPopRequest" => MomentoRpcMethod.SetPop,
            "_ListPushFrontRequest" => MomentoRpcMethod.ListPushFront,
            "_ListPushBackRequest" => MomentoRpcMethod.ListPushBack,
            "_ListPopFrontRequest" => MomentoRpcMethod.ListPopFront,
            "_ListPopBackRequest" => MomentoRpcMethod.ListPopBack,
            "_ListRemoveRequest" => MomentoRpcMethod.ListRemove,
            "_ListFetchRequest" => MomentoRpcMethod.ListFetch,
            "_ListLengthRequest" => MomentoRpcMethod.ListLength,
            "_ListConcatenateFrontRequest" => MomentoRpcMethod.ListConcatenateFront,
            "_ListConcatenateBackRequest" => MomentoRpcMethod.ListConcatenateBack,
            "_ListRetainRequest" => MomentoRpcMethod.ListRetain,
            "_SortedSetPutRequest" => MomentoRpcMethod.SortedSetPut,
            "_SortedSetFetchRequest" => MomentoRpcMethod.SortedSetFetch,
            "_SortedSetGetScoreRequest" => MomentoRpcMethod.SortedSetGetScore,
            "_SortedSetRemoveRequest" => MomentoRpcMethod.SortedSetRemove,
            "_SortedSetIncrementRequest" => MomentoRpcMethod.SortedSetIncrement,
            "_SortedSetGetRankRequest" => MomentoRpcMethod.SortedSetGetRank,
            "_SortedSetLengthRequest" => MomentoRpcMethod.SortedSetLength,
            "_SortedSetLengthByScoreRequest" => MomentoRpcMethod.SortedSetLengthByScore,
            "_PublishRequest" => MomentoRpcMethod.TopicPublish,
            "_SubscribeRequest" => MomentoRpcMethod.TopicSubscribe,
            _ => throw new ArgumentOutOfRangeException(nameof(methodName), "Unknown rpc method to convert from string")
        };
    }

    /// <summary>
    /// Converts the rpc method to a string value recognized by momento-local.
    /// </summary>
    /// <param name="rpcMethod">to convert to a string</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">if given an unknown rpc method</exception>
    public static string ToMomentoLocalMetadataString(this MomentoRpcMethod rpcMethod)
    {
        return rpcMethod switch
        {
            MomentoRpcMethod.Get => "get",
            MomentoRpcMethod.Set => "set",
            MomentoRpcMethod.Delete => "delete",
            MomentoRpcMethod.Increment => "increment",
            MomentoRpcMethod.SetIf => "set-if",
            MomentoRpcMethod.SetIfNotExists => "set-if",
            MomentoRpcMethod.GetBatch => "get-batch",
            MomentoRpcMethod.SetBatch => "set-batch",
            MomentoRpcMethod.KeysExist => "keys-exist",
            MomentoRpcMethod.UpdateTtl => "update-ttl",
            MomentoRpcMethod.ItemGetTtl => "item-get-ttl",
            MomentoRpcMethod.ItemGetType => "item-get-type",
            MomentoRpcMethod.DictionaryGet => "dictionary-get",
            MomentoRpcMethod.DictionarySet => "dictionary-set",
            MomentoRpcMethod.DictionaryIncrement => "dictionary-increment",
            MomentoRpcMethod.DictionaryDelete => "dictionary-delete",
            MomentoRpcMethod.DictionaryLength => "dictionary-length",
            MomentoRpcMethod.SetFetch => "set-fetch",
            MomentoRpcMethod.SetSample => "set-sample",
            MomentoRpcMethod.SetUnion => "set-union",
            MomentoRpcMethod.SetDifference => "set-difference",
            MomentoRpcMethod.SetContains => "set-contains",
            MomentoRpcMethod.SetLength => "set-length",
            MomentoRpcMethod.SetPop => "set-pop",
            MomentoRpcMethod.ListPushFront => "list-push-front",
            MomentoRpcMethod.ListPushBack => "list-push-back",
            MomentoRpcMethod.ListPopFront => "list-pop-front",
            MomentoRpcMethod.ListPopBack => "list-pop-back",
            MomentoRpcMethod.ListRemove => "list-remove",
            MomentoRpcMethod.ListFetch => "list-fetch",
            MomentoRpcMethod.ListLength => "list-length",
            MomentoRpcMethod.ListConcatenateFront => "list-concatenate-front",
            MomentoRpcMethod.ListConcatenateBack => "list-concatenate-back",
            MomentoRpcMethod.ListRetain => "list-retain",
            MomentoRpcMethod.SortedSetPut => "sorted-set-put",
            MomentoRpcMethod.SortedSetFetch => "sorted-set-fetch",
            MomentoRpcMethod.SortedSetGetScore => "sorted-set-get-score",
            MomentoRpcMethod.SortedSetRemove => "sorted-set-remove",
            MomentoRpcMethod.SortedSetIncrement => "sorted-set-increment",
            MomentoRpcMethod.SortedSetGetRank => "sorted-set-get-rank",
            MomentoRpcMethod.SortedSetLength => "sorted-set-length",
            MomentoRpcMethod.SortedSetLengthByScore => "sorted-set-length-by-score",
            MomentoRpcMethod.TopicPublish => "topic-publish",
            MomentoRpcMethod.TopicSubscribe => "topic-subscribe",
            _ => throw new ArgumentOutOfRangeException(nameof(rpcMethod), "Unknown rpc method to convert to string")
        };
    }
}
