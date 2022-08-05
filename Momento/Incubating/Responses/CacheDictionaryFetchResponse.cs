using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using CacheClient;
using MomentoSdk.Responses;
using MomentoSdk.Internal;

namespace MomentoSdk.Incubating.Responses;

public class CacheDictionaryFetchResponse
{
    public CacheGetStatus Status { get; private set; }
    private readonly RepeatedField<_DictionaryFieldValuePair>? items;
    private readonly Lazy<Dictionary<byte[], byte[]>?> _byteArrayDictionary;
    private readonly Lazy<Dictionary<string, string>?> _stringDictionary;

    public CacheDictionaryFetchResponse(_DictionaryFetchResponse response)
    {
        Status = (response.DictionaryCase == _DictionaryFetchResponse.DictionaryOneofCase.Found) ? CacheGetStatus.HIT : CacheGetStatus.MISS;
        items = (Status == CacheGetStatus.HIT) ? response.Found.Items : null;

        _byteArrayDictionary = new(() =>
        {
            if (items == null)
            {
                return null;
            }
            return new Dictionary<byte[], byte[]>(
                items.Select(kv => new KeyValuePair<byte[], byte[]>(kv.Field.ToByteArray(), kv.Value.ToByteArray())),
                Utils.ByteArrayComparer);
        });

        _stringDictionary = new(() =>
        {
            if (items == null)
            {
                return null;
            }
            return new Dictionary<string, string>(
                items.Select(kv => new KeyValuePair<string, string>(kv.Field.ToStringUtf8(), kv.Value.ToStringUtf8())));
        });
    }

    public Dictionary<byte[], byte[]>? ByteArrayDictionary { get => _byteArrayDictionary.Value; }

    public Dictionary<string, string>? StringDictionary() => _stringDictionary.Value;
}
