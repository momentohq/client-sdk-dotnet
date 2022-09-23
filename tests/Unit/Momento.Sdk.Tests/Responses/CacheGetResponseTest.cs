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
        CacheGetResponse.Success responseHit = new CacheGetResponse.Success(serverResponseHit);
        Assert.Equal(CacheGetStatus.HIT, responseHit.Status);
        Assert.Equal(cacheBody, responseHit.String());

        _GetResponse serverResponseMiss = new _GetResponse() { Result = ECacheResult.Miss };
        CacheGetResponse.Success responseMiss = new CacheGetResponse.Success(serverResponseMiss);
        Assert.Equal(CacheGetStatus.MISS, responseMiss.Status);

        _GetResponse serverResponseBadRequest = new _GetResponse() { Result = ECacheResult.Invalid };
        _ = Assert.Throws<InternalServerException>(() => new CacheGetResponse.Success(serverResponseBadRequest));
    }
}
