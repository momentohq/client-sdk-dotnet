using System;


public abstract class Expiration
{
    private readonly bool doesExpire;

    protected Expiration(bool doesExpire){
        this.doesExpire = doesExpire;
    }

    public bool DoesExpire()
    {
        return doesExpire;
    }
}

public class ExpiresIn : Expiration
{
    private readonly int? validForSeconds;

    private ExpiresIn(int? validForSeconds) : base(validForSeconds != null)
    {
         this.validForSeconds = validForSeconds;
    }

    public int? Seconds()
    {
        return validForSeconds;
    }

    public static ExpiresIn Never() {
        return new ExpiresIn(null);
    }

    public static ExpiresIn Seconds(int validForSeconds)
    {
        return new ExpiresIn(validForSeconds);
    }

    public static ExpiresIn Minutes(int validForSeconds)
    {
        return new ExpiresIn(validForSeconds * 60);
    }

    public static ExpiresIn Hours(int validForSeconds)
    {
        return new ExpiresIn(validForSeconds * 3600);
    }

    public static ExpiresIn Days(int validForSeconds)
    {
        return new ExpiresIn(validForSeconds * 86400);
    }

    // TODO: not sure yet what I really want to do with ulong vs int in these classes.
    //  Prolly oughtta standardize on one or support both.
    public static ExpiresIn Epoch(ulong expiresBy)
    {
        ulong now = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        return new ExpiresIn((int)(expiresBy - now));
    }
}

public class ExpiresAt : Expiration
{
    private readonly int? validUntil;

    private ExpiresAt(int? epochTimestamp) : base(epochTimestamp != 0 && epochTimestamp != null) {
        if (this.DoesExpire())
        {
            this.validUntil = epochTimestamp;
        }
        else
        {
            this.validUntil = null;
        }
    }

    public int? Epoch()
    {
        return validUntil;
    }

    public static ExpiresAt FromEpoch(int? epoch)
    {
        return new ExpiresAt(epoch);
    }
}
