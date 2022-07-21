using System.Collections.Generic;

namespace MomentoSdk.Responses;

public class CacheSetMultiResponse
{
    private readonly object items;

    public CacheSetMultiResponse(IDictionary<byte[], byte[]> items)
    {
        this.items = (object)items;
    }

    public CacheSetMultiResponse(IDictionary<string, string> items)
    {
        this.items = (object)items;
    }

    public IDictionary<string, string> Strings
    {
	get => (IDictionary<string, string>)items;
    }

    public IDictionary<byte[], byte[]> Bytes
    {
	get => (IDictionary<byte[], byte[]>)items;
    }
}
