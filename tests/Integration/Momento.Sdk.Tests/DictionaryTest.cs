using System.Collections.Generic;
using System.Threading.Tasks;
using Momento.Sdk.Internal.ExtensionMethods;
using Momento.Sdk.Requests;

namespace Momento.Sdk.Tests;

[Collection("CacheClient")]
public class DictionaryTest : TestBase
{
    public DictionaryTest(CacheClientFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData(null, "my-dictionary", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", null)]
    public async Task DictionaryGetFieldAsync_NullChecksFieldIsByteArray_IsError(string cacheName, string dictionaryName, byte[] field)
    {
        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Error, $"Unexpected response: {response}");
        var errResponse = (CacheDictionaryGetFieldResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldResponse.Error)response).ErrorCode);
        Assert.Equal(field ?? new byte[] { }, ((CacheDictionaryGetFieldResponse.Error)response).FieldByteArray);
    }

    [Theory]
    [InlineData(null, "my-dictionary", new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 }, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", new byte[] { 0x00 }, null)]
    public async Task DictionarySetFieldAsync_NullChecksFieldIsByteArrayValueIsByteArray_IsError(string cacheName, string dictionaryName, byte[] field, byte[] value)
    {
        CacheDictionarySetFieldResponse response = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value);
        Assert.True(response is CacheDictionarySetFieldResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionaryGetFieldAsync_FieldIsByteArray_DictionaryIsMissing()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
        var missResponse = (CacheDictionaryGetFieldResponse.Miss)response;
        Assert.Equal(field, missResponse.FieldByteArray);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsByteArrayValueIsByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value);

        CacheDictionaryGetFieldResponse getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
        var hitResponse = (CacheDictionaryGetFieldResponse.Hit)getResponse;
        Assert.Equal(field, hitResponse.FieldByteArray);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsByteArrayDictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        CacheDictionarySetFieldResponse setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value);
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");

        var otherField = Utils.NewGuidByteArray();
        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, otherField);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsByteArrayValueIsByteArray_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        CacheDictionarySetFieldResponse setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, CollectionTtl.Of(TimeSpan.FromSeconds(Utils.InitialRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");
        await Task.Delay(Utils.WaitForItemToBeSet);

        setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");
        await Task.Delay(Utils.WaitForInitialItemToExpire);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsByteArrayValueIsByteArray_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        CacheDictionarySetFieldResponse setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, CollectionTtl.Of(TimeSpan.FromSeconds(2)));
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");
        setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)));
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");
        await Task.Delay(2000);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {response}");
        Assert.Equal(value, ((CacheDictionaryGetFieldResponse.Hit)response).ValueByteArray);
    }

    [Theory]
    [InlineData(null, "my-dictionary", "field")]
    [InlineData("cache", null, "field")]
    [InlineData("cache", "my-dictionary", null)]
    public async Task DictionaryIncrementAsync_NullChecksFieldIsString_IsError(string cacheName, string dictionaryName, string field)
    {
        CacheDictionaryIncrementResponse response = await client.DictionaryIncrementAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryIncrementResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryIncrementResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionaryIncrementAsync_IncrementFromZero_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var fieldName = Utils.NewGuidString();

        CacheDictionaryIncrementResponse incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, fieldName, 1);
        Assert.True(incrementResponse is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        var successResponse = (CacheDictionaryIncrementResponse.Success)incrementResponse;
        Assert.Equal(1, successResponse.Value);

        incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, fieldName, 41);
        Assert.True(incrementResponse is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        successResponse = (CacheDictionaryIncrementResponse.Success)incrementResponse;
        Assert.Equal(42, successResponse.Value);
        Assert.Equal("Momento.Sdk.Responses.CacheDictionaryIncrementResponse+Success: Value: 42", successResponse.ToString());

        incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, fieldName, -1042);
        Assert.True(incrementResponse is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        successResponse = (CacheDictionaryIncrementResponse.Success)incrementResponse;
        Assert.Equal(-1000, successResponse.Value);

        CacheDictionaryGetFieldResponse getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, fieldName);
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
        var hitResponse = (CacheDictionaryGetFieldResponse.Hit)getResponse;
        Assert.Equal("-1000", hitResponse.ValueString);
    }

    [Fact]
    public async Task DictionaryIncrementAsync_IncrementFromZero_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();

        CacheDictionaryIncrementResponse resp = await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)));
        Assert.True(resp is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {resp}");
        resp = await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)));
        Assert.True(resp is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {resp}");
        await Task.Delay(2000);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {response}");
        Assert.Equal("2", ((CacheDictionaryGetFieldResponse.Hit)response).ValueString);
    }

    [Fact]
    public async Task DictionaryIncrementAsync_IncrementFromZero_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();

        CacheDictionaryIncrementResponse resp = await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.InitialRefreshTtl)));
        Assert.True(resp is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {resp}");
        await Task.Delay(Utils.WaitForItemToBeSet);

        resp = await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(resp is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {resp}");
        await Task.Delay(Utils.WaitForInitialItemToExpire);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionaryIncrementAsync_SetAndReset_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();

        // Set field
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, "10");
        CacheDictionaryIncrementResponse incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, amount: 0);
        Assert.True(incrementResponse is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        var successResponse = (CacheDictionaryIncrementResponse.Success)incrementResponse;
        Assert.Equal(10, successResponse.Value);

        incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, amount: 90);
        Assert.True(incrementResponse is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        successResponse = (CacheDictionaryIncrementResponse.Success)incrementResponse;
        Assert.Equal(100, successResponse.Value);

        // Reset field
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, "0");
        incrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, field, amount: 0);
        Assert.True(incrementResponse is CacheDictionaryIncrementResponse.Success, $"Unexpected response: {incrementResponse}");
        successResponse = (CacheDictionaryIncrementResponse.Success)incrementResponse;
        Assert.Equal(0, successResponse.Value);
    }

    [Fact]
    public async Task DictionaryIncrementAsync_FailedPrecondition_IsError()
    {
        var dictionaryName = Utils.NewGuidString();
        var fieldName = Utils.NewGuidString();

        var setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, fieldName, "abcxyz");
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");

        var dictionaryIncrementResponse = await client.DictionaryIncrementAsync(cacheName, dictionaryName, fieldName);
        Assert.True(dictionaryIncrementResponse is CacheDictionaryIncrementResponse.Error, $"Unexpected response: {dictionaryIncrementResponse}");
        Assert.Equal(MomentoErrorCode.FAILED_PRECONDITION_ERROR, ((CacheDictionaryIncrementResponse.Error)dictionaryIncrementResponse).ErrorCode);
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field")]
    [InlineData("cache", null, "my-field")]
    [InlineData("cache", "my-dictionary", null)]
    public async Task DictionaryGetFieldAsync_NullChecksFieldIsString_IsError(string cacheName, string dictionaryName, string field)
    {
        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Error, $"Unexpected response: {response}");
        var errResponse = (CacheDictionaryGetFieldResponse.Error)response;
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldResponse.Error)response).ErrorCode);
        Assert.Equal(field ?? "", ((CacheDictionaryGetFieldResponse.Error)response).FieldString);
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field", "my-value")]
    [InlineData("cache", null, "my-field", "my-value")]
    [InlineData("cache", "my-dictionary", null, "my-value")]
    [InlineData("cache", "my-dictionary", "my-field", null)]
    public async Task DictionarySetFieldAsync_NullChecksFieldIsStringValueIsString_IsError(string cacheName, string dictionaryName, string field, string value)
    {
        CacheDictionarySetFieldResponse response = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value);
        Assert.True(response is CacheDictionarySetFieldResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionaryGetFieldAsync_FieldIsString_DictionaryIsMissing()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
        var missResponse = (CacheDictionaryGetFieldResponse.Miss)response;
        Assert.Equal(field, missResponse.FieldString);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsStringValueIsString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        CacheDictionarySetFieldResponse response = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value);
        Assert.True(response is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {response}");

        CacheDictionaryGetFieldResponse getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
        var hitResponse = (CacheDictionaryGetFieldResponse.Hit)getResponse;
        Assert.Equal(field, hitResponse.FieldString);
    }

    [Fact]
    public async Task CacheDictionaryGetFieldResponse_ToString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();

        CacheDictionarySetFieldResponse response = await client.DictionarySetFieldAsync(cacheName, dictionaryName, "a", "b");
        Assert.True(response is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {response}");

        CacheDictionaryGetFieldResponse getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, "a");
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
        Assert.Equal("Momento.Sdk.Responses.CacheDictionaryGetFieldResponse+Hit: ValueString: \"b\" ValueByteArray: \"62\"", getResponse.ToString());
    }

    [Fact]
    public async Task DictionarySetGetAsync_DictionaryIsPresent_FieldIsMissing()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        CacheDictionarySetFieldResponse setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value);
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");

        var otherField = Utils.NewGuidString();
        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, otherField);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsStringValueIsString_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        CacheDictionarySetFieldResponse setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.InitialRefreshTtl)));
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");
        await Task.Delay(Utils.WaitForItemToBeSet);

        setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");
        await Task.Delay(Utils.WaitForInitialItemToExpire);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsStringValueIsString_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();

        CacheDictionarySetFieldResponse setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)));
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");
        setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)));
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");
        await Task.Delay(2000);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {response}");
        Assert.Equal(value, ((CacheDictionaryGetFieldResponse.Hit)response).ValueString);
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field", new byte[] { 0x00 })]
    [InlineData("cache", null, "my-field", new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-dictionary", "my-field", null)]
    public async Task DictionarySetFieldAsync_NullChecksFieldIsStringValueIsByteArray_IsError(string cacheName, string dictionaryName, string field, byte[] value)
    {
        CacheDictionarySetFieldResponse response = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value);
        Assert.True(response is CacheDictionarySetFieldResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsStringValueIsByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        CacheDictionarySetFieldResponse setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value);
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");

        CacheDictionaryGetFieldResponse getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsStringValueIsByteArray_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        CacheDictionarySetFieldResponse setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.InitialRefreshTtl)));
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");
        await Task.Delay(Utils.WaitForItemToBeSet);

        setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");
        await Task.Delay(Utils.WaitForInitialItemToExpire);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionarySetGetAsync_FieldIsStringValueIsByteArray_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();

        CacheDictionarySetFieldResponse setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)));
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");
        setResponse = await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)));
        Assert.True(setResponse is CacheDictionarySetFieldResponse.Success, $"Unexpected response: {setResponse}");
        await Task.Delay(2000);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {response}");
        Assert.Equal(value, ((CacheDictionaryGetFieldResponse.Hit)response).ValueByteArray);
    }

    [Fact]
    public async Task DictionarySetFieldsAsync_NullChecksFieldIsByteArrayValueIsByteArray_IsError()
    {
        var dictionaryName = Utils.NewGuidString();
        var dictionary = new Dictionary<byte[], byte[]>();
        CacheDictionarySetFieldsResponse response = await client.DictionarySetFieldsAsync(null!, dictionaryName, dictionary);
        Assert.True(response is CacheDictionarySetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionarySetFieldsAsync(cacheName, null!, dictionary);
        Assert.True(response is CacheDictionarySetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionarySetFieldsAsync(cacheName, dictionaryName, (IEnumerable<KeyValuePair<byte[], byte[]>>)null!);
        Assert.True(response is CacheDictionarySetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldsResponse.Error)response).ErrorCode);

        dictionary[Utils.NewGuidByteArray()] = null!;
        response = await client.DictionarySetFieldsAsync(cacheName, dictionaryName, dictionary);
        Assert.True(response is CacheDictionarySetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldsResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionarySetFieldsAsync_FieldsAreByteArrayValuesAreByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();

        var items = new Dictionary<byte[], byte[]>() { { field1, value1 }, { field2, value2 } };

        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, items, CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)));

        CacheDictionaryGetFieldResponse getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field1);
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
        Assert.Equal(value1, ((CacheDictionaryGetFieldResponse.Hit)getResponse).ValueByteArray);

        getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field2);
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
        Assert.Equal(value2, ((CacheDictionaryGetFieldResponse.Hit)getResponse).ValueByteArray);
    }

    [Fact]
    public async Task DictionarySetFieldsAsync_FieldsAreByteArrayValuesAreByteArray_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.InitialRefreshTtl)));
        await Task.Delay(Utils.WaitForItemToBeSet);

        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(Utils.WaitForInitialItemToExpire);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionarySetFieldsAsync_FieldsAreByteArrayValuesAreByteArray_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();
        var content = new Dictionary<byte[], byte[]>() { { field, value } };

        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)));
        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)));
        await Task.Delay(2000);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {response}");
        Assert.Equal(value, ((CacheDictionaryGetFieldResponse.Hit)response).ValueByteArray);
    }

    [Fact]
    public async Task DictionarySetFieldsAsync_NullChecksFieldsAreStringValuesAreString_IsError()
    {
        var dictionaryName = Utils.NewGuidString();
        var dictionary = new Dictionary<string, string>();
        CacheDictionarySetFieldsResponse response = await client.DictionarySetFieldsAsync(null!, dictionaryName, dictionary);
        Assert.True(response is CacheDictionarySetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionarySetFieldsAsync(cacheName, null!, dictionary);
        Assert.True(response is CacheDictionarySetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionarySetFieldsAsync(cacheName, dictionaryName, (IEnumerable<KeyValuePair<string, string>>)null!);
        Assert.True(response is CacheDictionarySetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldsResponse.Error)response).ErrorCode);

        dictionary[Utils.NewGuidString()] = null!;
        response = await client.DictionarySetFieldsAsync(cacheName, dictionaryName, dictionary);
        Assert.True(response is CacheDictionarySetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldsResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionarySetFieldsAsync_FieldsAreStringValuesAreString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();

        var items = new Dictionary<string, string>() { { field1, value1 }, { field2, value2 } };

        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, items);

        CacheDictionaryGetFieldResponse getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field1);
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
        Assert.Equal(value1, ((CacheDictionaryGetFieldResponse.Hit)getResponse).ValueString);

        getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field2);
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
        Assert.Equal(value2, ((CacheDictionaryGetFieldResponse.Hit)getResponse).ValueString);
    }

    [Fact]
    public async Task DictionarySetFieldsAsync_FieldsAreStringValuesAreString_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.InitialRefreshTtl)));
        await Task.Delay(Utils.WaitForItemToBeSet);

        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(Utils.WaitForInitialItemToExpire);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionarySetFieldsAsync_FieldsAreStringValuesAreString_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidString();
        var content = new Dictionary<string, string>() { { field, value } };

        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)));
        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)));
        await Task.Delay(2000);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {response}");
        Assert.Equal(value, ((CacheDictionaryGetFieldResponse.Hit)response).ValueString);
    }

    [Fact]
    public async Task DictionarySetFieldsAsync_NullChecksFieldsAreStringValuesAreByteArray_IsError()
    {
        var dictionaryName = Utils.NewGuidString();
        var dictionary = new Dictionary<string, string>();
        CacheDictionarySetFieldsResponse response = await client.DictionarySetFieldsAsync(null!, dictionaryName, dictionary);
        Assert.True(response is CacheDictionarySetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionarySetFieldsAsync(cacheName, null!, dictionary);
        Assert.True(response is CacheDictionarySetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionarySetFieldsAsync(cacheName, dictionaryName, (IEnumerable<KeyValuePair<string, byte[]>>)null!);
        Assert.True(response is CacheDictionarySetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldsResponse.Error)response).ErrorCode);

        dictionary[Utils.NewGuidString()] = null!;
        response = await client.DictionarySetFieldsAsync(cacheName, dictionaryName, dictionary);
        Assert.True(response is CacheDictionarySetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionarySetFieldsResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionarySetFieldsAsync_FieldsAreStringValuesAreByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidByteArray();

        var items = new Dictionary<string, byte[]>() { { field1, value1 }, { field2, value2 } };

        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, items);

        CacheDictionaryGetFieldResponse getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field1);
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
        Assert.Equal(value1, ((CacheDictionaryGetFieldResponse.Hit)getResponse).ValueByteArray);

        getResponse = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field2);
        Assert.True(getResponse is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {getResponse}");
        Assert.Equal(value2, ((CacheDictionaryGetFieldResponse.Hit)getResponse).ValueByteArray);
    }

    [Fact]
    public async Task DictionarySetFieldsAsync_FieldsAreStringValuesAreByteArray_NoRefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();
        var content = new Dictionary<string, byte[]>() { { field, value } };

        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.InitialRefreshTtl)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(Utils.WaitForItemToBeSet);

        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)).WithNoRefreshTtlOnUpdates());
        await Task.Delay(Utils.WaitForInitialItemToExpire);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionarySetFieldsAsync_FieldsAreStringValuesAreByteArray_RefreshTtl()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidString();
        var value = Utils.NewGuidByteArray();
        var content = new Dictionary<string, byte[]>() { { field, value } };

        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)));
        await client.DictionarySetFieldsAsync(cacheName, dictionaryName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)));
        await Task.Delay(2000);

        CacheDictionaryGetFieldResponse response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {response}");
        Assert.Equal(value, ((CacheDictionaryGetFieldResponse.Hit)response).ValueByteArray);
    }

    [Fact]
    public async Task DictionaryGetFieldsAsync_NullChecksFieldsAreByteArray_IsError()
    {
        var dictionaryName = Utils.NewGuidString();
        var testData = new byte[][][] { new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() }, new byte[][] { Utils.NewGuidByteArray(), null! } };

        CacheDictionaryGetFieldsResponse response = await client.DictionaryGetFieldsAsync(null!, dictionaryName, testData[0]);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryGetFieldsAsync(cacheName, null!, testData[0]);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryGetFieldsAsync(cacheName, dictionaryName, (byte[][])null!);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryGetFieldsAsync(cacheName, dictionaryName, testData[1]);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);

        var fieldsList = new List<byte[]>(testData[0]);
        response = await client.DictionaryGetFieldsAsync(null!, dictionaryName, fieldsList);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryGetFieldsAsync(cacheName, null!, fieldsList);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryGetFieldsAsync(cacheName, dictionaryName, (List<byte[]>)null!);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryGetFieldsAsync(cacheName, dictionaryName, new List<byte[]>(testData[1]));
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionaryGetFieldsAsync_FieldsAreByteArrayValuesAreByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();
        var field3 = Utils.NewGuidByteArray();

        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field1, value1);
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field2, value2);

        CacheDictionaryGetFieldsResponse response = await client.DictionaryGetFieldsAsync(cacheName, dictionaryName, new byte[][] { field1, field2, field3 });
        Assert.True(response is CacheDictionaryGetFieldsResponse.Hit, $"Unexpected response: {response}");

        var hit = (CacheDictionaryGetFieldsResponse.Hit)response;
        Assert.Equal(3, hit.Responses.Count);
        Assert.True(hit.Responses[0] is CacheDictionaryGetFieldResponse.Hit, $"should be hit but was {hit.Responses[0]}");
        var hit1Response = (CacheDictionaryGetFieldResponse.Hit)hit.Responses[0];
        Assert.Equal(value1, hit1Response.ValueByteArray);
        Assert.Equal(field1, hit1Response.FieldByteArray);
        Assert.True(hit.Responses[1] is CacheDictionaryGetFieldResponse.Hit, $"should be hit but was {hit.Responses[1]}");
        var hit2Response = (CacheDictionaryGetFieldResponse.Hit)hit.Responses[1];
        Assert.Equal(value2, hit2Response.ValueByteArray);
        Assert.Equal(field2, hit2Response.FieldByteArray);
        Assert.True(hit.Responses[2] is CacheDictionaryGetFieldResponse.Miss, $"should be miss but was {hit.Responses[2]}");
        var missResponse = (CacheDictionaryGetFieldResponse.Miss)hit.Responses[2];
        Assert.Equal(field3, missResponse.FieldByteArray);

        var expectedDictionary = new Dictionary<byte[], byte[]>() { { field1, value1 }, { field2, value2 } };
        Assert.True(hit.ValueDictionaryByteArrayByteArray.DictionaryEquals(expectedDictionary), "dictionaries did not match");
    }

    [Fact]
    public async Task DictionaryGetFieldsAsync_DictionaryMissing_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var field3 = Utils.NewGuidByteArray();

        CacheDictionaryGetFieldsResponse response = await client.DictionaryGetFieldsAsync(cacheName, dictionaryName, new byte[][] { field1, field2, field3 });
        Assert.True(response is CacheDictionaryGetFieldsResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionaryGetFieldsAsync_NullChecksFieldsAreString_IsError()
    {
        var dictionaryName = Utils.NewGuidString();
        var testData = new string[][] { new string[] { Utils.NewGuidString(), Utils.NewGuidString() }, new string[] { Utils.NewGuidString(), null! } };
        CacheDictionaryGetFieldsResponse response = await client.DictionaryGetFieldsAsync(null!, dictionaryName, testData[0]);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryGetFieldsAsync(cacheName, null!, testData[0]);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryGetFieldsAsync(cacheName, dictionaryName, (string[])null!);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryGetFieldsAsync(cacheName, dictionaryName, testData[1]);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);

        var fieldsList = new List<string>(testData[0]);
        response = await client.DictionaryGetFieldsAsync(null!, dictionaryName, fieldsList);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryGetFieldsAsync(cacheName, null!, fieldsList);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryGetFieldsAsync(cacheName, dictionaryName, (List<string>)null!);
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryGetFieldsAsync(cacheName, dictionaryName, new List<string>(testData[1]));
        Assert.True(response is CacheDictionaryGetFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryGetFieldsResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionaryGetFieldsAsync_FieldsAreString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();
        var field3 = Utils.NewGuidString();

        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field1, value1);
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field2, value2);

        CacheDictionaryGetFieldsResponse response = await client.DictionaryGetFieldsAsync(cacheName, dictionaryName, new string[] { field1, field2, field3 });
        Assert.True(response is CacheDictionaryGetFieldsResponse.Hit, $"Unexpected response: {response}");

        var hit = (CacheDictionaryGetFieldsResponse.Hit)response;
        Assert.Equal(3, hit.Responses.Count);
        Assert.True(hit.Responses[0] is CacheDictionaryGetFieldResponse.Hit, $"should be hit but was {hit.Responses[0]}");
        var hit1Response = (CacheDictionaryGetFieldResponse.Hit)hit.Responses[0];
        Assert.Equal(value1, hit1Response.ValueString);
        Assert.Equal(field1, hit1Response.FieldString);
        Assert.True(hit.Responses[1] is CacheDictionaryGetFieldResponse.Hit, $"should be hit but was {hit.Responses[1]}");
        var hit2Response = (CacheDictionaryGetFieldResponse.Hit)hit.Responses[1];
        Assert.Equal(value2, hit2Response.ValueString);
        Assert.Equal(field2, hit2Response.FieldString);
        Assert.True(hit.Responses[2] is CacheDictionaryGetFieldResponse.Miss, $"should be miss but was {hit.Responses[2]}");
        var missResponse = (CacheDictionaryGetFieldResponse.Miss)hit.Responses[2];
        Assert.Equal(field3, missResponse.FieldString);

        var expectedDictionary = new Dictionary<string, string> { { field1, value1 }, { field2, value2 } };
        Assert.Equal(expectedDictionary, hit.ValueDictionaryStringString);

        var otherDictionary = hit.ValueDictionaryStringByteArray;
        Assert.Equal(2, otherDictionary.Count);
        Assert.Equal(otherDictionary[field1], Internal.Utils.Utf8ToByteArray(value1));
        Assert.Equal(otherDictionary[field2], Internal.Utils.Utf8ToByteArray(value2));
    }

    [Fact]
    public async Task CacheDictionaryGetFieldsResponse_ToString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, "a", "b");
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, "c", "d");

        CacheDictionaryGetFieldsResponse getResponse = await client.DictionaryGetFieldsAsync(cacheName, dictionaryName, new string[] { "a", "c" });

        Assert.True(getResponse is CacheDictionaryGetFieldsResponse.Hit, $"Unexpected response: {getResponse}");
        var hitResponse = (CacheDictionaryGetFieldsResponse.Hit)getResponse;
        Assert.Equal("Momento.Sdk.Responses.CacheDictionaryGetFieldsResponse+Hit: ValueDictionaryStringString: {\"a\": \"b\", \"c\": \"d\"} ValueDictionaryByteArrayByteArray: {\"61\": \"6...63\": \"64\"}", hitResponse.ToString());
    }

    [Theory]
    [InlineData(null, "my-dictionary")]
    [InlineData("cache", null)]
    public async Task DictionaryFetchAsync_NullChecks_IsError(string cacheName, string dictionaryName)
    {
        CacheDictionaryFetchResponse response = await client.DictionaryFetchAsync(cacheName, dictionaryName);
        Assert.True(response is CacheDictionaryFetchResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryFetchResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionaryFetchAsync_Missing_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        CacheDictionaryFetchResponse response = await client.DictionaryFetchAsync(cacheName, dictionaryName);
        Assert.True(response is CacheDictionaryFetchResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionaryFetchAsync_HasContentStringString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidString();
        var contentDictionary = new Dictionary<string, string>() {
            {field1, value1},
            {field2, value2}
        };

        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field1, value1);
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field2, value2);

        CacheDictionaryFetchResponse fetchResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);

        Assert.True(fetchResponse is CacheDictionaryFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheDictionaryFetchResponse.Hit)fetchResponse;
        Assert.Equal(hitResponse.ValueDictionaryStringString, contentDictionary);

        // Test field caching behavior
        Assert.Same(hitResponse.ValueDictionaryStringString, hitResponse.ValueDictionaryStringString);
    }

    [Fact]
    public async Task CacheDictionaryFetchResponse_ToString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, "a", "b");

        CacheDictionaryFetchResponse fetchResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);

        Assert.True(fetchResponse is CacheDictionaryFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheDictionaryFetchResponse.Hit)fetchResponse;
        Assert.Equal("Momento.Sdk.Responses.CacheDictionaryFetchResponse+Hit: ValueDictionaryStringString: {\"a\": \"b\"} ValueDictionaryByteArrayByteArray: {\"61\": \"62\"}", hitResponse.ToString());
    }

    [Fact]
    public async Task DictionaryFetchAsync_HasContentStringByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidString();
        var value2 = Utils.NewGuidByteArray();
        var contentDictionary = new Dictionary<string, byte[]>() {
            {field1, value1},
            {field2, value2}
        };

        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field1, value1);
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field2, value2);

        CacheDictionaryFetchResponse fetchResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);

        Assert.True(fetchResponse is CacheDictionaryFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheDictionaryFetchResponse.Hit)fetchResponse;
        Assert.Equal(hitResponse.ValueDictionaryStringByteArray, contentDictionary);

        // Test field caching behavior
        Assert.Same(hitResponse.ValueDictionaryStringByteArray, hitResponse.ValueDictionaryStringByteArray);
    }

    [Fact]
    public async Task DictionaryFetchAsync_HasContentByteArrayByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();
        var value2 = Utils.NewGuidByteArray();
        var contentDictionary = new Dictionary<byte[], byte[]>() {
            {field1, value1},
            {field2, value2}
        };

        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field1, value1);
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field2, value2);

        CacheDictionaryFetchResponse fetchResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);

        Assert.True(fetchResponse is CacheDictionaryFetchResponse.Hit, $"Unexpected response: {fetchResponse}");

        var hitResponse = (CacheDictionaryFetchResponse.Hit)fetchResponse;
        // Exercise byte array dictionary structural equality comparer
        Assert.True(hitResponse.ValueDictionaryByteArrayByteArray!.ContainsKey(field1), $"Could not find key {field1} in dictionary byte array: {hitResponse.ValueDictionaryByteArrayByteArray!}");
        Assert.True(hitResponse.ValueDictionaryByteArrayByteArray!.ContainsKey(field2), $"Could not find key {field2} in dictionary byte array: {hitResponse.ValueDictionaryByteArrayByteArray!}");
        Assert.Equal(2, hitResponse.ValueDictionaryByteArrayByteArray!.Count);

        // Exercise DictionaryEquals extension
        Assert.True(hitResponse.ValueDictionaryByteArrayByteArray!.DictionaryEquals(contentDictionary), $"Expected DictionaryEquals to return true for these dictionaries: {hitResponse.ValueDictionaryByteArrayByteArray!} AND {contentDictionary}");

        // Test field caching behavior
        Assert.Same(hitResponse.ValueDictionaryByteArrayByteArray, hitResponse.ValueDictionaryByteArrayByteArray);
    }

    [Fact]
    public async Task DictionaryDeleteAsync_DictionaryDoesNotExist_Noop()
    {
        var dictionaryName = Utils.NewGuidString();
        var response = await client.DictionaryFetchAsync(cacheName, dictionaryName);
        Assert.True(response is CacheDictionaryFetchResponse.Miss, $"Unexpected response: {response}");
        var deleteResponse = await client.DeleteAsync(cacheName, dictionaryName);
        Assert.True(deleteResponse is CacheDeleteResponse.Success, $"Unexpected response: {deleteResponse}");
        var fetchResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);
        Assert.True(fetchResponse is CacheDictionaryFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Fact]
    public async Task DictionaryDeleteAsync_DictionaryExists_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, Utils.NewGuidString(), Utils.NewGuidString());
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, Utils.NewGuidString(), Utils.NewGuidString());
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, Utils.NewGuidString(), Utils.NewGuidString());

        var fetchResponse = await client.DictionaryFetchAsync(cacheName, dictionaryName);
        Assert.True(fetchResponse is CacheDictionaryFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var deleteResponse = await client.DeleteAsync(cacheName, dictionaryName);
        Assert.True(deleteResponse is CacheDeleteResponse.Success, $"Unexpected response: {deleteResponse}");
        var response = await client.DictionaryFetchAsync(cacheName, dictionaryName);
        Assert.True(response is CacheDictionaryFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Theory]
    [InlineData(null, "my-dictionary", new byte[] { 0x00 })]
    [InlineData("my-cache", null, new byte[] { 0x00 })]
    [InlineData("my-cache", "my-dictionary", null)]
    public async Task DictionaryRemoveFieldAsync_NullChecksFieldIsByteArray_IsError(string cacheName, string dictionaryName, byte[] field)
    {
        CacheDictionaryRemoveFieldResponse response = await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field);
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldResponse.Error)response).ErrorCode);
    }

    [Theory]
    [InlineData(null, "my-dictionary", "my-field")]
    [InlineData("my-cache", null, "my-field")]
    [InlineData("my-cache", "my-dictionary", null)]
    public async Task DictionaryRemoveFieldAsync_NullChecksFieldIsString_IsError(string cacheName, string dictionaryName, string field)
    {
        CacheDictionaryRemoveFieldResponse response = await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field);
        Assert.True(response is CacheDictionaryRemoveFieldResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionaryRemoveFieldAsync_FieldIsByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidByteArray();
        var value1 = Utils.NewGuidByteArray();
        var field2 = Utils.NewGuidByteArray();

        // Add a field then delete it
        var response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field1);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field1, value1);
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field1);
        Assert.True(response is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {response}");

        await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field1);
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field1);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");

        // Test no-op
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field2);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
        await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field2);
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field2);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionaryRemoveFieldAsync_FieldIsString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field1 = Utils.NewGuidString();
        var value1 = Utils.NewGuidString();
        var field2 = Utils.NewGuidString();

        // Add a field then delete it
        var response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field1);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field1, value1);
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field1);
        Assert.True(response is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {response}");

        await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field1);
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field1);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");

        // Test no-op
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field2);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
        await client.DictionaryRemoveFieldAsync(cacheName, dictionaryName, field2);
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, field2);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionaryRemoveFieldsAsync_NullChecksFieldsAreByteArray_IsError()
    {
        var dictionaryName = Utils.NewGuidString();
        var testData = new byte[][][] { new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() }, new byte[][] { Utils.NewGuidByteArray(), null! } };

        CacheDictionaryRemoveFieldsResponse response = await client.DictionaryRemoveFieldsAsync(null!, dictionaryName, testData[0]);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryRemoveFieldsAsync(cacheName, null!, testData[0]);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, (byte[][])null!);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, testData[1]);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);

        var fieldsList = new List<byte[]>(testData[0]);
        response = await client.DictionaryRemoveFieldsAsync(null!, dictionaryName, fieldsList);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryRemoveFieldsAsync(cacheName, null!, fieldsList);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, (List<byte[]>)null!);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, new List<byte[]>(testData[1]));
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionaryRemoveFieldsAsync_FieldsAreByteArray_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var fields = new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };
        var otherField = Utils.NewGuidByteArray();

        // Test enumerable
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, fields[0], Utils.NewGuidByteArray());
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, fields[1], Utils.NewGuidByteArray());
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, otherField, Utils.NewGuidByteArray());

        var fieldsList = new List<byte[]>(fields);
        await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fieldsList);
        var response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, fields[0]);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, fields[1]);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, otherField);
        Assert.True(response is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionaryRemoveFieldsAsync_NullChecksFieldsAreString_IsError()
    {
        var dictionaryName = Utils.NewGuidString();
        var testData = new string[][] { new string[] { Utils.NewGuidString(), Utils.NewGuidString() }, new string[] { Utils.NewGuidString(), null! } };
        CacheDictionaryRemoveFieldsResponse response = await client.DictionaryRemoveFieldsAsync(null!, dictionaryName, testData[0]);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryRemoveFieldsAsync(cacheName, null!, testData[0]);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, (string[])null!);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, testData[1]);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);

        var fieldsList = new List<string>(testData[0]);
        response = await client.DictionaryRemoveFieldsAsync(null!, dictionaryName, fieldsList);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryRemoveFieldsAsync(cacheName, null!, fieldsList);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, (List<string>)null!);
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
        response = await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, new List<string>(testData[1]));
        Assert.True(response is CacheDictionaryRemoveFieldsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheDictionaryRemoveFieldsResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task DictionaryRemoveFieldsAsync_FieldsAreString_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var fields = new string[] { Utils.NewGuidString(), Utils.NewGuidString() };
        var otherField = Utils.NewGuidString();

        // Test enumerable
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, fields[0], Utils.NewGuidString());
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, fields[1], Utils.NewGuidString());
        await client.DictionarySetFieldAsync(cacheName, dictionaryName, otherField, Utils.NewGuidString());

        var fieldsList = new List<string>(fields);
        await client.DictionaryRemoveFieldsAsync(cacheName, dictionaryName, fieldsList);
        var response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, fields[0]);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, fields[1]);
        Assert.True(response is CacheDictionaryGetFieldResponse.Miss, $"Unexpected response: {response}");
        response = await client.DictionaryGetFieldAsync(cacheName, dictionaryName, otherField);
        Assert.True(response is CacheDictionaryGetFieldResponse.Hit, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionaryLengthAsync_DictionaryIsMissing()
    {
        var dictionaryName = Utils.NewGuidString();
        CacheDictionaryLengthResponse response = await client.DictionaryLengthAsync(cacheName, dictionaryName);
        Assert.True(response is CacheDictionaryLengthResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task DictionaryLengthAsync_HappyPath()
    {
        var dictionaryName = Utils.NewGuidString();
        var field = Utils.NewGuidByteArray();
        var value = Utils.NewGuidByteArray();

        await client.DictionarySetFieldAsync(cacheName, dictionaryName, field, value);

        CacheDictionaryLengthResponse lengthResponse = await client.DictionaryLengthAsync(cacheName, dictionaryName);
        Assert.True(lengthResponse is CacheDictionaryLengthResponse.Hit, $"Unexpected response: {lengthResponse}");
        var hitResponse = (CacheDictionaryLengthResponse.Hit)lengthResponse;
        Assert.Equal(1, hitResponse.Length);
    }
}
