using Xunit;
using MomentoSdk.Responses;
using CacheClient;
using Google.Protobuf;
using MomentoSdk.Exceptions;

namespace MomentoTest.Responses
{
    public class CacheGetResponseTest
    {
        [Fact]
        public void CorrectResultMapping()
        {
            string cacheBody = "test body";
            ByteString body = ByteString.CopyFromUtf8(cacheBody);
            GetResponse serverResponseHit = new GetResponse() { CacheBody = body, Result = ECacheResult.Hit };
            CacheGetResponse responseHit = new CacheGetResponse(serverResponseHit);
            Assert.Equal(CacheGetStatus.HIT, responseHit.Status);
            Assert.Equal(cacheBody, responseHit.String());

            GetResponse serverResponseMiss = new GetResponse() { Result = ECacheResult.Miss };
            CacheGetResponse responseMiss = new CacheGetResponse(serverResponseMiss);
            Assert.Equal(CacheGetStatus.MISS, responseMiss.Status);

            GetResponse serverResponseBadRequest = new GetResponse() { Result = ECacheResult.Invalid };
            _ = Assert.Throws<InternalServerException>(() => new CacheGetResponse(serverResponseBadRequest));
            
        }
    }
}
