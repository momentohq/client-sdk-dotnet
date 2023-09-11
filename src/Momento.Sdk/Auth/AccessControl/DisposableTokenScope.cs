#if !BUILD_FOR_UNITY
using System.Collections.Generic;

namespace Momento.Sdk.Auth.AccessControl;

public class DisposableTokenScope
{
    public List<DisposableTokenPermission> Permissions { get; }

    public DisposableTokenScope(List<DisposableTokenPermission> Permissions)
    {
        this.Permissions = Permissions;
    }
}
#endif
