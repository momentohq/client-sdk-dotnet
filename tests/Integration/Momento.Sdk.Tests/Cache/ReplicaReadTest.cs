using System.Linq;
using System.Threading.Tasks;

namespace Momento.Sdk.Tests.Integration.Cache;

[Collection("CacheClient")]
public class ReplicaReadTest : TestBase
{
    private new readonly ICacheClient client;
    public ReplicaReadTest(CacheClientFixture fixture) : base(fixture)
    {
        client = fixture.ClientWithBalancedReads;
    }

    [Fact]
    public async Task LatestValueAfterReplicationDelay()
    {
        const int numTrials = 10;
        const int delayBetweenTrials = 100;
        const int replicationDelayMs = 1000;
        var random = new Random();

        var trials = Enumerable.Range(0, numTrials).Select(async trialNumber =>
        {
            var startDelay = (trialNumber + 1) * delayBetweenTrials + (random.NextDouble() - 0.5) * 10;
            await Task.Delay((int)startDelay);

            var key = Utils.NewGuidString();
            var value = Utils.NewGuidString();

            var setResponse = await client.SetAsync(cacheName, key, value);
            Assert.True(setResponse is CacheSetResponse.Success, $"Unexpected response: {setResponse}");

            await Task.Delay(replicationDelayMs);

            var getResponse = await client.GetAsync(cacheName, key);
            Assert.True(getResponse is CacheGetResponse.Hit, $"Unexpected response: {getResponse}");
            var hitResponse = (CacheGetResponse.Hit)getResponse;
            string setValue = hitResponse.ValueString;
            Assert.Equal(value, setValue);
        });

        await Task.WhenAll(trials);
    }
}
