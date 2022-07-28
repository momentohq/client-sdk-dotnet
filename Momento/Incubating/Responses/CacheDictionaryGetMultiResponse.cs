using System;
using System.Collections.Generic;
using System.Linq;
using CacheClient;
using Google.Protobuf.Collections;
using MomentoSdk.Responses;


namespace MomentoSdk.Incubating.Responses;

public class CacheDictionaryGetMultiResponse
{
    public IEnumerable<CacheDictionaryGetResponse> Responses { get; private set; }

    public CacheDictionaryGetMultiResponse(_DictionaryGetResponse responses)
    {
        this.Responses = responses.DictionaryBody.Select(response => new CacheDictionaryGetResponse(response));
    }

    public IEnumerable<CacheGetStatus> Status
    {
        get => Responses.Select(response => response.Status);
    }

    public IEnumerable<string?> Strings()
    {
        return Responses.Select(response => response.String());
    }

    public IEnumerable<byte[]?> Bytes
    {
        get => Responses.Select(response => response.Bytes);
    }
}
