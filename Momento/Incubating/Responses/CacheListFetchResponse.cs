using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Momento.Protos.CacheClient;
using Momento.Sdk.Responses;

namespace Momento.Sdk.Incubating.Responses;

public class CacheListFetchResponse
{
    public CacheGetStatus Status { get; private set; }
    private readonly RepeatedField<ByteString>? values;
    private readonly Lazy<List<byte[]>?> _byteArrayList;
    private readonly Lazy<List<string>?> _stringList;

    public CacheListFetchResponse(_ListFetchResponse response)
    {
        Status = (response.ListCase == _ListFetchResponse.ListOneofCase.Found) ? CacheGetStatus.HIT : CacheGetStatus.MISS;
        values = (Status == CacheGetStatus.HIT) ? response.Found.Values : null;

        _byteArrayList = new(() =>
        {
            if (values == null)
            {
                return null;
            }

            return new List<byte[]>(values.Select(v => v.ToByteArray()));
        });

        _stringList = new(() =>
        {
            if (values == null)
            {
                return null;
            }
            return new List<string>(values.Select(v => v.ToStringUtf8()));
        });
    }

    public List<byte[]>? ByteArrayList { get => _byteArrayList.Value; }

    public List<string>? StringList() => _stringList.Value;
}
