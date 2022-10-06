using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;
using Xunit;

namespace Momento.Sdk.Tests.Responses;

public class CacheGetResponseTest
{
    [Fact]
    public void CorrectResultMapping()
    {
        string cacheBody = "test body";
        ByteString body = ByteString.CopyFromUtf8(cacheBody);
        _GetResponse serverResponseHit = new _GetResponse() { CacheBody = body, Result = ECacheResult.Hit };
        CacheGetResponse.Hit responseHit = new CacheGetResponse.Hit(serverResponseHit);
        Assert.Equal(cacheBody, responseHit.ValueString);
    }
}
