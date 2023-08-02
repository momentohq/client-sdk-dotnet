using System.Diagnostics;
using HdrHistogram;
using Microsoft.Extensions.Logging;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Config.Transport;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;

namespace MomentoLoadGen
{
    public record CsharpLoadGeneratorOptions
    (
        LogLevel logLevel,
        TimeSpan showStatsInterval,
        int cacheItemPayloadBytes,
        int numberOfConcurrentRequests,
        int maxRequestsPerSecond,
        TimeSpan howLongToRun
    );

    enum AsyncSetGetResult
    {
        SUCCESS,
        UNAVAILABLE,
        TIMEOUT,
        LIMIT_EXCEEDED,
        RST_STREAM,
        UNKNOWN,
        CANCELLED
    };

    internal class CsharpLoadGeneratorContext
    {
        public Stopwatch StartTime;
        public Recorder GetLatencies;
        public Recorder SetLatencies;

        public int GlobalRequestCount;
        public int LastWorkerStatsPrintRequestCount;
        public int GlobalSuccessCount;
        public int GlobalUnavailableCount;
        public int GlobalTimeoutExceededCount;
        public int GlobalLimitExceededCount;
        public int GlobalUnknownCount;
        public int GlobalCancelledCount;
        public int GlobalRstStreamCount;

        public CsharpLoadGeneratorContext()
        {
            StartTime = System.Diagnostics.Stopwatch.StartNew();
            GetLatencies = HistogramFactory.With64BitBucketSize().WithValuesFrom(1).WithValuesUpTo(TimeStamp.Minutes(1)).WithPrecisionOf(1).WithThreadSafeWrites().WithThreadSafeReads().Create();
            SetLatencies = HistogramFactory.With64BitBucketSize().WithValuesFrom(1).WithValuesUpTo(TimeStamp.Minutes(1)).WithPrecisionOf(1).WithThreadSafeWrites().WithThreadSafeReads().Create();

            GlobalRequestCount = 0;
            LastWorkerStatsPrintRequestCount = 0;
            GlobalSuccessCount = 0;
            GlobalTimeoutExceededCount = 0;
            GlobalLimitExceededCount = 0;
            GlobalUnknownCount = 0;
            GlobalCancelledCount = 0;
            GlobalRstStreamCount = 0;
            GlobalUnavailableCount = 0;
        }
    };


    public class CsharpLoadGenerator
    {
        const int CACHE_ITEM_TTL_SECONDS = 60;
        const string CACHE_NAME = "dotnet-momento-loadgen";
        const int NUM_REQUESTS_PER_OPERATION = 2;

        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<CsharpLoadGenerator> _logger;
        private readonly IConfiguration _momentoClientConfig;
        private readonly CsharpLoadGeneratorOptions _options;
        private readonly string _cacheValue;

        public CsharpLoadGenerator(IConfiguration momentoClientConfig, CsharpLoadGeneratorOptions options)
        {
            _loggerFactory = momentoClientConfig.LoggerFactory;
            _logger = _loggerFactory.CreateLogger<CsharpLoadGenerator>();
            _momentoClientConfig = momentoClientConfig;

            _options = options;

            _cacheValue = new String('x', _options.cacheItemPayloadBytes);
        }


        public async Task Run()
        {
            var authProvider = new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN");

            using (ICacheClient momento = new CacheClient(
                _momentoClientConfig,
                authProvider,
                TimeSpan.FromSeconds(CACHE_ITEM_TTL_SECONDS)
            ))
            {
                try
                {
                    await momento.CreateCacheAsync(CACHE_NAME);
                }
                catch (AlreadyExistsException)
                {
                    _logger.LogInformation("cache '{0}' already exists", CACHE_NAME);
                }


                var workerDelayBetweenRequests = Convert.ToInt32(Math.Floor((1000.0 * _options.numberOfConcurrentRequests) / (_options.maxRequestsPerSecond * 1)));
                Console.WriteLine($"Targeting a max of {_options.maxRequestsPerSecond} requests per second (delay between requests: {workerDelayBetweenRequests})");
                Console.WriteLine($"Running {_options.numberOfConcurrentRequests} concurrent requests for {_options.howLongToRun}");

                var context = new CsharpLoadGeneratorContext();

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                var asyncTasks = Enumerable.Range(0, _options.numberOfConcurrentRequests).Select<int, Task>(workerId =>
                    LaunchAndRunWorkers(
                        momento,
                        context,
                        workerId + 1,
                        workerDelayBetweenRequests,
                        cancellationTokenSource.Token
                    )
                );
                var statsPrinterTask = LaunchStatsPrinterTask(
                    context, 
                    _options.showStatsInterval, 
                    cancellationTokenSource.Token
                );
                asyncTasks.Append(statsPrinterTask);

                cancellationTokenSource.CancelAfter(_options.howLongToRun);

                // this will ensure that the program exits promptly if one of the async
                // tasks throws an uncaught exception.
                var firstResult = await Task.WhenAny(asyncTasks);
                await firstResult;
                
                await Task.WhenAll(asyncTasks);
            }
            _logger.LogInformation("Done");
        }


