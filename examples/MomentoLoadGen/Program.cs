using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using System.Text;
using Grpc.Core;
using HdrHistogram;
using Microsoft.Extensions.Logging;
using Momento.Sdk;
using Momento.Sdk.Exceptions;

namespace MomentoLoadGen
{
    public record CsharpLoadGeneratorOptions
    (
        uint requestTimeoutMs,
        int cacheItemPayloadBytes,
        int numberOfConcurrentRequests,
        int totalNumberOfOperationsToExecute
    );

    enum AsyncSetGetResult
    {
        SUCCESS,
        UNAVAILABLE,
        DEADLINE_EXCEEDED,
        RESOURCE_EXHAUSTED,
        RST_STREAM
    };

    internal class CsharpLoadGeneratorContext
    {
        public Stopwatch StartTime;
        public Recorder GetLatencies;
        public Recorder SetLatencies;

        public int GlobalRequestCount;
        public int GlobalSuccessCount;
        public int GlobalUnavailableCount;
        public int GlobalDeadlineExceededCount;
        public int GlobalResourceExhaustedCount;
        public int GlobalRstStreamCount;

        public CsharpLoadGeneratorContext()
        {
            StartTime = System.Diagnostics.Stopwatch.StartNew();
            GetLatencies = HistogramFactory.With64BitBucketSize().WithValuesFrom(1).WithValuesUpTo(TimeStamp.Minutes(1)).WithPrecisionOf(1).WithThreadSafeWrites().WithThreadSafeReads().Create();
            SetLatencies = HistogramFactory.With64BitBucketSize().WithValuesFrom(1).WithValuesUpTo(TimeStamp.Minutes(1)).WithPrecisionOf(1).WithThreadSafeWrites().WithThreadSafeReads().Create();

            GlobalRequestCount = 0;
            GlobalSuccessCount = 0;
            GlobalDeadlineExceededCount = 0;
            GlobalResourceExhaustedCount = 0;
            GlobalRstStreamCount = 0;
            GlobalUnavailableCount = 0;
        }
    };


    public class CsharpLoadGenerator
    {
        const int CACHE_ITEM_TTL_SECONDS = 60;
        const string CACHE_NAME = "momento-loadgen";
        const int NUM_REQUESTS_PER_OPERATION = 2;
        const int PRINT_STATS_EVERY_N_REQUESTS = 5000;

        private readonly ILogger<CsharpLoadGenerator> _logger;
        private readonly CsharpLoadGeneratorOptions _options;
        private readonly string _cacheValue;

        public CsharpLoadGenerator(ILoggerFactory loggerFactory, CsharpLoadGeneratorOptions options)
        {
            _logger = loggerFactory.CreateLogger<CsharpLoadGenerator>();

            _options = options;

            _cacheValue = new String('x', _options.cacheItemPayloadBytes);
        }


        public async Task Run()
        {
            string? authToken = System.Environment.GetEnvironmentVariable("MOMENTO_AUTH_TOKEN");
            if (authToken == null)
            {
                throw new Exception("Missing required environment variable MOMENTO_AUTH_TOKEN");
            }

            var momento = new SimpleCacheClient(
                authToken,
                CACHE_ITEM_TTL_SECONDS,
                _options.requestTimeoutMs
            );

            try
            {
                momento.CreateCache(CACHE_NAME);
            }
            catch (AlreadyExistsException)
            {
                _logger.LogInformation("cache '{0}' already exists", CACHE_NAME);
            }


            var numOperationsPerWorker = _options.totalNumberOfOperationsToExecute / _options.numberOfConcurrentRequests;
            var totalNumRequestsExpected = _options.totalNumberOfOperationsToExecute * NUM_REQUESTS_PER_OPERATION;

            var context = new CsharpLoadGeneratorContext();


            var asyncResults = Enumerable.Range(0, _options.numberOfConcurrentRequests).Select<int, Task>(workerId =>
                LaunchAndRunWorkers(
                    momento,
                    context,
                    workerId + 1,
                    numOperationsPerWorker
                    )
            );

            var statsPrinterTask = LaunchStatsPrinterTask(context, totalNumRequestsExpected);

            await Task.WhenAll(asyncResults);

            await statsPrinterTask;
            _logger.LogInformation("Done");
        }


        private async Task LaunchAndRunWorkers(
            SimpleCacheClient client,
            CsharpLoadGeneratorContext context,
            int workerId,
            int numOperations)
        {
            for (var i = 1; i <= numOperations; i++)
            {
                await IssueAsyncSetGet(client, context, workerId, i);

            }
        }

        private async Task LaunchStatsPrinterTask(CsharpLoadGeneratorContext context, int totalNumRequests)
        {
            var setsAccumulatingHistogram = new LongHistogram(TimeStamp.Minutes(1), 1);
            var getsAccumulatingHistogram = new LongHistogram(TimeStamp.Minutes(1), 1);

            var nextStatsUpdateRequestCount = PRINT_STATS_EVERY_N_REQUESTS;
            while (context.GlobalRequestCount < totalNumRequests)
            {
                if (context.GlobalRequestCount >= nextStatsUpdateRequestCount)
                {
                    nextStatsUpdateRequestCount += PRINT_STATS_EVERY_N_REQUESTS;
                    PrintStats(setsAccumulatingHistogram, getsAccumulatingHistogram, context);
                }

                await Task.Delay(500);
            }
            PrintStats(setsAccumulatingHistogram, getsAccumulatingHistogram, context);
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
        deadline exceeded: {context.GlobalDeadlineExceededCount} ({PercentRequests(context, context.GlobalDeadlineExceededCount)}%)
    resource exhausted: {context.GlobalResourceExhaustedCount} ({PercentRequests(context, context.GlobalResourceExhaustedCount)}%)
            rst stream: {context.GlobalRstStreamCount} ({PercentRequests(context, context.GlobalRstStreamCount)}%)

cumulative set latencies:
{OutputHistogramSummary(setsAccumulatingHistogram)}

cumulative get latencies:
{OutputHistogramSummary(getsAccumulatingHistogram)}

");
        }

