using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Momento.Protos.CacheClient;
using Momento.Sdk.Internal;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Incubating.Responses;

public class CacheDictionaryFetchResponse
{
    public CacheGetStatus Status { get; private set; }
    private readonly RepeatedField<_DictionaryFieldValuePair>? items;
    private readonly Lazy<Dictionary<byte[], byte[]>?> _byteArrayByteArrayDictionary;
    private readonly Lazy<Dictionary<string, string>?> _stringStringDictionary;

    public CacheDictionaryFetchResponse(_DictionaryFetchResponse response)
    {
        Status = (response.DictionaryCase == _DictionaryFetchResponse.DictionaryOneofCase.Found) ? CacheGetStatus.HIT : CacheGetStatus.MISS;
        items = (Status == CacheGetStatus.HIT) ? response.Found.Items : null;

        _byteArrayByteArrayDictionary = new(() =>
        {
            if (items == null)
            {
                return null;
            }
            return new Dictionary<byte[], byte[]>(
                items.Select(kv => new KeyValuePair<byte[], byte[]>(kv.Field.ToByteArray(), kv.Value.ToByteArray())),
                Utils.ByteArrayComparer);
        });

        _stringStringDictionary = new(() =>
        {
            if (items == null)
            {
                return null;
            }
            return new Dictionary<string, string>(
                items.Select(kv => new KeyValuePair<string, string>(kv.Field.ToStringUtf8(), kv.Value.ToStringUtf8())));
        });
    }

    public Dictionary<byte[], byte[]>? ByteArrayByteArrayDictionary { get => _byteArrayByteArrayDictionary.Value; }

    public Dictionary<string, string>? StringStringDictionary() => _stringStringDictionary.Value;
}