        private async Task LaunchAndRunWorkers(
            ICacheClient client,
            CsharpLoadGeneratorContext context,
            int workerId,
            int delayMillisBetweenRequests,
            CancellationToken token
        )
        {
            for (var i = 1; !token.IsCancellationRequested; i++)
            {
                await IssueAsyncSetGet(client, context, workerId, i, delayMillisBetweenRequests);
            }
        }

        private async Task LaunchStatsPrinterTask(CsharpLoadGeneratorContext context, TimeSpan showStatsInterval, CancellationToken token)
        {
            var setsAccumulatingHistogram = new LongHistogram(TimeStamp.Minutes(1), 1);
            var getsAccumulatingHistogram = new LongHistogram(TimeStamp.Minutes(1), 1);

            while (!token.IsCancellationRequested)
            {
                try {
                    await Task.Delay(showStatsInterval, token);
                }
                finally {
                    PrintStats(setsAccumulatingHistogram, getsAccumulatingHistogram, context);
                }
            }
        }

        private void PrintStats(LongHistogram setsAccumulatingHistogram, LongHistogram getsAccumulatingHistogram, CsharpLoadGeneratorContext context)
        {
            var setsIntervalHistogram = context.SetLatencies.GetIntervalHistogram();
            setsAccumulatingHistogram.Add(setsIntervalHistogram);
            var getsIntervalHistogram = context.GetLatencies.GetIntervalHistogram();
            getsAccumulatingHistogram.Add(getsIntervalHistogram);

            Console.WriteLine($@"
cumulative stats:
        total requests: {context.GlobalRequestCount} ({Tps(context, context.GlobalRequestCount)} tps)
               success: {context.GlobalSuccessCount} ({PercentRequests(context, context.GlobalSuccessCount)}%) ({Tps(context, context.GlobalSuccessCount)} tps)
           unavailable: {context.GlobalUnavailableCount} ({PercentRequests(context, context.GlobalUnavailableCount)}%)
      timeout exceeded: {context.GlobalTimeoutExceededCount} ({PercentRequests(context, context.GlobalTimeoutExceededCount)}%)
        limit exceeded: {context.GlobalLimitExceededCount} ({PercentRequests(context, context.GlobalLimitExceededCount)}%)
        cancelled: {context.GlobalCancelledCount} ({PercentRequests(context, context.GlobalCancelledCount)}%)
        unknown: {context.GlobalUnknownCount} ({PercentRequests(context, context.GlobalUnknownCount)}%)
            rst stream: {context.GlobalRstStreamCount} ({PercentRequests(context, context.GlobalRstStreamCount)}%)

cumulative set latencies:
{OutputHistogramSummary(setsAccumulatingHistogram)}

cumulative get latencies:
{OutputHistogramSummary(getsAccumulatingHistogram)}

");
            _logger.LogInformation($"Load gen data point:\t{_options.numberOfConcurrentRequests}\t{Tps(context, context.GlobalRequestCount)}\t{getsAccumulatingHistogram.GetValueAtPercentile(50)}\t{getsAccumulatingHistogram.GetValueAtPercentile(99.9)}");
        }

        private async Task IssueAsyncSetGet(ICacheClient client, CsharpLoadGeneratorContext context, int workerId, int operationId, int delayMillisBetweenRequests)
        {
            var cacheKey = $"worker{workerId}operation{operationId}";

            var setStartTime = System.Diagnostics.Stopwatch.StartNew();
            var result = await ExecuteRequestAndUpdateContextCounts(
                context,
                () => IssueSetRequest(client, cacheKey, _cacheValue)
                );
            if (result is CacheSetResponse.Success setSuccess)
            {
                var setDuration = setStartTime.ElapsedMilliseconds;
                context.SetLatencies.RecordValue(setDuration);
                if (setDuration < delayMillisBetweenRequests)
                {
                    await Task.Delay((int)(delayMillisBetweenRequests - setDuration));
                }
            }

            var getStartTime = System.Diagnostics.Stopwatch.StartNew();
            var getResponse = await ExecuteRequestAndUpdateContextCounts(
                context,
                () => IssueGetRequest(client, cacheKey)
                );

            if (getResponse is CacheGetResponse.Hit hitResponse)
            {
                var getDuration = getStartTime.ElapsedMilliseconds;
                context.GetLatencies.RecordValue(getDuration);
                if (getDuration < delayMillisBetweenRequests)
                {
                    await Task.Delay((int)(delayMillisBetweenRequests - getDuration));
                }
            }
        }

        private async Task<TResult> ExecuteRequestAndUpdateContextCounts<TResult>(
            CsharpLoadGeneratorContext context,
            Func<Task<Tuple<AsyncSetGetResult, TResult>>> block
            )
        {
            var result = await block();
            UpdateContextCountsForRequest(context, result.Item1);
            return result.Item2;
        }

        private async Task<Tuple<AsyncSetGetResult, CacheGetResponse?>> IssueGetRequest(ICacheClient client, String cacheKey)
        {
            var getResponse = await client.GetAsync(CACHE_NAME, cacheKey);
            if (getResponse is CacheGetResponse.Hit hit)
            {
                return Tuple.Create<AsyncSetGetResult, CacheGetResponse?>(AsyncSetGetResult.SUCCESS, hit);
            }
            else if (getResponse is CacheGetResponse.Miss miss)
            {
                return Tuple.Create<AsyncSetGetResult, CacheGetResponse?>(AsyncSetGetResult.SUCCESS, miss);
            }
            else if (getResponse is CacheGetResponse.Error error)
            {
                return Tuple.Create<AsyncSetGetResult, CacheGetResponse?>(ConvertErrorToAsyncSetGetResult(error.ErrorCode, error.InnerException), null);
            }
            else
            {
                throw new ApplicationException($"Unsupported get response: {getResponse}");
            }
        }

        private async Task<Tuple<AsyncSetGetResult, CacheSetResponse?>> IssueSetRequest(ICacheClient client, String cacheKey, String cacheValue)
        {
            var setResponse = await client.SetAsync(CACHE_NAME, cacheKey, cacheValue);
            if (setResponse is CacheSetResponse.Success success)
            {
                return Tuple.Create<AsyncSetGetResult, CacheSetResponse?>(AsyncSetGetResult.SUCCESS, success);
            }
            else if (setResponse is CacheSetResponse.Error error)
            {
                return Tuple.Create<AsyncSetGetResult, CacheSetResponse?>(ConvertErrorToAsyncSetGetResult(error.ErrorCode, error.InnerException), null);
            }
            else
            {
                throw new ApplicationException($"Unsupported set response: {setResponse}");
            }
        }

        private AsyncSetGetResult ConvertErrorToAsyncSetGetResult(MomentoErrorCode errorCode, SdkException ex)
        {
            if (errorCode == MomentoErrorCode.SERVER_UNAVAILABLE)
            {
                _logger.LogError("SERVER UNAVAILABLE: {}", ex);
                return AsyncSetGetResult.UNAVAILABLE;
            }
            else if (errorCode == MomentoErrorCode.INTERNAL_SERVER_ERROR)
            {
                _logger.LogError("INTERNAL SERVER ERROR: {}", ex);
                return AsyncSetGetResult.UNAVAILABLE;
            }
            else if (errorCode == MomentoErrorCode.TIMEOUT_ERROR)
            {
                _logger.LogError("TIMEOUT ERROR: {}", ex);
                return AsyncSetGetResult.TIMEOUT;
            }
            else if (errorCode == MomentoErrorCode.LIMIT_EXCEEDED_ERROR)
            {
                return AsyncSetGetResult.LIMIT_EXCEEDED;
            }
            else if (errorCode is MomentoErrorCode.UNKNOWN_ERROR or MomentoErrorCode.UNKNOWN_SERVICE_ERROR)
            {
                return AsyncSetGetResult.UNKNOWN;
            }
            else if (errorCode == MomentoErrorCode.CANCELLED_ERROR)
            {
                return AsyncSetGetResult.CANCELLED;
            }
            _logger.LogError("UNCAUGHT EXCEPTION: {}", ex);
            throw new ApplicationException($"Unsupported error code: {errorCode}");
        }

        private static void UpdateContextCountsForRequest(
            CsharpLoadGeneratorContext context,
            AsyncSetGetResult result
            )
        {
            Interlocked.Increment(ref context.GlobalRequestCount);
            var updated = result switch
            {
                AsyncSetGetResult.SUCCESS => Interlocked.Increment(ref context.GlobalSuccessCount),
                AsyncSetGetResult.UNAVAILABLE => Interlocked.Increment(ref context.GlobalUnavailableCount),
                AsyncSetGetResult.TIMEOUT => Interlocked.Increment(ref context.GlobalTimeoutExceededCount),
                AsyncSetGetResult.LIMIT_EXCEEDED => Interlocked.Increment(ref context.GlobalLimitExceededCount),
                AsyncSetGetResult.RST_STREAM => Interlocked.Increment(ref context.GlobalRstStreamCount),
                AsyncSetGetResult.UNKNOWN => Interlocked.Increment(ref context.GlobalUnknownCount),
                AsyncSetGetResult.CANCELLED => Interlocked.Increment(ref context.GlobalCancelledCount),
                _ => throw new Exception($"Unrecognized result: {result}"),
            };
            return;
        }

        private static double Tps(CsharpLoadGeneratorContext context, int requestCount)
        {
            return Math.Round((requestCount * 1000.0) / context.StartTime.ElapsedMilliseconds);
        }

        private static string PercentRequests(CsharpLoadGeneratorContext context, int count)
        {
            if (context.GlobalRequestCount == 0)
            {
                return "0";
            }
            return Math.Round((Convert.ToDouble(count) / context.GlobalRequestCount) * 100.0, 1).ToString();
        }

        private static string OutputHistogramSummary(HistogramBase histogram)
        {
            return $@"
count: {histogram.TotalCount}
        p50: {histogram.GetValueAtPercentile(50)}
        p90: {histogram.GetValueAtPercentile(90)}
        p99: {histogram.GetValueAtPercentile(99)}
      p99.9: {histogram.GetValueAtPercentile(99.9)}
        max: {histogram.GetMaxValue()}
";
        }
    }


