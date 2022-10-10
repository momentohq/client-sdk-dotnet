using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal;

namespace Momento.Sdk.Auth;

internal static class AuthUtils
{
    public static Claims TryDecodeAuthToken(string authToken)
    {
        try
        {
            return JwtUtils.DecodeJwt(authToken);
        }
        catch (InvalidArgumentException)
        {
            throw new InvalidArgumentException("The supplied Momento authToken is not valid.");
        }
    }
}
