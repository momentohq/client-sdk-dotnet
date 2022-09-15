using Momento.Protos.CacheClient;

namespace Momento.Sdk.Incubating.Responses;

public class CacheDictionaryIncrementResponse
{
    public long? Value { get; private set; }

    public CacheDictionaryIncrementResponse(_DictionaryIncrementResponse response)
    {
        Value = response.Value;
    }
}
