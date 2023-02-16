using Microsoft.Extensions.Logging;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Requests;
using Momento.Sdk.Responses;

public class Driver
{
    private static readonly string AUTH_TOKEN_ENV_VAR = "MOMENTO_AUTH_TOKEN";
    private static readonly string CACHE_NAME_ENV_VAR = "MOMENTO_CACHE_NAME";
    private static readonly ILogger _logger;
    private static readonly ILoggerFactory _loggerFactory;

    static Driver()
    {
        _loggerFactory = InitializeLogging();
        _logger = _loggerFactory.CreateLogger<Driver>();
    }

    public async static Task Main()
    {
        var authToken = ReadAuthToken();
        var cacheName = ReadCacheName();

        // Set up the client
        using var client = new SimpleCacheClient(Configurations.Laptop.Latest(), authToken, TimeSpan.FromSeconds(60));
        await EnsureCacheExistsAsync(client, cacheName);

        // Set a value
        var dictionaryName = "my-dictionary";
        var setResponse = await client.DictionarySetFieldAsync(cacheName: cacheName, dictionaryName: dictionaryName,
            field: "my-field", value: "my-value", CollectionTtl.FromCacheTtl());
        if (setResponse is CacheDictionarySetFieldResponse.Error setError)
        {
            _logger.LogInformation($"Error setting a value in a dictionary: {setError.Message}");
            Environment.Exit(1);
        }

        // Set multiple values
        var setBatchResponse = await client.DictionarySetFieldsAsync(
            cacheName: cacheName,
            dictionaryName: dictionaryName,
            new Dictionary<string, string>() {
                { "field1", "value1" },
                { "field2", "value2" },
                { "field3", "value3" }},
            CollectionTtl.FromCacheTtl());
        if (setBatchResponse is CacheDictionarySetFieldsResponse.Error setBatchError)
        {
            _logger.LogInformation($"Error setting a values in a dictionary: {setBatchError.Message}");
            Environment.Exit(1);
        }

        // Get a value
        var field = "field1";
        var getResponse = await client.DictionaryGetFieldAsync(
            cacheName: cacheName,
            dictionaryName: dictionaryName,
            field: field);

        var status = "";
        var value = "";
        if (getResponse is CacheDictionaryGetFieldResponse.Hit unaryHit)
        {
            status = "HIT";
            value = unaryHit.ValueString;
        }
        else if (getResponse is CacheDictionaryGetFieldResponse.Miss)
        {
            // In this example you can get here if you:
            // - change the field name to one that does not exist, or if you
            // - set a short TTL, then add a Task.Delay so that it expires.
            status = "MISS";
            value = "<NONE; operation was a MISS>";
        }
        else if (getResponse is CacheDictionaryGetFieldResponse.Error getError)
        {
            _logger.LogInformation($"Error getting value from a dictionary: {getError.Message}");
            Environment.Exit(1);
        }

        _logger.LogInformation("");
        _logger.LogInformation($"Dictionary get of {field}: status={status}; value={value}");

        // Get multiple values
        var batchFieldList = new string[] { "field1", "field2", "field3", "field4" };
        var getBatchResponse = await client.DictionaryGetFieldsAsync(
            cacheName: cacheName,
            dictionaryName: dictionaryName,
            fields: batchFieldList);
        if (getBatchResponse is CacheDictionaryGetFieldsResponse.Hit hitResponse)
        {
            var dictionary = hitResponse.ValueDictionaryStringString;
            _logger.LogInformation("");
            _logger.LogInformation($"Accessing an entry of {dictionaryName} using a native Dictionary: {dictionary["field1"]}");

            _logger.LogInformation("");
            _logger.LogInformation("Displaying the results of dictionary get fields:");
            dictionary.ToList().ForEach(kv =>
                _logger.LogInformation($"- field={kv.Key}; value={kv.Value}"));

            // if you prefer to iterate over each field in the original request,
            // you can use the .Responses property
            _logger.LogInformation("");
            _logger.LogInformation("Displaying the result of dictionary get fields, one field at a time:");

            foreach (var response in hitResponse.Responses)
            {
                status = "MISS";
                value = "<NONE; field was a MISS>";
                if (response is CacheDictionaryGetFieldResponse.Hit hit)
                {
                    status = "HIT";
                    value = hit.ValueString;
                }
                _logger.LogInformation($"- field={response.FieldString}; status={status}; value={value}");
            }
        }
        else if (getBatchResponse is CacheDictionaryGetFieldsResponse.Error getBatchError)
        {
            _logger.LogInformation($"Error getting value from a dictionary: {getBatchError.Message}");
            Environment.Exit(1);
        }

        // Get the whole dictionary
        var fetchResponse = await client.DictionaryFetchAsync(
            cacheName: cacheName,
            dictionaryName: dictionaryName);
        if (fetchResponse is CacheDictionaryFetchResponse.Hit fetchHit)
        {
            var dictionary = fetchHit.ValueDictionaryStringString;
            _logger.LogInformation("");
            _logger.LogInformation($"Accessing an entry of {dictionaryName} using a native Dictionary: {dictionary["field1"]}");

            _logger.LogInformation("");
            _logger.LogInformation("Displaying the results of dictionary fetch:");
            dictionary.ToList().ForEach(kv =>
                _logger.LogInformation($"- field={kv.Key}; value={kv.Value}"));
        }
        else if (fetchResponse is CacheDictionaryFetchResponse.Miss fetchMiss)
        {
            // You can reach here by:
            // - fetching a dictionary that does not exist, eg changing the name above, or
            // - setting a short TTL and adding a Task.Delay so the dictionary expires
            _logger.LogInformation($"Expected {dictionaryName} to be a hit; got a miss.");
            Environment.Exit(1);
        }
        else if (fetchResponse is CacheDictionaryFetchResponse.Error fetchError)
        {
            _logger.LogInformation($"Error while fetching {dictionaryName}: {fetchError.Message}");
            Environment.Exit(1);
        }
    }

