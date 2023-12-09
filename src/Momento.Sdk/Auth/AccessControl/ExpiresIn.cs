using System;


/// <summary>
/// Represents an expiration time for a token.
/// </summary>
public abstract class Expiration
{
    private readonly bool doesExpire;

    /// <summary>
    /// Creates an expiration time.
    /// </summary>
    /// <param name="doesExpire">Whether the token expires.</param>
    protected Expiration(bool doesExpire)
    {
        this.doesExpire = doesExpire;
    }

    /// <summary>
    /// Whether the token expires.
    /// </summary>
    /// <returns>True if the token expires, false otherwise.</returns>
    public bool DoesExpire()
    {
        return doesExpire;
    }
}

/// <summary>
/// Represents an expiration time for a token that expires in a certain amount of time.
/// </summary>
public class ExpiresIn : Expiration
{
    private readonly int? validForSeconds;

    private ExpiresIn(int? validForSeconds) : base(validForSeconds != null)
    {
        this.validForSeconds = validForSeconds;
    }

    /// <summary>
    /// The number of seconds until the token expires.
    /// </summary>
    /// <returns>The number of seconds until the token expires.</returns>
    public int? Seconds()
    {
        return validForSeconds;
    }

    /// <summary>
    /// Creates an expiration time that never expires.
    /// </summary>
    /// <returns>An expiration time that never expires.</returns>
    public static ExpiresIn Never()
    {
        return new ExpiresIn(null);
    }

    /// <summary>
    /// Creates an expiration time that expires in a certain number of seconds.˜˜
    /// </summary>
    /// <param name="validForSeconds">The number of seconds until the token expires.</param>
    /// <returns>An expiration time that expires in a certain number of seconds.</returns>
    public static ExpiresIn Seconds(int validForSeconds)
    {
        return new ExpiresIn(validForSeconds);
    }

    /// <summary>
    /// Creates an expiration time that expires in a certain number of minutes.
    /// </summary>
    /// <param name="validForMinutes">The number of minutes until the token expires.</param>
    /// <returns>An expiration time that expires in a certain number of minutes.</returns>
    public static ExpiresIn Minutes(int validForMinutes)
    {
        return new ExpiresIn(validForMinutes * 60);
    }

    /// <summary>
    /// Creates an expiration time that expires in a certain number of hours.
    /// </summary>
    /// <param name="validForHours">The number of hours until the token expires.</param>
    /// <returns>An expiration time that expires in a certain number of hours.</returns>
    public static ExpiresIn Hours(int validForHours)
    {
        return new ExpiresIn(validForHours * 3600);
    }

    /// <summary>
    /// Creates an expiration time that expires in a certain number of days.
    /// </summary>
    /// <param name="validForDays">The number of days until the token expires.</param>
    /// <returns>An expiration time that expires in a certain number of days.</returns>
    public static ExpiresIn Days(int validForDays)
    {
        return new ExpiresIn(validForDays * 86400);
    }

    /// <summary>
    /// Creates an expiration time that expires at a UNIX epoch timestamp.
    /// </summary>
    /// <param name="expiresIn">The UNIX epoch timestamp at which the token expires.</param>
    /// <returns>An expiration time that expires at a UNIX epoch timestamp.</returns>
    public static ExpiresIn Epoch(ulong expiresIn)
    {
        ulong now = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        return new ExpiresIn((int)(expiresIn - now));
    }

    /// <summary>
    /// Creates an expiration time that expires at a UNIX epoch timestamp.
    /// </summary>
    /// <param name="expiresIn">The UNIX epoch timestamp at which the token expires.</param>
    /// <returns>An expiration time that expires at a UNIX epoch timestamp.</returns>
    public static ExpiresIn Epoch(int expiresIn)
    {
        return Epoch((ulong)expiresIn);
    }

}

/// <summary>
/// Represents an expiration time for a token that expires at a certain UNIX epoch timestamp.
/// </summary>
public class ExpiresAt : Expiration
{
    private readonly int? validUntil;

    /// <summary>
    /// Creates an expiration time that expires at a certain UNIX epoch timestamp.
    /// </summary>
    /// <param name="epochTimestamp">The UNIX epoch timestamp at which the token expires.</param>
    private ExpiresAt(int? epochTimestamp) : base(epochTimestamp != 0 && epochTimestamp != null)
    {
        if (this.DoesExpire())
        {
            this.validUntil = epochTimestamp;
        }
        else
        {
            this.validUntil = null;
        }
    }

    /// <summary>
    /// The UNIX epoch timestamp at which the token expires.
    /// </summary>
    /// <returns>The UNIX epoch timestamp at which the token expires.</returns>
    public int? Epoch()
    {
        return validUntil;
    }

    /// <summary>
    /// Creates an expiration time that never expires.
    /// </summary>
    /// <param name="epoch">The UNIX epoch timestamp at which the token expires.</param>
    /// <returns>An expiration time that expires at a certain UNIX epoch timestamp.</returns>
    public static ExpiresAt FromEpoch(int? epoch)
    {
        return new ExpiresAt(epoch);
    }
}
