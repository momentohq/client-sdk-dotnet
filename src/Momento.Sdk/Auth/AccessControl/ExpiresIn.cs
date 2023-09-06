using System;

namespace Momento.Sdk.Auth.AccessControl;

public class ExpiresIn
{
    readonly TimeSpan Value;

    public ExpiresIn(ulong expiresAt)
    {
        // TODO: make sure this is 1) the desired approach and 2) correct if so.
        ulong now = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        Value = TimeSpan.FromSeconds(expiresAt - now);
    }
}
