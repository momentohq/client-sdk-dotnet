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
            ByteString body = ByteString.CopyFromUtf8("test body");
            GetResponse serverResponseHit = new GetResponse() { CacheBody = body, Result = ECacheResult.Hit };
            CacheGetResponse responseHit = new CacheGetResponse(serverResponseHit);
            Assert.Equal(MomentoCacheResult.Hit, responseHit.result);
            Assert.Equal(body, responseHit.body);

            GetResponse serverResponseMiss = new GetResponse() { Result = ECacheResult.Miss };
            CacheGetResponse responseMiss = new CacheGetResponse(serverResponseMiss);
            Assert.Equal(MomentoCacheResult.Miss, responseMiss.result);

            GetResponse serverResponseBadRequest = new GetResponse() { Result = ECacheResult.BadRequest };
            CacheGetResponse responseBadRequest = new CacheGetResponse(serverResponseBadRequest);
            Assert.Equal(MomentoCacheResult.Unknown, responseBadRequest.result);
        }
    }
}
