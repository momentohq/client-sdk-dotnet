using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;

namespace Momento.Sdk.Internal.ExtensionMethods;

/// <summary>
/// Defines extension methods to support the conversion of data to
/// various forms of ByteString.
/// </summary>
public static class ToByteStringExtensions
{
    /// <summary>
    /// Convert a byte array to a <see cref="ByteString"/>
    /// </summary>
    /// <param name="byteArray">The byte array to convert.</param>
    /// <returns>The byte array as a <see cref="ByteString"/></returns>
    public static ByteString ToByteString(this byte[] byteArray)
    {
        return ByteString.CopyFrom(byteArray);
    }

    /// <summary>
    /// Convert a UTF-8 string to a <see cref="ByteString"/>.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>The string as a <see cref="ByteString"/>.</returns>
    public static ByteString ToByteString(this string str)
    {
        return ByteString.CopyFromUtf8(str);
    }

    /// <summary>
    /// Convert a byte array to a singleton <see cref="ByteString"/> array
    /// </summary>
    /// <param name="byteArray">The byte array to convert.</param>
    /// <returns>A length one array containing the converted byte array.</returns>
    public static ByteString[] ToSingletonByteString(this byte[] byteArray)
    {
        return new ByteString[] { byteArray.ToByteString() };
    }

    /// <summary>
    /// Convert a UTF-8 string to a singleton <see cref="ByteString"/> array.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>A length one array containing the converted string.</returns>
    public static ByteString[] ToSingletonByteString(this string str)
    {
        return new ByteString[] { str.ToByteString() };
    }

    /// <summary>
    /// Convert an enumerable of byte arrays to <see cref="IEnumerable{ByteString}"/>
    /// </summary>
    /// <param name="enumerable">The enumerable to convert.</param>
    /// <returns>An enumerable over <see cref="ByteString"/>s.</returns>
    public static IEnumerable<ByteString> ToEnumerableByteString(this IEnumerable<byte[]> enumerable)
    {
        return enumerable.Select(item => item.ToByteString());
    }

    /// <inheritdoc cref="ToEnumerableByteString(IEnumerable{byte[]})"/>
    public static IEnumerable<ByteString> ToEnumerableByteString(this IEnumerable<string> enumerable)
    {
        return enumerable.Select(item => item.ToByteString());
    }
}
