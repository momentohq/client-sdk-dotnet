using System;
using System.Text;

namespace Momento.Sdk.Internal.ExtensionMethods;

/// <summary>
/// Defines extension methods to support pretty printing.
/// </summary>
public static class ByteArrayExtensions
{
    /// <summary>
    /// Converts the byte array to a hex string.
    /// </summary>
    /// <param name="byteArray">The byte array to convert.</param>
    /// <returns>The pretty hex string.</returns>
    public static string ToPrettyHexString(this byte[] byteArray)
    {
        return BitConverter.ToString(byteArray);
    }
}


