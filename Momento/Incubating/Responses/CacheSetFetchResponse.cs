using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;
using CacheClient;
using MomentoSdk.Internal;
using MomentoSdk.Responses;

namespace MomentoSdk.Incubating.Responses;

public class CacheSetFetchResponse
{
    public CacheGetStatus Status { get; private set; }
    private readonly RepeatedField<ByteString>? elements;

    public CacheSetFetchResponse(_SetFetchResponse response)
    {
        Status = (response.SetCase == _SetFetchResponse.SetOneofCase.Found) ? CacheGetStatus.HIT : CacheGetStatus.MISS;
        elements = (Status == CacheGetStatus.HIT) ? response.Found.Elements : null;
    }

    public HashSet<byte[]>? ByteArraySet
    {
        get
        {
            if (elements == null)
            {
                return null;
            }
            return new HashSet<byte[]>(
                elements.Select(element => element.ToByteArray()),
                Utils.ByteArrayComparer
            );
        }
    }

    public HashSet<string>? StringSet()
    {
        if (elements == null)
        {
            return null;
        }

        return new HashSet<string>(elements.Select(element => element.ToStringUtf8()));
    }
}
