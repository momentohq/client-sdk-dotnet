using System;
using Xunit;
using MomentoSdk.Responses;
using CacheClient;
using Google.Protobuf;

namespace MomentoTest.Responses
{
    public class CacheGetResponseTest
    {
        [Fact]
        public void CorrectResultMapping()
        {
            String cacheBody = "test body";
            ByteString body = ByteString.CopyFromUtf8(cacheBody);
            GetResponse serverResponseHit = new GetResponse() { CacheBody = body, Result = ECacheResult.Hit };
            CacheGetResponse responseHit = new CacheGetResponse(serverResponseHit);
            Assert.Equal(MomentoCacheResult.HIT, responseHit.Result);
            Assert.Equal(cacheBody, responseHit.String());

            GetResponse serverResponseMiss = new GetResponse() { Result = ECacheResult.Miss };
            CacheGetResponse responseMiss = new CacheGetResponse(serverResponseMiss);
            Assert.Equal(MomentoCacheResult.MISS, responseMiss.Result);

            GetResponse serverResponseBadRequest = new GetResponse() { Result = ECacheResult.Invalid };
            CacheGetResponse responseBadRequest = new CacheGetResponse(serverResponseBadRequest);
            Assert.Equal(MomentoCacheResult.UNKNOWN, responseBadRequest.Result);
        }
    }
}
