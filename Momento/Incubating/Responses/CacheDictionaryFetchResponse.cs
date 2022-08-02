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
    private readonly RepeatedField<_DictionaryKeyValuePair>? dictionaryBody;

    public CacheDictionaryFetchResponse(_DictionaryGetAllResponse response)
    {
        Status = CacheGetStatusUtil.From(response.Result);
        dictionaryBody = (Status == CacheGetStatus.HIT) ? response.DictionaryBody : null;
    }

    public Dictionary<byte[], byte[]>? ByteArrayDictionary
    {
        get
        {
            if (dictionaryBody == null)
            {
                return null;
            }
            return new Dictionary<byte[], byte[]>(
                dictionaryBody.Select(
                    kv => new KeyValuePair<byte[], byte[]>(
                        kv.Key.ToByteArray(), kv.Value.ToByteArray())),
                Utils.ByteArrayComparer);
        }
    }

    public Dictionary<string, string>? StringDictionary()
    {
        if (dictionaryBody == null)
        {
            return null;
        }

        return new Dictionary<string, string>(
            dictionaryBody.Select(kv => new KeyValuePair<string, string>(
                kv.Key.ToStringUtf8(), kv.Value.ToStringUtf8()
            ))
        );
    }
}
