using System.Diagnostics;
using System.Text;
using Amazon.Lambda.Core;
using Google.Protobuf.WellKnownTypes;
using HdrHistogram;
using Microsoft.Extensions.Logging;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaMomentoHandler;

internal class CsharpLoadGeneratorContext
{
    public Stopwatch StartTime;
    public Recorder GetLatencies;
    //public Recorder SetLatencies;

    public int GlobalRequestCount;
    public int LastWorkerStatsPrintRequestCount;
    public int GlobalSuccessCount;
    public int GlobalUnavailableCount;
    public int GlobalTimeoutExceededCount;
    public int GlobalLimitExceededCount;
    public int GlobalRstStreamCount;

    public CsharpLoadGeneratorContext()
    {
        StartTime = System.Diagnostics.Stopwatch.StartNew();
        GetLatencies = HistogramFactory.With64BitBucketSize().WithValuesFrom(1).WithValuesUpTo(TimeStamp.Minutes(1)).WithPrecisionOf(1).WithThreadSafeWrites().WithThreadSafeReads().Create();
        //SetLatencies = HistogramFactory.With64BitBucketSize().WithValuesFrom(1).WithValuesUpTo(TimeStamp.Minutes(1)).WithPrecisionOf(1).WithThreadSafeWrites().WithThreadSafeReads().Create();

        GlobalRequestCount = 0;
        LastWorkerStatsPrintRequestCount = 0;
        GlobalSuccessCount = 0;
        GlobalTimeoutExceededCount = 0;
        GlobalLimitExceededCount = 0;
        GlobalRstStreamCount = 0;
        GlobalUnavailableCount = 0;
    }
};

enum AsyncGetResult
{
    SUCCESS,
    UNAVAILABLE,
    TIMEOUT,
    LIMIT_EXCEEDED,
    RST_STREAM
};

public class Function
{

    ICacheClient? momento = null;
    string CACHE_NAME = "MomentoLambda";
    int numInitialRequests = 10;
    ILogger? _logger;

    public async void FunctionHandler()
    {
        Console.WriteLine("Hi there!");
        TimeSpan DEFAULT_TTL = TimeSpan.FromSeconds(60);
        ICredentialProvider authProvider = new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN");
        IConfiguration clientConfig = Configurations.InRegion.Lambda.Latest().WithClientTimeout(TimeSpan.FromSeconds(15));
        if (momento == null)
        {
            Console.WriteLine("constructing client");
            momento = new CacheClient(
                clientConfig, authProvider, DEFAULT_TTL
            );
        }

        var _loggerFactory = clientConfig.LoggerFactory;
        _logger = _loggerFactory.CreateLogger("lambdaTest");

        var context = new CsharpLoadGeneratorContext();

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        var asyncTasks = Enumerable.Range(0, numInitialRequests).Select<int, Task>(workerId =>
            LaunchAndRunWorkers(
                momento,
                context,
                workerId + 1,
                cancellationTokenSource.Token
            )
        );
        //var statsPrinterTask = LaunchStatsPrinterTask(
        //    context,
        //    _options.showStatsInterval,
        //    cancellationTokenSource.Token
        //);
        //asyncTasks.Append(statsPrinterTask);

        //cancellationTokenSource.CancelAfter(_options.howLongToRun);

        // this will ensure that the program exits promptly if one of the async
        // tasks throws an uncaught exception.
        Console.WriteLine("awaiting");
        var firstResult = await Task.WhenAny(asyncTasks);
        await firstResult;
        Console.WriteLine("Got first result");

        await Task.WhenAll(asyncTasks);
        return;
    }

    private async Task LaunchAndRunWorkers(
        ICacheClient client,
        CsharpLoadGeneratorContext context,
        int workerId,
        CancellationToken token
    )
    {
        for (var i = 1; !token.IsCancellationRequested; i++)
        {
            await IssueAsyncGet(client, context, workerId, i);
        }
    }


