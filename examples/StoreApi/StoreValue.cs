namespace Momento.Sdk.StoreClient;

public enum StoreValueType
{
    String,
    Integer64,
    Double,
    Bool
}

public class StoreValue
{
    private object _value;
    public StoreValueType Type { get; }

    public StoreValue(object value, StoreValueType type)
    {
        _value = value;
        Type = type;
    }

    public StoreValue(string value) : this(value, StoreValueType.String)
    {
    }

    public StoreValue(long value) : this(value, StoreValueType.Integer64)
    {
    }

    public StoreValue(double value) : this(value, StoreValueType.Double)
    {
    }

    public StoreValue(bool value) : this(value, StoreValueType.Bool)
    {
    }

    // implicit cast from the various types into StoreValue
    public static implicit operator StoreValue(string value) => new StoreValue(value);
    public static implicit operator StoreValue(long value) => new StoreValue(value);
    public static implicit operator StoreValue(double value) => new StoreValue(value);
    public static implicit operator StoreValue(bool value) => new StoreValue(value);

    // implicit cast from StoreValue into the various types
    public static implicit operator string(StoreValue value) => (string)value._value;
    public static implicit operator long(StoreValue value) => (long)value._value;
    public static implicit operator double(StoreValue value) => (double)value._value;
    public static implicit operator bool(StoreValue value) => (bool)value._value;
}
