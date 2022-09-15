using Momento.Protos.CacheClient;

namespace Momento.Sdk.Incubating.Responses;

public enum CacheDictionaryIncrementStatus
{
    OK,
    PARSE_ERROR
}

public class CacheDictionaryIncrementResponse
{
    public CacheDictionaryIncrementStatus Status { get; private set; }
    public long? Value { get; private set; }

    public CacheDictionaryIncrementResponse(_DictionaryIncrementResponse response)
    {
        Status = CacheDictionaryIncrementStatus.OK;
        Value = response.Value;
    }
}
