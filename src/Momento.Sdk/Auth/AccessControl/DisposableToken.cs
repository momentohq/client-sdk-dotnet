namespace Momento.Sdk.Auth.AccessControl;

public abstract record DisposableTokenPermission;

public abstract record DisposableToken
{
    public record CachePermission(CacheRole Role, CacheSelector CacheSelector) : DisposableTokenPermission
    {
        // public virtual bool Equals(CachePermission? other)
        // {
        //     return false;
        // }
    }

    public record CacheItemPermission
        (CacheRole Role, CacheSelector CacheSelector, CacheItemSelector CacheItemSelector) : CachePermission(Role,
            CacheSelector);

    public record TopicPermission(TopicRole Role, CacheSelector CacheSelector, TopicSelector TopicSelector) : DisposableTokenPermission;
}
