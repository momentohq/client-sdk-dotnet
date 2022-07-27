using System;
using System.Linq;
using System.Collections.Generic;
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

    /// <summary>
    /// Throw an exception if the argument is null.
    /// </summary>
    /// <param name="argument">The instance to check for null.</param>
    /// <param name="paramName">The name of the object to propagate to the exception.</param>
    /// <exception cref="ArgumentNullException">`argument` is `null`.</exception>
    public static void ArgumentNotNull(object? argument, string paramName)
    {
        if (argument == null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    /// <summary>
    /// Throw an exception if any of the dictionary values is null.
    /// </summary>
    /// <typeparam name="TKey">Dictionary key type.</typeparam>
    /// <typeparam name="TValue">Dictionary value type.</typeparam>
    /// <param name="argument">Dictionary to check for null values.</param>
    /// <param name="paramName">Name of the dictionary to propagate to the exception.</param>
    /// <exception cref="ArgumentNullException">Any of `argument` values is `null`.</exception>
    public static void DictionaryValuesNotNull<TKey, TValue>(IDictionary<TKey, TValue> argument, string paramName)
    {
        if (argument.Values.Any(value => value == null))
        {
            throw new ArgumentNullException(paramName, "Each value must be non-null");
        }
    }

    public static void ValuesNotNull<_, TValue>(IEnumerable<KeyValuePair<_, TValue>> argument, string paramName)
    {
        if (argument.Any(kv => kv.Value == null))
        {
            throw new ArgumentNullException(paramName, "Each value must be non-null");
        }
    }

    /// <summary>
    /// Throw an exception if any of the elements of the enumerable is null.
    /// </summary>
    /// <typeparam name="T">Enumerable element type.</typeparam>
    /// <param name="argument">Enumerable to check for null elements.</param>
    /// <param name="paramName">Name of the eumerable to propagate to the exception.</param>
    /// <exception cref="ArgumentNullException">Any of `argument` elements is `null`.</exception>
    public static void ElementsNotNull<T>(IEnumerable<T> argument, string paramName)
    {
        if (argument.Any(value => value == null))
        {
            throw new ArgumentNullException(paramName, "Each value must be non-null");
        }
    }
}
