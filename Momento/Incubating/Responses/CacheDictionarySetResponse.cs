using System;
using System.Text;

namespace MomentoSdk.Incubating.Responses;

public class CacheDictionarySetResponse
{
    public string DictionaryName { get; private set; }
    private readonly object field;
    private readonly object value;

    public CacheDictionarySetResponse(string dictionaryName, object field, object value)
    {
        DictionaryName = dictionaryName;
        this.field = field;
        this.value = value;
    }

    public byte[] FieldToByteArray()
    {
        return (byte[])field;
    }

    public string FieldToString()
    {
        return (string)field;
    }

    public byte[] ValueToByteArray()
    {
        return (byte[])value;
    }

    public string ValueToString()
    {
        return (string)value;
    }
}
