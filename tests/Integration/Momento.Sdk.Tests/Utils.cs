namespace Momento.Sdk.Tests.Integration;

/// <summary>
/// Ad-hoc utility methods.
/// </summary>
public static class Utils
{
    
    public static string TestCacheName() => "dotnet-integration-" + NewGuidString();
    
    public static string NewGuidString() => Guid.NewGuid().ToString();

    public static byte[] NewGuidByteArray() => Guid.NewGuid().ToByteArray();

    public static int initialRefreshTtl = 4;

    public static int updatedRefreshTtl = 10;

    public static int waitForItemToBeSet = 100;

    public static int waitForInitialItemToExpire = 4900;
}