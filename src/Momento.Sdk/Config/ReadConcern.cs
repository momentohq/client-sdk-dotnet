using System;

namespace Momento.Sdk.Config;

/// <summary>
/// The read consistency setting for the cache client. Consistent guarantees read after write consistency, but applies a
/// 6x multiplier to your operation usage. 
/// </summary>
public enum ReadConcern
{
    /// <summary>
    /// Balanced is the default read concern for the cache client.
    /// </summary>
    Balanced,
    /// <summary>
    /// Consistent read concern guarantees read after write consistency.
    /// </summary>
    Consistent
}

/// <summary>
/// Extension methods for the ReadConcern enum.
/// </summary>
public static class ReadConcernExtensions
{
    /// <summary>
    /// Converts the read concern to a string value.
    /// </summary>
    /// <param name="readConcern">to convert to a string</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">if given an unknown read concern</exception>
    public static string ToStringValue(this ReadConcern readConcern)
    {
        return readConcern switch
        {
            ReadConcern.Balanced => "balanced",
            ReadConcern.Consistent => "consistent",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}