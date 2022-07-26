using System;
using System.Text;

namespace MomentoSdk.Internal;

/// <summary>
/// Ad-hoc utility methods.
/// </summary>
public class Utils
{
    /// <summary>
    /// Make a globally unique ID as a string.
    /// </summary>
    /// <returns>A GUID as a string.</returns>
    public static string GuidString() => Guid.NewGuid().ToString();

    /// <summary>
    /// Make a globally unique ID as a byte array.
    /// </summary>
    /// <returns>A GUID as a byte array.</returns>
    public static byte[] GuidBytes() => Guid.NewGuid().ToByteArray();

    /// <summary>
    /// Convert a UTF-8 encoded string to a byte array.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>The string as a byte array.</returns>
    public static byte[] Utf8ToBytes(string s) => Encoding.UTF8.GetBytes(s);
}
