using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using MomentoSdkDotnet45;

namespace MomentoApplicationPresignedUrl
{
    class Program
    {
        private static readonly string SIGNING_KEY = Environment.GetEnvironmentVariable("SIGNING_KEY");
        private static readonly string ENDPOINT = Environment.GetEnvironmentVariable("ENDPOINT");

        private static readonly uint URL_TTL_MINUTES = 10;
        private static readonly string CACHE_NAME = Environment.GetEnvironmentVariable("CACHE_NAME");
        private static readonly string OBJECT_KEY = "MyKey";
        private static readonly string OBJECT_VALUE = "MyData";
        private static readonly uint OBJECT_TTL_SECONDS = 60 * 60 * 24 * 14; // 14 days

        private static readonly HttpClient client = new HttpClient();

        private static async Task RunPresignedUrlExample(Uri setUri, Uri getUri)
        {
            Console.WriteLine($"Posting value with signed URL for {CacheOperation.SET}: {OBJECT_VALUE}");
            var data = new StringContent(OBJECT_VALUE, Encoding.UTF8, "application/json");
            var setResponse = await client.PostAsync(setUri, data);
            setResponse.EnsureSuccessStatusCode();

            var getResponse = await client.GetStringAsync(getUri);
            Console.WriteLine($"Retrieved value with signed URL for {CacheOperation.GET}: {getResponse}");
        }

        static async Task Main(string[] args)
        {
            if (String.IsNullOrEmpty(SIGNING_KEY))
            {
                Console.WriteLine($"Enviroment variable SIGNING_KEY not set");
                Environment.Exit(1);
            }
            if (String.IsNullOrEmpty(ENDPOINT))
            {
                Console.WriteLine($"Enviroment variable ENDPOINT not set");
                Environment.Exit(1);
            }

            var cacheName = CACHE_NAME;
            if (String.IsNullOrEmpty(CACHE_NAME))
            {
                cacheName = "default-cache";
                Console.WriteLine($"Enviroment variable CACHE_NAME not set. Using cache name {cacheName}.");
            }

            // prepare requests
            uint expiryEpochSeconds = (uint)DateTimeOffset.UtcNow.AddMinutes(URL_TTL_MINUTES).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var setReq = new SigningRequest(cacheName, OBJECT_KEY, CacheOperation.SET, expiryEpochSeconds) { TtlSeconds = OBJECT_TTL_SECONDS };
            var getReq = new SigningRequest(cacheName, OBJECT_KEY, CacheOperation.GET, expiryEpochSeconds);
            Console.WriteLine($"Request claims: exp = {expiryEpochSeconds}, cache = {cacheName}, key = {OBJECT_KEY}, ttl (for set) = {OBJECT_TTL_SECONDS}");

            // create presigned urls
            MomentoSigner signer = new MomentoSigner(SIGNING_KEY);
            var setUrl = signer.CreatePresignedUrl(ENDPOINT, setReq);
            var getUrl = signer.CreatePresignedUrl(ENDPOINT, getReq);

            Uri setUri, getUri;
            Uri.TryCreate(setUrl, UriKind.Absolute, out setUri);
            Console.WriteLine($"Signed URL for {CacheOperation.SET}:\n{setUri}");
            Uri.TryCreate(getUrl, UriKind.Absolute, out getUri);
            Console.WriteLine($"Signed URL for {CacheOperation.GET}:\n{getUri}");

            // set and get using presigned urls
            await RunPresignedUrlExample(setUri, getUri);
        }
    }
}