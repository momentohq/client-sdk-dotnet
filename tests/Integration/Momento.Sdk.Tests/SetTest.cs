using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momento.Sdk.Requests;
using Momento.Sdk.Responses;
using Momento.Sdk.Tests;
using Xunit.Abstractions;

namespace Momento.Sdk.Tests;

[Collection("CacheClient")]
public class SetTest : TestBase
{
    private readonly ITestOutputHelper testOutputHelper;

    public SetTest(CacheClientFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(null, "my-set", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-set", null)]
    public async Task SetAddElementAsync_NullChecksByteArray_IsError(string cacheName, string setName, byte[] element)
    {
        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, element);
        Assert.True(response is CacheSetAddElementResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetAddElementResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task SetAddFetch_ElementIsByteArray_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();

        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, element);
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");

        var set = ((CacheSetFetchResponse.Hit)fetchResponse).ValueSetByteArray;
        Assert.Single(set);
        Assert.Contains(element, set);
    }

    [Fact]
    public async Task SetAddFetch_ElementIsByteArray_noRefreshTtlOnUpdates()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();

        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, element, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.InitialRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        await Task.Delay(Utils.WaitForItemToBeSet);

        response = await client.SetAddElementAsync(cacheName, setName, element, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        await Task.Delay(Utils.WaitForInitialItemToExpire);
        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Fact]
    public async Task SetAddFetch_ElementIsByteArray_RefreshTtl()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();

        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, element, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)).WithNoRefreshTtlOnUpdates());
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        await client.SetAddElementAsync(cacheName, setName, element, CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)));
        await Task.Delay(2000);

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        Assert.Single(((CacheSetFetchResponse.Hit)fetchResponse).ValueSetByteArray);
    }

    [Theory]
    [InlineData(null, "my-set", "my-element")]
    [InlineData("cache", null, "my-element")]
    [InlineData("cache", "my-set", null)]
    public async Task SetAddElementAsync_NullChecksString_IsError(string cacheName, string setName, string element)
    {
        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, element);
        Assert.True(response is CacheSetAddElementResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetAddElementResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task SetAddFetch_ElementIsString_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();

        CacheSetAddElementResponse respose = await client.SetAddElementAsync(cacheName, setName, element);
        Assert.True(respose is CacheSetAddElementResponse.Success, $"Unexpected response: {respose}");

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");

        var set = ((CacheSetFetchResponse.Hit)fetchResponse).ValueSetString;
        Assert.Single(set);
        Assert.Contains(element, set);
    }

    [Fact]
    public async Task SetAddFetch_ElementIsString_noRefreshTtlOnUpdates()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();

        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, element, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.InitialRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        await Task.Delay(Utils.WaitForItemToBeSet);

        response = await client.SetAddElementAsync(cacheName, setName, element, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        await Task.Delay(Utils.WaitForInitialItemToExpire);

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Fact]
    public async Task SetAddFetch_ElementIsString_RefreshTtl()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();

        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, element, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)).WithNoRefreshTtlOnUpdates());
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        response = await client.SetAddElementAsync(cacheName, setName, element, CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)));
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        await Task.Delay(2000);

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        Assert.Single(((CacheSetFetchResponse.Hit)fetchResponse).ValueSetString);
    }

    [Fact]
    public async Task SetAddElementsAsync_NullChecksByteArray_IsError()
    {
        var setName = Utils.NewGuidString();
        var set = new HashSet<byte[]>();
        CacheSetAddElementsResponse response = await client.SetAddElementsAsync(null!, setName, set);
        Assert.True(response is CacheSetAddElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetAddElementsResponse.Error)response).ErrorCode);
        response = await client.SetAddElementsAsync(cacheName, null!, set);
        Assert.True(response is CacheSetAddElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetAddElementsResponse.Error)response).ErrorCode);
        response = await client.SetAddElementsAsync(cacheName, setName, (IEnumerable<byte[]>)null!);
        Assert.True(response is CacheSetAddElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetAddElementsResponse.Error)response).ErrorCode);

        set.Add(null!);
        response = await client.SetAddElementsAsync(cacheName, setName, set);
        Assert.True(response is CacheSetAddElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetAddElementsResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task SetAddElementsAsync_ElementsAreByteArrayEnumerable_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element1 = Utils.NewGuidByteArray();
        var element2 = Utils.NewGuidByteArray();
        var content = new List<byte[]>() { element1, element2 };

        CacheSetAddElementsResponse response = await client.SetAddElementsAsync(cacheName, setName, content);
        Assert.True(response is CacheSetAddElementsResponse.Success, $"Unexpected response: {response}");

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");

        var set = ((CacheSetFetchResponse.Hit)fetchResponse).ValueSetByteArray;
        Assert.Equal(2, set!.Count);
        Assert.Contains(element1, set);
        Assert.Contains(element2, set);
    }

    [Fact]
    public async Task SetAddElementsAsync_ElementsAreByteArrayEnumerable_noRefreshTtlOnUpdates()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();
        var content = new List<byte[]>() { element };

        CacheSetAddElementsResponse response = await client.SetAddElementsAsync(cacheName, setName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.InitialRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(response is CacheSetAddElementsResponse.Success, $"Unexpected response: {response}");
        await Task.Delay(Utils.WaitForItemToBeSet);

        response = await client.SetAddElementsAsync(cacheName, setName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(response is CacheSetAddElementsResponse.Success, $"Unexpected response: {response}");
        await Task.Delay(Utils.WaitForInitialItemToExpire);

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Fact]
    public async Task SetAddElementsAsync_ElementsAreByteArrayEnumerable_RefreshTtl()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();
        var content = new List<byte[]>() { element };

        CacheSetAddElementsResponse response = await client.SetAddElementsAsync(cacheName, setName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)).WithNoRefreshTtlOnUpdates());
        Assert.True(response is CacheSetAddElementsResponse.Success, $"Unexpected response: {response}");
        await client.SetAddElementsAsync(cacheName, setName, content, CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)));
        await Task.Delay(2000);

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");

        var set = ((CacheSetFetchResponse.Hit)fetchResponse).ValueSetByteArray;
        Assert.Single(set);
        Assert.Contains(element, set);
    }

    [Fact]
    public async Task SetAddElementsAsync_NullChecksString_IsError()
    {
        var setName = Utils.NewGuidString();
        var set = new HashSet<string>();
        CacheSetAddElementsResponse response = await client.SetAddElementsAsync(null!, setName, set);
        Assert.True(response is CacheSetAddElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetAddElementsResponse.Error)response).ErrorCode);
        response = await client.SetAddElementsAsync(cacheName, null!, set);
        Assert.True(response is CacheSetAddElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetAddElementsResponse.Error)response).ErrorCode);
        response = await client.SetAddElementsAsync(cacheName, setName, (IEnumerable<string>)null!);
        Assert.True(response is CacheSetAddElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetAddElementsResponse.Error)response).ErrorCode);

        set.Add(null!);
        response = await client.SetAddElementsAsync(cacheName, setName, set);
        Assert.True(response is CacheSetAddElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetAddElementsResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task SetAddElementsAsync_ElementsAreStringEnumerable_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element1 = Utils.NewGuidString();
        var element2 = Utils.NewGuidString();
        var content = new List<string>() { element1, element2 };

        CacheSetAddElementsResponse response = await client.SetAddElementsAsync(cacheName, setName, content);
        Assert.True(response is CacheSetAddElementsResponse.Success, $"Unexpected response: {response}");

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");

        var set = ((CacheSetFetchResponse.Hit)fetchResponse).ValueSetString;
        Assert.Equal(2, set!.Count);
        Assert.Contains(element1, set);
        Assert.Contains(element2, set);
    }

    [Fact]
    public async Task SetAddElementsAsync_ElementsAreStringEnumerable_noRefreshTtlOnUpdates()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();
        var content = new List<string>() { element };

        CacheSetAddElementsResponse response = await client.SetAddElementsAsync(cacheName, setName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.InitialRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(response is CacheSetAddElementsResponse.Success, $"Unexpected response: {response}");
        await Task.Delay(Utils.WaitForItemToBeSet);

        response = await client.SetAddElementsAsync(cacheName, setName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)).WithNoRefreshTtlOnUpdates());
        Assert.True(response is CacheSetAddElementsResponse.Success, $"Unexpected response: {response}");
        await Task.Delay(Utils.WaitForInitialItemToExpire);

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Fact]
    public async Task SetAddElementsAsync_ElementsAreStringEnumerable_RefreshTtl()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();
        var content = new List<string>() { element };

        CacheSetAddElementsResponse response = await client.SetAddElementsAsync(cacheName, setName, content, ttl: CollectionTtl.Of(TimeSpan.FromSeconds(2)).WithNoRefreshTtlOnUpdates());
        Assert.True(response is CacheSetAddElementsResponse.Success, $"Unexpected response: {response}");
        response = await client.SetAddElementsAsync(cacheName, setName, content, CollectionTtl.Of(TimeSpan.FromSeconds(Utils.UpdatedRefreshTtl)));
        Assert.True(response is CacheSetAddElementsResponse.Success, $"Unexpected response: {response}");
        await Task.Delay(2000);

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");

        var set = ((CacheSetFetchResponse.Hit)fetchResponse).ValueSetString;
        Assert.Single(set);
        Assert.Contains(element, set);
    }

    [Theory]
    [InlineData(null, "my-set", new byte[] { 0x00 })]
    [InlineData("cache", null, new byte[] { 0x00 })]
    [InlineData("cache", "my-set", null)]
    public async Task SetRemoveElementAsync_NullChecksByteArray_IsError(string cacheName, string setName, byte[] element)
    {
        CacheSetRemoveElementResponse response = await client.SetRemoveElementAsync(cacheName, setName, element);
        Assert.True(response is CacheSetRemoveElementResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task SetRemoveElementAsync_ElementIsByteArray_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();

        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, element);
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");

        // Remove element that is not there -- no-op
        CacheSetRemoveElementResponse removeResponse = await client.SetRemoveElementAsync(cacheName, setName, Utils.NewGuidByteArray());
        Assert.True(removeResponse is CacheSetRemoveElementResponse.Success, $"Unexpected response: {removeResponse}");
        // Fetch the whole set and make sure response has element we expect
        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var set = ((CacheSetFetchResponse.Hit)fetchResponse).ValueSetByteArray;
        Assert.Single(set);
        Assert.Contains(element, set);

        // Remove element
        removeResponse = await client.SetRemoveElementAsync(cacheName, setName, element);
        Assert.True(removeResponse is CacheSetRemoveElementResponse.Success, $"Unexpected response: {removeResponse}");
        fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Fact]
    public async Task SetRemoveElementAsync_SetIsMissingElementIsByteArray_Noop()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();

        // Pre-condition: set is missing
        var fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");

        // Remove element that is not there -- no-op
        CacheSetRemoveElementResponse removeResponse = await client.SetRemoveElementAsync(cacheName, setName, Utils.NewGuidByteArray());
        Assert.True(removeResponse is CacheSetRemoveElementResponse.Success, $"Unexpected response: {removeResponse}");

        // Post-condition: set is still missing
        fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Theory]
    [InlineData(null, "my-set", "my-element")]
    [InlineData("cache", null, "my-element")]
    [InlineData("cache", "my-set", null)]
    public async Task SetRemoveElementAsync_NullChecksString_IsError(string cacheName, string setName, string element)
    {
        CacheSetRemoveElementResponse response = await client.SetRemoveElementAsync(cacheName, setName, element);
        Assert.True(response is CacheSetRemoveElementResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task SetRemoveElementAsync_ElementIsString_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();

        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, element);
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");

        // Remove element that is not there -- no-op
        CacheSetRemoveElementResponse removeResponse = await client.SetRemoveElementAsync(cacheName, setName, Utils.NewGuidString());
        Assert.True(removeResponse is CacheSetRemoveElementResponse.Success, $"Unexpected response: {removeResponse}");
        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var set = ((CacheSetFetchResponse.Hit)fetchResponse).ValueSetString;
        Assert.Single(set);
        Assert.Contains(element, set);

        // Remove element
        removeResponse = await client.SetRemoveElementAsync(cacheName, setName, element);
        Assert.True(removeResponse is CacheSetRemoveElementResponse.Success, $"Unexpected response: {removeResponse}");
        fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Fact]
    public async Task SetRemoveElementAsync_SetIsMissingElementIsString_Noop()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidString();

        // Pre-condition: set is missing
        var fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");

        // Remove element that is not there -- no-op
        CacheSetRemoveElementResponse response = await client.SetRemoveElementAsync(cacheName, setName, Utils.NewGuidString());
        Assert.True(response is CacheSetRemoveElementResponse.Success, $"Unexpected response: {response}");

        // Post-condition: set is still missing
        fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Fact]
    public async Task SetRemoveElementsAsync_NullChecksElementsAreByteArray_IsError()
    {
        var setName = Utils.NewGuidString();
        var testData = new byte[][][] { new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() }, new byte[][] { Utils.NewGuidByteArray(), null! } };

        CacheSetRemoveElementsResponse response = await client.SetRemoveElementsAsync(null!, setName, testData[0]);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
        response = await client.SetRemoveElementsAsync(cacheName, null!, testData[0]);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
        response = await client.SetRemoveElementsAsync(cacheName, setName, (byte[][])null!);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
        response = await client.SetRemoveElementsAsync(cacheName, setName, testData[1]);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);

        var fieldsList = new List<byte[]>(testData[0]);
        response = await client.SetRemoveElementsAsync(null!, setName, fieldsList);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
        response = await client.SetRemoveElementsAsync(cacheName, null!, fieldsList);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
        response = await client.SetRemoveElementsAsync(cacheName, setName, (List<byte[]>)null!);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
        response = await client.SetRemoveElementsAsync(cacheName, setName, new List<byte[]>(testData[1]));
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task SetRemoveElementsAsync_FieldsAreByteArray_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var elements = new byte[][] { Utils.NewGuidByteArray(), Utils.NewGuidByteArray() };
        var otherElement = Utils.NewGuidByteArray();

        // Test enumerable
        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, elements[0]);
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        await client.SetAddElementAsync(cacheName, setName, elements[1]);
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        await client.SetAddElementAsync(cacheName, setName, otherElement);
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");

        var elementsList = new List<byte[]>(elements);
        CacheSetRemoveElementsResponse removeResponse = await client.SetRemoveElementsAsync(cacheName, setName, elementsList);
        Assert.True(removeResponse is CacheSetRemoveElementsResponse.Success, $"Unexpected response: {removeResponse}");
        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheSetFetchResponse.Hit)fetchResponse;
        Assert.Single(hitResponse.ValueSetByteArray!);
        Assert.Contains(otherElement, hitResponse.ValueSetByteArray!);
    }

    [Fact]
    public async Task SetRemoveElementsAsync_NullChecksElementsAreString_IsError()
    {
        var setName = Utils.NewGuidString();
        var testData = new string[][] { new string[] { Utils.NewGuidString(), Utils.NewGuidString() }, new string[] { Utils.NewGuidString(), null! } };

        CacheSetRemoveElementsResponse response = await client.SetRemoveElementsAsync(null!, setName, testData[0]);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
        response = await client.SetRemoveElementsAsync(cacheName, null!, testData[0]);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
        response = await client.SetRemoveElementsAsync(cacheName, setName, (byte[][])null!);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
        response = await client.SetRemoveElementsAsync(cacheName, setName, testData[1]);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);

        var elementsList = new List<string>(testData[0]);
        response = await client.SetRemoveElementsAsync(null!, setName, elementsList);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
        response = await client.SetRemoveElementsAsync(cacheName, null!, elementsList);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
        response = await client.SetRemoveElementsAsync(cacheName, setName, (List<string>)null!);
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
        response = await client.SetRemoveElementsAsync(cacheName, setName, new List<string>(testData[1]));
        Assert.True(response is CacheSetRemoveElementsResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetRemoveElementsResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task SetRemoveElementsAsync_FieldsAreString_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var elements = new string[] { Utils.NewGuidString(), Utils.NewGuidString() };
        var otherElement = Utils.NewGuidByteArray();

        // Test enumerable
        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, elements[0]);
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        response = await client.SetAddElementAsync(cacheName, setName, elements[1]);
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        response = await client.SetAddElementAsync(cacheName, setName, otherElement);
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");

        var elementsList = new List<string>(elements);
        CacheSetRemoveElementsResponse removeResponse = await client.SetRemoveElementsAsync(cacheName, setName, elementsList);
        Assert.True(removeResponse is CacheSetRemoveElementsResponse.Success, $"Unexpected response: {removeResponse}");
        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheSetFetchResponse.Hit)fetchResponse;
        Assert.Single(hitResponse.ValueSetByteArray!);
        Assert.Contains(otherElement, hitResponse.ValueSetByteArray!);
    }

    [Theory]
    [InlineData(null, "my-set")]
    [InlineData("cache", null)]
    public async Task SetFetchAsync_NullChecks_IsError(string cacheName, string setName)
    {
        CacheSetFetchResponse response = await client.SetFetchAsync(cacheName, setName);
        Assert.True(response is CacheSetFetchResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetFetchResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task SetFetchAsync_Missing_HappyPath()
    {
        var setName = Utils.NewGuidString();
        CacheSetFetchResponse response = await client.SetFetchAsync(cacheName, setName);
        Assert.True(response is CacheSetFetchResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task SetFetchAsync_UsesCachedByteArraySet_HappyPath()
    {
        var setName = Utils.NewGuidString();
        CacheSetAddElementsResponse setResponse = await client.SetAddElementsAsync(cacheName, setName, new string[] { Utils.NewGuidString(), Utils.NewGuidString() });
        Assert.True(setResponse is CacheSetAddElementsResponse.Success, $"Unexpected response: {setResponse}");
        CacheSetFetchResponse response = await client.SetFetchAsync(cacheName, setName);
        Assert.True(response is CacheSetFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheSetFetchResponse.Hit)response;
        var set1 = hitResponse.ValueSetByteArray;
        var set2 = hitResponse.ValueSetByteArray;
        Assert.Same(set1, set2);
    }

    [Fact]
    public async Task SetFetchAsync_UsesCachedStringSet_HappyPath()
    {
        var setName = Utils.NewGuidString();
        CacheSetAddElementsResponse setResponse = await client.SetAddElementsAsync(cacheName, setName, new string[] { Utils.NewGuidString(), Utils.NewGuidString() });
        Assert.True(setResponse is CacheSetAddElementsResponse.Success, $"Unexpected response: {setResponse}");
        CacheSetFetchResponse response = await client.SetFetchAsync(cacheName, setName);
        Assert.True(response is CacheSetFetchResponse.Hit, $"Unexpected response: {response}");
        var hitResponse = (CacheSetFetchResponse.Hit)response;
        var set1 = hitResponse.ValueSetString;
        var set2 = hitResponse.ValueSetString;
        Assert.Same(set1, set2);
    }
    
    
    [Theory]
    [InlineData(null, "my-set")]
    [InlineData("cache", null)]
    public async Task SetSampleAsync_NullChecks_IsError(string cacheName, string setName)
    {
        CacheSetSampleResponse response = await client.SetSampleAsync(cacheName, setName, 100);
        Assert.True(response is CacheSetSampleResponse.Error, $"Unexpected response: {response}");
        Assert.Equal(MomentoErrorCode.INVALID_ARGUMENT_ERROR, ((CacheSetSampleResponse.Error)response).ErrorCode);
    }

    [Fact]
    public async Task SetSampleAsync_Missing_HappyPath()
    {
        var setName = Utils.NewGuidString();
        CacheSetSampleResponse response = await client.SetSampleAsync(cacheName, setName, 100);
        Assert.True(response is CacheSetSampleResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task SetSampleAsync_UsesCachedStringSet_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var allValues = new HashSet<String> { "jalapeno", "habanero", "serrano", "poblano" };
        CacheSetAddElementsResponse setResponse = await client.SetAddElementsAsync(cacheName, setName, allValues);
        Assert.True(setResponse is CacheSetAddElementsResponse.Success, $"Unexpected response: {setResponse}");

        CacheSetSampleResponse allElementsResponse = await client.SetSampleAsync(cacheName, setName, allValues.Count);
        Assert.True(allElementsResponse is CacheSetSampleResponse.Hit, $"Unexpected response: {allElementsResponse}");
        var allElementsHitValues = ((CacheSetSampleResponse.Hit)allElementsResponse).ValueSetString;
        Assert.True(allValues.SetEquals(allElementsHitValues), $"Expected sample with with limit matching set size to return the entire set; expected ({String.Join(", ", allValues)}), got ({String.Join(", ", allElementsHitValues)})");
        
        CacheSetSampleResponse limitGreaterThanSetSizeResponse = await client.SetSampleAsync(cacheName, setName, 1000);
        Assert.True(limitGreaterThanSetSizeResponse is CacheSetSampleResponse.Hit, $"Unexpected response: {limitGreaterThanSetSizeResponse}");
        var limitGreaterThanSetSizeHitValues = ((CacheSetSampleResponse.Hit)limitGreaterThanSetSizeResponse).ValueSetString;
        Assert.True(allValues.SetEquals(limitGreaterThanSetSizeHitValues), $"Expected sample with with limit greater than set size to return the entire set; expected ({String.Join(", ", allValues)}), got ({String.Join(", ", limitGreaterThanSetSizeHitValues)})");
        
        CacheSetSampleResponse limitZeroResponse = await client.SetSampleAsync(cacheName, setName, 0);
        // TODO: for now the server is returning a MISS for this. We will are updating that behavior and will need to fix this
        // test accordingly, but this is an edge case that we don't need to block the SDK release on so we can fix the test
        // once the server behavior changes.
        Assert.True(limitZeroResponse is CacheSetSampleResponse.Miss, $"Unexpected response: {limitZeroResponse}");
        // var limitZeroHitValues = ((CacheSetSampleResponse.Hit)limitZeroResponse).ValueSetString;
        // Assert.True(allValues.SetEquals(limitZeroHitValues), $"Expected sample with with limit zero to return the entire set; expected ({allValues}), got ({limitZeroHitValues})");
        
        for (int i = 0; i < 10; i++)
        {
            CacheSetSampleResponse response = await client.SetSampleAsync(cacheName, setName, allValues.Count - 2);
            Assert.True(response is CacheSetSampleResponse.Hit, $"Unexpected response: {response}");
            var hitResponse = (CacheSetSampleResponse.Hit)response;
            var hitValues = hitResponse.ValueSetString;
            Assert.True(hitValues.IsSubsetOf(allValues),
                $"Expected hit values ({String.Join(", ", hitValues)}) to be subset of all values ({String.Join(", ", allValues)}), but it is not!");
        }
    }
    

    [Fact]
    public async Task CacheSetFetchResponse_ToString_HappyPath()
    {
        var setName = Utils.NewGuidString();
        await client.SetAddElementAsync(cacheName, setName, "a");

        CacheSetFetchResponse fetchResponse = await client.SetFetchAsync(cacheName, setName);

        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var hitResponse = (CacheSetFetchResponse.Hit)fetchResponse;
        Assert.Equal("Momento.Sdk.Responses.CacheSetFetchResponse+Hit: ValueSetString: {\"a\"} ValueSetByteArray: {\"61\"}", hitResponse.ToString());
    }

    [Fact]
    public async Task SetDeleteAsync_SetDoesNotExist_Noop()
    {
        var setName = Utils.NewGuidString();
        var fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
        var response = await client.DeleteAsync(cacheName, setName);
        Assert.True(response is CacheDeleteResponse.Success, $"Unexpected response: {response}");
        fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }

    [Fact]
    public async Task SetDeleteAsync_SetExists_HappyPath()
    {
        var setName = Utils.NewGuidString();
        CacheSetAddElementResponse response = await client.SetAddElementAsync(cacheName, setName, Utils.NewGuidString());
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        response = await client.SetAddElementAsync(cacheName, setName, Utils.NewGuidString());
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");
        response = await client.SetAddElementAsync(cacheName, setName, Utils.NewGuidString());
        Assert.True(response is CacheSetAddElementResponse.Success, $"Unexpected response: {response}");

        var fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Hit, $"Unexpected response: {fetchResponse}");
        var deleteResponse = await client.DeleteAsync(cacheName, setName);
        Assert.True(deleteResponse is CacheDeleteResponse.Success, $"Unexpected response: {deleteResponse}");
        fetchResponse = await client.SetFetchAsync(cacheName, setName);
        Assert.True(fetchResponse is CacheSetFetchResponse.Miss, $"Unexpected response: {fetchResponse}");
    }
    
    [Fact]
    public async Task SetLengthAsync_SetIsMissing()
    {
        var setName = Utils.NewGuidString();
        CacheSetLengthResponse response = await client.SetLengthAsync(cacheName, setName);
        Assert.True(response is CacheSetLengthResponse.Miss, $"Unexpected response: {response}");
    }

    [Fact]
    public async Task SetLengthAsync_HappyPath()
    {
        var setName = Utils.NewGuidString();
        var element = Utils.NewGuidByteArray();

        await client.SetAddElementAsync(cacheName, setName, element);

        CacheSetLengthResponse lengthResponse = await client.SetLengthAsync(cacheName, setName);
        Assert.True(lengthResponse is CacheSetLengthResponse.Hit, $"Unexpected response: {lengthResponse}");
        var hitResponse = (CacheSetLengthResponse.Hit)lengthResponse;
        Assert.Equal(1, hitResponse.Length);
    }
}
