#if !NET5_0_OR_GREATER

using System.ComponentModel;

// NOTE: this is required in order to use C# 9.0 records with older versions of .NET.
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit { }
}

#endif
