using System.Collections.Generic;

namespace Momento.Sdk.Requests.Vector;

/// <summary>
/// Wrapper for a list of metadata fields. Used in vector methods that can either take a
/// list of metadata to look up, or a value specifying that all metadata should be returned.
/// </summary>
public abstract class MetadataFields
{
    /// <summary>
    /// Static value representing all metadata fields.
    /// </summary>
    public static readonly AllFields All = new AllFields();

    /// <summary>
    /// Implicitly convert a list of strings to a MetadataFields. Allows for passing a bare list instead
    /// of having to explicitly create a MetadataFields object.
    /// </summary>
    /// <param name="fields">The fields to look up.</param>
    /// <returns></returns>
    public static implicit operator MetadataFields(List<string> fields) => new List(fields);

    /// <summary>
    /// MetadataFields implementation representing a list of specific fields.
    /// </summary>
    public class List : MetadataFields
    {
        /// <summary>
        /// Constructs a MetadataFields.List with specific fields.
        /// </summary>
        /// <param name="fields">The fields to look up.</param>
        public List(IEnumerable<string> fields)
        {
            Fields = fields;
        }

        /// <summary>
        /// The fields to look up.
        /// </summary>
        public IEnumerable<string> Fields { get; }
    }

    /// <summary>
    /// MetadataFields implementation representing all fields.
    /// </summary>
    public class AllFields : MetadataFields
    {
        /// <summary>
        /// Constructs a MetadataFields.All.
        /// </summary>
        public AllFields()
        {
        }
    }
}