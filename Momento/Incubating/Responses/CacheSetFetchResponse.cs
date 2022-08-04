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
    private HashSet<byte[]>? _byteArraySet = null;
    private HashSet<string>? _stringSet = null;

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
            if (_byteArraySet != null)
            {
                return _byteArraySet;
            }
            _byteArraySet = new HashSet<byte[]>(
                elements.Select(element => element.ToByteArray()),
                Utils.ByteArrayComparer
            );
            return _byteArraySet;
        }
    }

    public HashSet<string>? StringSet()
    {
        if (elements == null)
        {
            return null;
        }
        if (_stringSet != null)
        {
            return _stringSet;
        }

        _stringSet = new HashSet<string>(elements.Select(element => element.ToStringUtf8()));
        return _stringSet;
    }
}
