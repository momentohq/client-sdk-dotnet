using System;
using System.Text;

namespace Momento.Sdk.Internal.ExtensionMethods;

/// <summary>
/// Defines extension methods to support pretty printing.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Truncates a string to a max length, eliding with a fill value.
    /// </summary>
    /// <param name="str">The string to truncate.</param>
    /// <param name="totalLength">Maximum allowed input length.</param>
    /// <param name="fillValue">The string to use to elide the input when it exceeds the max length.</param>
    /// <returns>The original string if not too long, else the truncated string.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string Truncate(this string str, int totalLength = 20, string fillValue = "...")
    {
        if (str.Length <= totalLength)
        {
            return str;
        }

        if (fillValue.Length >= totalLength)
        {
            throw new ArgumentException($"fillValue was larger than totalLength: {fillValue} vs {totalLength}");
        }

        var firstHalf = totalLength / 2 - fillValue.Length / 2;

        // Even length and odd sized fillvalue means the output will have the same number of characters
        // on either side of the fillValue. 
        if (totalLength % 2 == 0 && fillValue.Length % 2 == 1)
        {
            firstHalf--;
        }

        var sb = new StringBuilder();
        sb.Append(str.Substring(0, firstHalf));
        sb.Append(fillValue);
        sb.Append(str.Substring(str.Length - (totalLength - firstHalf - fillValue.Length)));
        return sb.ToString();
    }
}
