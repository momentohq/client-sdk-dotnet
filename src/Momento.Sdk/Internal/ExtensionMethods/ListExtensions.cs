using System.Collections.Generic;
using System.Linq;

namespace Momento.Sdk.Internal.ExtensionMethods;

/// <summary>
/// Defines extension methods to operate on lists containing
/// byte arrays.
/// </summary>
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
    public static bool ListEquals(this IList<byte[]> list, IList<byte[]> other)
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