    internal class Program
    {
        static ILoggerFactory InitializeLogging(LogLevel minLogLevel)
        {
            return LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "hh:mm:ss ";
                });
                builder.AddFilter("Grpc.Net.Client", LogLevel.Error);
                builder.SetMinimumLevel(minLogLevel);
            });
        }

        const string PERFORMANCE_INFORMATION_MESSAGE = @"
Thanks for trying out our basic c# load generator!  This tool is
included to allow you to experiment with performance in your environment
based on different configurations.  It's very simplistic, and only intended
to give you a quick way to explore the performance of the Momento client
running in a dotnet application.

Since performance will be impacted by network latency, you'll get the best
results if you run on a cloud VM in the same region as your Momento cache.

Check out the configuration settings at the bottom of the 'MomentoLoadGen/Program.cs'
file to see how different configurations impact performance.

If you have questions or need help experimenting further, please reach out to us!
        ";


        static async Task Main(string[] args)
        {
            CsharpLoadGeneratorOptions loadGeneratorOptions = new CsharpLoadGeneratorOptions(
              ///
              /// Controls the verbosity of the output during the run.
              ///
              logLevel: LogLevel.Debug,
              ///
              /// Each time this amount of time has passed, statistics about throughput and latency
              /// will be printed.
              /// 
              ///
              showStatsInterval: TimeSpan.FromSeconds(5),
              ///
              /// Controls the size of the payload that will be used for the cache items in
              /// the load test.  Smaller payloads will generally provide lower latencies than
              /// larger payloads.
              ///
              cacheItemPayloadBytes: 100,
              ///
              /// Controls the number of concurrent requests that will be made (via asynchronous
              /// function calls) by the load test.  Increasing this number may improve throughput,
              /// but it will also increase CPU consumption.  As CPU usage increases and there
              /// is more contention between the concurrent function calls, client-side latencies
              /// may increase.
              ///
              numberOfConcurrentRequests: 50,
              ///
              /// Sets an upper bound on how many requests per second will be sent to the server.
              /// Momento caches have a default throttling limit of 100 requests per second,
              /// so if you raise this, you may observe throttled requests.  Contact
              /// support@momentohq.com to inquire about raising your limits.
              ///
              maxRequestsPerSecond: 100,
              ///
              /// Controls how long the load test will run.
              ///
              howLongToRun: TimeSpan.FromMinutes(1)
            );
            
            using (ILoggerFactory loggerFactory = InitializeLogging(loadGeneratorOptions.logLevel))
            {
                /// 
                /// This is the configuration that will be used for the Momento client.  Choose from
                /// our pre-built configurations that are optimized for Laptop vs InRegion environments,
                /// or build your own.
                ///
                IConfiguration config = Configurations.Laptop.V1(loggerFactory);

                CsharpLoadGenerator loadGenerator = new CsharpLoadGenerator(
                    config,
                    loadGeneratorOptions
                    );
                try
                {
                    await loadGenerator.Run();
                    Console.WriteLine("success!");
                    Console.WriteLine(PERFORMANCE_INFORMATION_MESSAGE);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR!: {0}", e);
                }
            }
        }
    }
}
