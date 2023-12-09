namespace Momento.Sdk.Auth.AccessControl;

/// <summary>
/// Represents the permissions of a disposable token.
/// </summary>
public abstract record DisposableTokenPermission;

/// <summary>
/// A token that can be used to access a resource.
/// </summary>
public abstract record DisposableToken
{
    /// <summary>
    /// A cache permission.
    /// </summary>
    /// <param name="Role">The role.</param>
    /// <param name="CacheSelector">The cache selector for the permissions.</param>
    public record CachePermission(CacheRole Role, CacheSelector CacheSelector) : DisposableTokenPermission
    {
        // public virtual bool Equals(CachePermission? other)
        // {
        //     return false;
        // }
    }

    /// <summary>
    /// A cache item permission.
    /// </summary>
    /// <param name="Role">The role.</param>
    /// <param name="CacheSelector">The cache selector for the permissions.</param>
    /// <param name="CacheItemSelector">A specific cache item selector for the permissions.</param>
    public record CacheItemPermission
        (CacheRole Role, CacheSelector CacheSelector, CacheItemSelector CacheItemSelector) : CachePermission(Role,
            CacheSelector);

    /// <summary>
    /// A topic permission.
    /// </summary>
    /// <param name="Role">The role.</param>
    /// <param name="CacheSelector">The cache selector for the permissions.</param>
    /// <param name="TopicSelector">The topic selector for the permissions.</param>
    public record TopicPermission(TopicRole Role, CacheSelector CacheSelector, TopicSelector TopicSelector) : DisposableTokenPermission;
}
