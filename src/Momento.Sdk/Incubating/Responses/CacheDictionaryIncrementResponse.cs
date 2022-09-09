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

    public CacheDictionaryIncrementResponse()
    {
        Status = CacheDictionaryIncrementStatus.OK;
        Value = 42;
    }
}
