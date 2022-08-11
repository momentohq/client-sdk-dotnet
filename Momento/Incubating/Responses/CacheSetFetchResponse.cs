using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Momento.Protos.CacheClient;
using MomentoSdk.Internal;
using MomentoSdk.Responses;

namespace MomentoSdk.Incubating.Responses;

public class CacheSetFetchResponse
{
    public CacheGetStatus Status { get; private set; }
    private readonly RepeatedField<ByteString>? elements;
    private readonly Lazy<HashSet<byte[]>?> _byteArraySet;
    private readonly Lazy<HashSet<string>?> _stringSet;

    public CacheSetFetchResponse(_SetFetchResponse response)
    {
        Status = (response.SetCase == _SetFetchResponse.SetOneofCase.Found) ? CacheGetStatus.HIT : CacheGetStatus.MISS;
        elements = (Status == CacheGetStatus.HIT) ? response.Found.Elements : null;

        _byteArraySet = new(() =>
        {
            if (elements == null)
            {
                return null;
            }
            return new HashSet<byte[]>(
                elements.Select(element => element.ToByteArray()),
                Utils.ByteArrayComparer);
        });

        _stringSet = new(() =>
        {
            if (elements == null)
            {
                return null;
            }
            return new HashSet<string>(elements.Select(element => element.ToStringUtf8()));
        });
    }

    public HashSet<byte[]>? ByteArraySet { get => _byteArraySet.Value; }

    public HashSet<string>? StringSet() => _stringSet.Value;
}
