using System.Collections.Generic;
using System.Linq;

namespace Momento.Sdk.Internal.ExtensionMethods;

/// <summary>
/// Defines extension methods to operate on dictionaries with
/// byte array keys and values.
/// </summary>
public static class ByteArrayDictionaryExtensions
{
    /// <summary>
    /// DWIM equality implementation for dictionaries, cf <see href="https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.setequals?view=net-6.0">SetEquals</see>.
    ///
    ///
    /// Tests whether the dictionaries contain the same content as opposed to the same
    /// references.
    /// </summary>
    /// <param name="dictionary">LHS to compare</param>
    /// <param name="other">RHS to compare</param>
    /// <returns><see langword="true"/> if the dictionaries contain the same content.</returns>
    public static bool DictionaryEquals(this IDictionary<byte[], byte[]> dictionary, IDictionary<byte[], byte[]> other)
    {
        if (dictionary == null && other == null)
        {
            return true;
        }
        else if (dictionary == null || other == null)
        {
            return false;
        }
        else if (dictionary.Count != other.Count)
        {
            return false;
        }

        var keySet = new HashSet<byte[]>(dictionary.Keys, Utils.ByteArrayComparer);
        return other.All(kv => keySet.Contains(kv.Key) && dictionary[kv.Key].SequenceEqual(kv.Value));
    }
}


/// <summary>
/// Defines extension methods to operate on dictionaries with
/// string keys and values.
/// </summary>
public static class StringStringDictionaryExtensions
{
    /// <summary>
    /// Make a shallow copy of a dictionary.
    /// </summary>
    /// <param name="dictionary">The dictionary to copy.</param>
    /// <returns>A new dictionary container with the same contents.</returns>
    public static IDictionary<string, string> Clone(this IDictionary<string, string> dictionary)
    {
        return new Dictionary<string, string>(dictionary);
    }
}


/// <summary>
/// Defines extension methods to operate on dictionaries with
/// generic keys and values.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Add all items in <paramref name="items"/> to <paramref name="dictionary"/>.
    /// </summary>
    /// <remark>
    /// We add this since the .NET Framework does not have a constructor that takes an
    /// <see cref="IEnumerable{T}"/>.
    /// </remark>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary to add the items to.</param>
    /// <param name="items">The key-value pairs to add.</param>
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        foreach (var item in items)
        {
            dictionary.Add(item);
        }
    }
}
