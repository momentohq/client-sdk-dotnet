using System.Collections.Generic;
using Grpc.Core;

namespace Momento.Sdk.Internal.ExtensionMethods;

/// <summary>
/// Defines extension methods to operate on grpc Metadata.
/// </summary>
public static class MetadataExtensions
{
    /// <summary>
    /// Adds a string-valued header to the metadata if it does not already exist.
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void TryAddHeader(this Metadata metadata, string key, string? value = null)
    {
        if (value != null && metadata.GetValue(key) == null)
        {
            metadata.Add(key, value);
        }
    }

    /// <summary>
    /// Adds a string list-valued header to the metadata if it does not already exist.
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="key"></param>
    /// <param name="values"></param>
    public static void TryAddHeader(this Metadata metadata, string key, IList<string>? values = null)
    {
        if (values != null && values.Count > 0 && metadata.GetValue(key) == null)
        {
            var valuesAsString = string.Join(" ", values);
            metadata.Add(key, valuesAsString);
        }
    }

    /// <summary>
    /// Adds an int-valued header to the metadata if it does not already exist.
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void TryAddHeader(this Metadata metadata, string key, int? value = null)
    {
        if (value != null && metadata.GetValue(key) == null)
        {
            metadata.Add(key, value.ToString());
        }
    }
}