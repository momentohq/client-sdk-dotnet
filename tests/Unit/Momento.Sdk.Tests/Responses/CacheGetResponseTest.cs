using Google.Protobuf;
using Momento.Protos.CacheClient;
using Momento.Sdk.Responses;
using Xunit;

namespace Momento.Sdk.Tests.Unit;

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

    [Theory]
    [InlineData("test body", "Momento.Sdk.Responses.CacheGetResponse+Hit: ValueString: \"test body\" ValueByteArray: \"74-65-73...-6F-64-79\"")]
    [InlineData("this is a very long string and will be truncated", "Momento.Sdk.Responses.CacheGetResponse+Hit: ValueString: \"this is ...truncated\" ValueByteArray: \"74-68-69...-74-65-64\"")]
    public void ToString_Truncates_HappyPath(string input, string expected)
    {
        ByteString body = ByteString.CopyFromUtf8(input);
        _GetResponse serverResponseHit = new _GetResponse() { CacheBody = body, Result = ECacheResult.Hit };
        CacheGetResponse.Hit responseHit = new CacheGetResponse.Hit(serverResponseHit);

        var asString = responseHit.ToString();
        Assert.Equal(expected, asString);
    }
}
