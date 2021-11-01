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
            Assert.Equal(MomentoCacheResult.Hit, responseHit.Result);
            Assert.Equal(cacheBody, responseHit.String());

            GetResponse serverResponseMiss = new GetResponse() { Result = ECacheResult.Miss };
            CacheGetResponse responseMiss = new CacheGetResponse(serverResponseMiss);
            Assert.Equal(MomentoCacheResult.Miss, responseMiss.Result);

            GetResponse serverResponseBadRequest = new GetResponse() { Result = ECacheResult.BadRequest };
            CacheGetResponse responseBadRequest = new CacheGetResponse(serverResponseBadRequest);
            Assert.Equal(MomentoCacheResult.Unknown, responseBadRequest.Result);
        }
    }
}