        private async Task IssueAsyncSetGet(SimpleCacheClient client, CsharpLoadGeneratorContext context, int workerId, int operationId)
        {
            var cacheKey = $"worker{workerId}operation{operationId}";
            var setStartTime = System.Diagnostics.Stopwatch.StartNew();
            var result = await ExecuteRequestAndUpdateContextCounts(
                context,
                () => client.SetAsync(CACHE_NAME, cacheKey, _cacheValue)
                );
            if (result != null)
            {
                var setDuration = setStartTime.ElapsedMilliseconds;
                context.SetLatencies.RecordValue(setDuration);
            }

            var getStartTime = System.Diagnostics.Stopwatch.StartNew();
            var getResult = await ExecuteRequestAndUpdateContextCounts(
                context,
                () => client.GetAsync(CACHE_NAME, cacheKey)
                );

            if (getResult != null)
            {
                var getDuration = getStartTime.ElapsedMilliseconds;
                context.GetLatencies.RecordValue(getDuration);

                string valueString;

                if (getResult.Status == Momento.Sdk.Responses.CacheGetStatus.HIT)
                {
                    string value = getResult.String()!;
                    valueString = $"{value.Substring(0, 10)}... (len: {value.Length})";
                }
                else
                {
                    valueString = "n/a";
                }

                if (context.GlobalRequestCount % PRINT_STATS_EVERY_N_REQUESTS == 0)
                {
                    _logger.LogInformation($"worker: {workerId}, worker request: {operationId}, global request: {context.GlobalRequestCount}, status: ${getResult.Status}, val: ${valueString}");
                }
            }
        }

        private async Task<TResult?> ExecuteRequestAndUpdateContextCounts<TResult>(
            CsharpLoadGeneratorContext context,
            Func<Task<TResult>> block
            )
        {
            var result = await ExecuteRequest(block);
            UpdateContextCountsForRequest(context, result.Item1);
            return result.Item2;
        }


        private async Task<Tuple<AsyncSetGetResult, T?>> ExecuteRequest<T>(
            Func<Task<T>> block
            )
        {
            try
            {
                T result = await block();
                return Tuple.Create(AsyncSetGetResult.SUCCESS, (T?)result);
            }
            catch (InternalServerException e)
            {
                var innerException = e.InnerException as RpcException;
                _logger.LogWarning("CAUGHT AN EXCEPTION WHILE EXECUTING REQUEST: {0}", innerException);
                switch (innerException!.StatusCode)
                {

                    case StatusCode.Unavailable:
                        return Tuple.Create(AsyncSetGetResult.UNAVAILABLE, default(T));
                    default:
                        throw e;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("CAUGHT AN EXCEPTION WHILE EXECUTING REQUEST: {0}", e);
                throw;
            }
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
                AsyncSetGetResult.DEADLINE_EXCEEDED => Interlocked.Increment(ref context.GlobalDeadlineExceededCount),
                AsyncSetGetResult.RESOURCE_EXHAUSTED => Interlocked.Increment(ref context.GlobalResourceExhaustedCount),
                AsyncSetGetResult.RST_STREAM => Interlocked.Increment(ref context.GlobalRstStreamCount),
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
        static ILoggerFactory InitializeLogging()
        {
            return LoggerFactory.Create(builder =>
                   builder.AddSimpleConsole(options =>
                   {
                       options.IncludeScopes = true;
                       options.SingleLine = true;
                       options.TimestampFormat = "hh:mm:ss ";
                   }));
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
            using ILoggerFactory loggerFactory = InitializeLogging();

            CsharpLoadGeneratorOptions loadGeneratorOptions = new CsharpLoadGeneratorOptions(
              /**
               * Configures the Momento client to timeout if a request exceeds this limit.
               * Momento client default is 5 seconds.
               */
              requestTimeoutMs: 5 * 1000,
              /**
               * Controls the size of the payload that will be used for the cache items in
               * the load test.  Smaller payloads will generally provide lower latencies than
               * larger payloads.
               */
              cacheItemPayloadBytes: 100,
              /**
               * Controls the number of concurrent requests that will be made (via asynchronous
               * function calls) by the load test.  Increasing this number may improve throughput,
               * but it will also increase CPU consumption.  As CPU usage increases and there
               * is more contention between the concurrent function calls, client-side latencies
               * may increase.
               */
              numberOfConcurrentRequests: 50,
              /**
               * Controls how long the load test will run.  We will execute this many operations
               * (1 cache 'set' followed immediately by 1 'get') across all of our concurrent
               * workers before exiting.  Statistics will be logged every 1000 operations.
               */
              totalNumberOfOperationsToExecute: 50_000
            );

            CsharpLoadGenerator loadGenerator = new CsharpLoadGenerator(
                loggerFactory,
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