    private static ILoggerFactory InitializeLogging()
    {
        return LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "hh:mm:ss ";
            });
            builder.SetMinimumLevel(LogLevel.Information);
        });
    }

    private static ICredentialProvider ReadAuthToken()
    {
        try
        {
            return new EnvMomentoTokenProvider(AUTH_TOKEN_ENV_VAR);
        }
        catch (InvalidArgumentException)
        {
        }

        Console.Write($"Auth token not detected in environment variable {AUTH_TOKEN_ENV_VAR}. Enter auth token here: ");
        var authToken = Console.ReadLine()!.Trim();

        StringMomentoTokenProvider? authProvider = null;
        try
        {
            authProvider = new StringMomentoTokenProvider(authToken);
        }
        catch (InvalidArgumentException e)
        {
            _logger.LogInformation("{}", e);
            _loggerFactory.Dispose();
            Environment.Exit(1);
        }
        return authProvider!;
    }

    private static string ReadCacheName()
    {
        var cacheName = System.Environment.GetEnvironmentVariable(CACHE_NAME_ENV_VAR);
        if (cacheName is not null)
        {
            return cacheName;
        }
        return "default-cache";
    }

    private static async Task EnsureCacheExistsAsync(ISimpleCacheClient client, string cacheName)
    {
        _logger.LogInformation($"Creating cache {cacheName} if it doesn't already exist.");
        var createCacheResponse = await client.CreateCacheAsync(cacheName);
        if (createCacheResponse is CreateCacheResponse.Success)
        {
            _logger.LogInformation($"Created cache {cacheName}.");
        }
        else if (createCacheResponse is CreateCacheResponse.CacheAlreadyExists)
        {
            _logger.LogInformation($"Cache {cacheName} already exists.");
        }
        else if (createCacheResponse is CreateCacheResponse.Error error)
        {
            _logger.LogInformation($"Error creating cache: {error.Message}");
            Environment.Exit(1);
        }
    }
}