    private async Task IssueAsyncGet(ICacheClient client, CsharpLoadGeneratorContext context, int workerId, int operationId)
    {
        var cacheKey = $"key-{workerId}";

        var getStartTime = System.Diagnostics.Stopwatch.StartNew();
        var getResponse = await ExecuteRequestAndUpdateContextCounts(
            context,
            () => IssueGetRequest(client, cacheKey)
        );

        if (getResponse is CacheGetResponse.Hit hitResponse)
        {
            var getDuration = getStartTime.ElapsedMilliseconds;
            Console.WriteLine(getDuration);
            context.GetLatencies.RecordValue(getDuration);
            // TODO: sleep for random 1-10 sec interval and issue and time another get
        }
    }

    private async Task<TResult> ExecuteRequestAndUpdateContextCounts<TResult>(
        CsharpLoadGeneratorContext context,
        Func<Task<Tuple<AsyncGetResult, TResult>>> block
    )
    {
        var result = await block();
        UpdateContextCountsForRequest(context, result.Item1);
        return result.Item2;
    }

    private async Task<Tuple<AsyncGetResult, CacheGetResponse?>> IssueGetRequest(ICacheClient client, String cacheKey)
    {
        var getResponse = await client.GetAsync(CACHE_NAME, cacheKey);
        if (getResponse is CacheGetResponse.Hit hit)
        {
            return Tuple.Create<AsyncGetResult, CacheGetResponse?>(AsyncGetResult.SUCCESS, hit);
        }
        else if (getResponse is CacheGetResponse.Miss miss)
        {
            return Tuple.Create<AsyncGetResult, CacheGetResponse?>(AsyncGetResult.SUCCESS, miss);
        }
        else if (getResponse is CacheGetResponse.Error error)
        {
            return Tuple.Create<AsyncGetResult, CacheGetResponse?>(ConvertErrorToAsyncSetGetResult(error.ErrorCode, error.InnerException), null);
        }
        else
        {
            throw new ApplicationException($"Unsupported get response: {getResponse}");
        }
    }

    private static void UpdateContextCountsForRequest(
        CsharpLoadGeneratorContext context,
        AsyncGetResult result
    )
    {
        Interlocked.Increment(ref context.GlobalRequestCount);
        var updated = result switch
        {
            AsyncGetResult.SUCCESS => Interlocked.Increment(ref context.GlobalSuccessCount),
            AsyncGetResult.UNAVAILABLE => Interlocked.Increment(ref context.GlobalUnavailableCount),
            AsyncGetResult.TIMEOUT => Interlocked.Increment(ref context.GlobalTimeoutExceededCount),
            AsyncGetResult.LIMIT_EXCEEDED => Interlocked.Increment(ref context.GlobalLimitExceededCount),
            AsyncGetResult.RST_STREAM => Interlocked.Increment(ref context.GlobalRstStreamCount),
            _ => throw new Exception($"Unrecognized result: {result}"),
        };
        return;
    }

    private AsyncGetResult ConvertErrorToAsyncSetGetResult(MomentoErrorCode errorCode, SdkException ex)
    {
        if (errorCode == MomentoErrorCode.SERVER_UNAVAILABLE)
        {
            _logger.LogError("SERVER UNAVAILABLE: {}", ex);
            return AsyncGetResult.UNAVAILABLE;
        }
        else if (errorCode == MomentoErrorCode.INTERNAL_SERVER_ERROR)
        {
            _logger.LogError("INTERNAL SERVER ERROR: {}", ex);
            return AsyncGetResult.UNAVAILABLE;
        }
        else if (errorCode == MomentoErrorCode.TIMEOUT_ERROR)
        {
            _logger.LogError("TIMEOUT ERROR: {}", ex);
            return AsyncGetResult.TIMEOUT;
        }
        else if (errorCode == MomentoErrorCode.LIMIT_EXCEEDED_ERROR)
        {
            return AsyncGetResult.LIMIT_EXCEEDED;
        }
        _logger.LogError("UNCAUGHT EXCEPTION: {}", ex);
        throw new ApplicationException($"Unsupported error code: {errorCode}");

    }

    static void Main(string[] args)
    {
        new Function().FunctionHandler();
    }
}
