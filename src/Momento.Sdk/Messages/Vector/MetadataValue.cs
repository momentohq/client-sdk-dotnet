using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Momento.Sdk.Messages.Vector;

/// <summary>
/// Container for a piece of vector metadata.
/// </summary>
public abstract class MetadataValue
{
    /// <inheritdoc />
    public abstract override string ToString();
    
    /// <summary>
    /// Implicitly convert a string to a StringValue.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    public static implicit operator MetadataValue(string value) => new StringValue(value);
    
    /// <summary>
    /// Implicitly convert a long to a LongValue.
    /// </summary>
    /// <param name="value">The long to convert.</param>
    public static implicit operator MetadataValue(long value) => new LongValue(value);
    
    /// <summary>
    /// Implicitly convert a double to a DoubleValue.
    /// </summary>
    /// <param name="value">The double to convert.</param>
    public static implicit operator MetadataValue(double value) => new DoubleValue(value);
    
    /// <summary>
    /// Implicitly convert a bool to a BoolValue.
    /// </summary>
    /// <param name="value">The bool to convert.</param>
    public static implicit operator MetadataValue(bool value) => new BoolValue(value);
    
    /// <summary>
    /// Implicitly convert a list of strings to a StringListValue.
    /// </summary>
    /// <param name="value">The list of strings to convert.</param>
    public static implicit operator MetadataValue(List<string> value) => new StringListValue(value);
}

/// <summary>
/// String vector metadata.
/// </summary>
public class StringValue : MetadataValue
{
    /// <summary>
    /// Constructs a StringValue.
    /// </summary>
    /// <param name="value">the string to wrap.</param>
    public StringValue(string value)
    {
        Value = value;
    }

    /// <summary>
    /// The wrapped string.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly convert a string to a StringValue.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    public static implicit operator StringValue(string value) => new StringValue(value);

    /// <summary>
    /// Explicitly convert a StringValue to a string.
    /// </summary>
    /// <param name="value">The StringValue to convert.</param>
    public static explicit operator string(StringValue value) => value.Value;

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is StringValue other && Value == other.Value;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

/// <summary>
/// Long vector metadata.
/// </summary>
public class LongValue : MetadataValue
{
    /// <summary>
    /// Constructs a LongValue.
    /// </summary>
    /// <param name="value">the long to wrap.</param>
    public LongValue(long value)
    {
        Value = value;
    }

    /// <summary>
    /// The wrapped long.
    /// </summary>
    public long Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Implicitly convert a long to a LongValue.
    /// </summary>
    /// <param name="value">The long to convert.</param>
    public static implicit operator LongValue(long value) => new LongValue(value);

    /// <summary>
    /// Explicitly convert a LongValue to a long.
    /// </summary>
    /// <param name="value">The LongValue to convert.</param>
    public static explicit operator long(LongValue value) => value.Value;

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is LongValue other && Value == other.Value;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

/// <summary>
/// Double vector metadata.
/// </summary>
public class DoubleValue : MetadataValue
{
    /// <summary>
    /// Constructs a DoubleValue.
    /// </summary>
    /// <param name="value">the double to wrap.</param>
    public DoubleValue(double value)
    {
        Value = value;
    }

    /// <summary>
    /// The wrapped double.
    /// </summary>
    public double Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Implicitly convert a double to a DoubleValue.
    /// </summary>
    /// <param name="value">The double to convert.</param>
    public static implicit operator DoubleValue(double value) => new DoubleValue(value);

    /// <summary>
    /// Explicitly convert a DoubleValue to a double.
    /// </summary>
    /// <param name="value">The DoubleValue to convert.</param>
    public static explicit operator double(DoubleValue value) => value.Value;

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return obj is DoubleValue other && Value == other.Value;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

/// <summary>
/// Boolean vector metadata.
/// </summary>
public class BoolValue : MetadataValue
{
    /// <summary>
    /// Constructs a BoolValue.
    /// </summary>
    /// <param name="value">the bool to wrap.</param>
    public BoolValue(bool value)
    {
        Value = value;
    }

    /// <summary>
    /// The wrapped bool.
    /// </summary>
    public bool Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Implicitly convert a bool to a BoolValue.
    /// </summary>
    /// <param name="value">The bool to convert.</param>
    public static implicit operator BoolValue(bool value) => new BoolValue(value);

    /// <summary>
    /// Explicitly convert a BoolValue to a bool.
    /// </summary>
    /// <param name="value">The BoolValue to convert.</param>
    public static explicit operator bool(BoolValue value) => value.Value;

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is BoolValue other && Value == other.Value;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

/// <summary>
/// String list vector metadata.
/// </summary>
public class StringListValue : MetadataValue
{
    /// <summary>
    /// Constructs a StringListValue.
    /// </summary>
    /// <param name="value">the list of strings to wrap.</param>
    public StringListValue(List<string> value)
    {
        Value = value;
    }

    /// <summary>
    /// The wrapped string list.
    /// </summary>
    public List<string> Value { get; }

    /// <inheritdoc />
    public override string ToString() => string.Join(", ", Value);

    /// <summary>
    /// Implicitly convert a list of strings to a StringListValue.
    /// </summary>
    /// <param name="value">The list of strings to convert.</param>
    public static implicit operator StringListValue(List<string> value) => new StringListValue(value);

    /// <summary>
    /// Explicitly convert a StringListValue to a list of strings.
    /// </summary>
    /// <param name="value">The StringListValue to convert.</param>
    public static explicit operator List<string>(StringListValue value) => value.Value;

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is StringListValue other && Value.SequenceEqual(other.Value);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Value.Aggregate(0, (acc, val) => acc ^ (val != null ? val.GetHashCode() : 0));
    }
}