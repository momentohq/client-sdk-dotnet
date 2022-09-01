using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Protobuf;

namespace Momento.Sdk.Internal
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
        /// Throw an exception if the argument is <see langword="null"/>.
        /// </summary>
        /// <param name="argument">The instance to check for <see langword="null"/>.</param>
        /// <param name="paramName">The name of the object to propagate to the exception.</param>
        /// <exception cref="ArgumentNullException"><paramref name="argument"/> is <see langword="null"/>.</exception>
        public static void ArgumentNotNull(object? argument, string paramName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Throw an exception if any of the keys or values is <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="argument">Enumerable to check for <see langword="null"/> keys/values.</param>
        /// <param name="paramName">Name of the enumerable to propagate to the exception.</param>
        /// <exception cref="ArgumentNullException">Any of <paramref name="argument"/> keys or values is <see langword="null"/>.</exception>
        public static void KeysAndValuesNotNull<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> argument, string paramName)
        {
            if (argument.Any(kv => kv.Key == null || kv.Value == null))
            {
                throw new ArgumentNullException(paramName, "Each key and value must be non-null");
            }
        }

        /// <summary>
        /// Throw an exception if any of the elements of the enumerable is <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">Enumerable element type.</typeparam>
        /// <param name="argument">Enumerable to check for <see langword="null"/> elements.</param>
        /// <param name="paramName">Name of the eumerable to propagate to the exception.</param>
        /// <exception cref="ArgumentNullException">Any of <paramref name="argument"/> elements is <see langword="null"/>.</exception>
        public static void ElementsNotNull<T>(IEnumerable<T> argument, string paramName)
        {
            if (argument.Any(value => value == null))
            {
                throw new ArgumentNullException(paramName, "Each value must be non-null");
            }
        }

        /// <summary>
        /// Throw an exception if the argument is zero.
        /// </summary>
        /// <param name="argument">The integer to zero test.</param>
        /// <param name="paramName">Name of the integer to propagate to the exception.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="argument"/> is zero.</exception>
        public static void ArgumentStrictlyPositive(uint? argument, string paramName)
        {
            if (argument == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, "Number must be strictly positive.");
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

        public static class ByteArrayListExtensions
        {
            /// <summary>
            /// DWIM equality implementation for lists, cf <see href="https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.setequals?view=net-6.0">SetEquals</see>.
            /// 
            ///
            /// Tests whether the lists contain the same content as opposed to the same
            /// references.
            /// </summary>
            /// <param name="list">LHS to compare</param>
            /// <param name="other">RHS to compare</param>
            /// <returns><see langword="true"/> if the lists contain the same content.</returns>
            public static bool ListEquals(this List<byte[]> list, List<byte[]> other)
            {
                if (list == null && other == null)
                {
                    return true;
                }
                else if (list == null || other == null)
                {
                    return false;
                }
                else if (list.Count != other.Count)
                {
                    return false;
                }

                return Enumerable.Range(0, list.Count).All(index => list[index].SequenceEqual(other[index]));
            }
        }

    }
}
