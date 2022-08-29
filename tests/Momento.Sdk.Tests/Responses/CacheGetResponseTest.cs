using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;
using Xunit;

namespace MomentoTest.Responses;

public class CacheGetResponseTest
{
    [Fact]
    public void CorrectResultMapping()
    {
        string cacheBody = "test body";
        ByteString body = ByteString.CopyFromUtf8(cacheBody);
        _GetResponse serverResponseHit = new _GetResponse() { CacheBody = body, Result = ECacheResult.Hit };
        CacheGetResponse responseHit = new CacheGetResponse(serverResponseHit);
        Assert.Equal(CacheGetStatus.HIT, responseHit.Status);
        Assert.Equal(cacheBody, responseHit.String());

        _GetResponse serverResponseMiss = new _GetResponse() { Result = ECacheResult.Miss };
        CacheGetResponse responseMiss = new CacheGetResponse(serverResponseMiss);
        Assert.Equal(CacheGetStatus.MISS, responseMiss.Status);

        _GetResponse serverResponseBadRequest = new _GetResponse() { Result = ECacheResult.Invalid };
        _ = Assert.Throws<InternalServerException>(() => new CacheGetResponse(serverResponseBadRequest));

    }
}
