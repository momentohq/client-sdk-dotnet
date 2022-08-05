using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;

namespace MomentoSdk.Internal
{
    /// <summary>
    /// Ad-hoc utility methods.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Make a globally unique ID as a string.
        /// </summary>
        /// <returns>A GUID as a string.</returns>
        public static string NewGuidString() => Guid.NewGuid().ToString();

        /// <summary>
        /// Make a globally unique ID as a byte array.
        /// </summary>
        /// <returns>A GUID as a byte array.</returns>
        public static byte[] NewGuidByteArray() => Guid.NewGuid().ToByteArray();

        /// <summary>
        /// Convert a UTF-8 encoded string to a byte array.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>The string as a byte array.</returns>
        public static byte[] Utf8ToByteArray(string s) => Encoding.UTF8.GetBytes(s);

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
        /// Throw an exception if any of the keys or values is null.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="argument">Enumerable to check for null keys/values.</param>
        /// <param name="paramName">Name of the enumerable to propagate to the exception.</param>
        /// <exception cref="ArgumentNullException">Any of `argument` keys or values is `null`.</exception>
        public static void KeysAndValuesNotNull<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> argument, string paramName)
        {
            if (argument.Any(kv => kv.Key == null || kv.Value == null))
            {
                throw new ArgumentNullException(paramName, "Each key and value must be non-null");
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

        /// <summary>
        /// Defines methods to support comparing containers of reference items by their
        /// contents (structure) instead of by reference.
        /// </summary>
        public class StructuralEqualityComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
            }
        }

        /// <summary>
        /// Comprarer to use in byte array containers (Set, Dictionary, List)
        /// so comparisons operate on byte-array content instead of reference.
        /// </summary>
        public static StructuralEqualityComparer<byte[]> ByteArrayComparer = new();
    }

    namespace ExtensionMethods
    {
        public static class ToByteStringExtensions
        {
            /// <summary>
            /// Convert a byte array to a `ByteString`
            /// </summary>
            /// <param name="byteArray">The byte array to convert.</param>
            /// <returns>The byte array as a `ByteString`</returns>
            public static ByteString ToByteString(this byte[] byteArray)
            {
                return ByteString.CopyFrom(byteArray);
            }

            /// <summary>
            /// Convert a UTF-8 string to a `ByteString`.
            /// </summary>
            /// <param name="str">The string to convert.</param>
            /// <returns>The string as a `ByteString`.</returns>
            public static ByteString ToByteString(this string str)
            {
                return ByteString.CopyFromUtf8(str);
            }
        }

        public static class ByteArrayDictionaryExtensions
        {
            /// <summary>
            /// DWIM equality implementation for dictionaries. cf `SetEquals`.
            /// https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.setequals?view=net-6.0
            ///
            /// Tests whether the dictionaries contain the same content as opposed to the same
            /// references.
            /// </summary>
            /// <param name="dictionary">LHS to compare</param>
            /// <param name="other">RHS to compare</param>
            /// <returns>`true` if the dictionaries contain the same content.</returns>
            public static bool DictionaryEquals(this Dictionary<byte[], byte[]> dictionary, Dictionary<byte[], byte[]> other)
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
    }
}
