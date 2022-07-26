using System;
using System.Collections.Generic;
using System.Text;

namespace MomentoSdk.Incubating.Responses;

public class CacheDictionarySetMultiResponse
{
    public string DictionaryName { get; private set; }
    private readonly object items;

    public CacheDictionarySetMultiResponse(string dictionaryName, object items)
    {
        DictionaryName = dictionaryName;
        this.items = items;
    }

    public IEnumerable<KeyValuePair<byte[], byte[]>> ItemsAsByteArrays()
    {
        return (IEnumerable<KeyValuePair<byte[], byte[]>>)items;
    }

    public IEnumerable<KeyValuePair<string, string>> ItemsAsStrings()
    {
        return (IEnumerable<KeyValuePair<string, string>>)items;
    }
}
