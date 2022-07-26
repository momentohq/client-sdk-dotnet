using System;
using System.Collections.Generic;
using System.Linq;
using CacheClient;
using Google.Protobuf.Collections;
using MomentoSdk.Responses;


namespace MomentoSdk.Incubating.Responses;

public class CacheDictionaryGetMultiResponse
{
    private readonly IEnumerable<CacheDictionaryGetResponse> responses;

    public CacheDictionaryGetMultiResponse(_DictionaryGetResponse responses)
    {
        this.responses = responses.DictionaryBody.Select(response => new CacheDictionaryGetResponse(response));
    }

    public List<CacheGetStatus> Status
    {
        get => responses.Select(response => response.Status).ToList();
    }

    public List<string?> Strings()
    {
        return responses.Select(response => response.String()).ToList();
    }

    public List<byte[]?> Bytes
    {
        get => responses.Select(response => response.Bytes).ToList();
    }
}
