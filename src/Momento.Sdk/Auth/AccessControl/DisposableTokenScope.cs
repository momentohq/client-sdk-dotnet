using System.Collections.Generic;

namespace Momento.Sdk.Auth.AccessControl;

/// <summary>
/// The permissions that can be granted.
/// </summary>
public class DisposableTokenScope
{
    /// <summary>
    /// The permissions that can be granted.
    /// </summary>
    public List<DisposableTokenPermission> Permissions { get; }

    /// <summary>
    /// Create a new <see cref="DisposableTokenScope"/> object.
    /// </summary>
    /// <param name="Permissions">The permissions that can be granted.</param>
    public DisposableTokenScope(List<DisposableTokenPermission> Permissions)
    {
        this.Permissions = Permissions;
    }
}
