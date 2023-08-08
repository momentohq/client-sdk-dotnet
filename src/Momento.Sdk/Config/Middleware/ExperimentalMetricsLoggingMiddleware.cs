using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Momento.Sdk.Config.Middleware
{
    /// <summary>
    /// This middleware enables per-request client-side metrics. Metrics for each
    /// request will be written to logs. The log data can be analyzed or shared
    /// with Momento to diagnose performance issues.
    ///
    /// The metrics format is currently considered experimental. In a future release,
    /// once the format is considered stable, this class will be renamed to remove
    /// the Experimental prefix.
    ///
    /// WARNING: enabling this middleware may have minor performance implications,
    /// so enable with caution.
    ///
    /// WARNING: depending on your request volume, this middleware will produce a high
    /// volume of log output. If you are writing logs directly to local disk, be aware
    /// of disk usage and make sure you have log rotation / compression enabled via a
    /// tool such as `logrotate`.
    /// </summary>
    public class ExperimentalMetricsLoggingMiddleware : ExperimentalMetricsMiddleware
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor for the ExperimentalMetricsLoggingMiddleware class.
        /// </summary>
        /// <param name="loggerFactory">Used for logging the metrics and any errors that occur.</param>
        public ExperimentalMetricsLoggingMiddleware(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExperimentalMetricsLoggingMiddleware>();
        }

        /// <summary>
        /// Logs metrics for a Momento request.
        /// </summary>
        public override Task EmitMetrics(ExperimentalRequestMetrics metrics)
        {
            var json = "{" +
                       $"\"numActiveRequestsAtStart\": {metrics.NumActiveRequestsAtStart}, " +
                       $"\"numActiveRequestsAtFinish\": {metrics.NumActiveRequestsAtFinish}, " +
                       $"\"requestType\": \"{metrics.RequestType}\", " +
                       $"\"status\": \"{metrics.Status}\", " +
                       $"\"startTime\": \"{metrics.StartTime}\", " +
                       $"\"endTime\": \"{metrics.EndTime}\", " +
                       $"\"duration\": \"{metrics.Duration}\", " +
                       $"\"requestSize\": {metrics.RequestSize}, " +
                       $"\"responseSize\": {metrics.ResponseSize}" +
                       "}";

            _logger.LogInformation(json);

            return Task.CompletedTask;
        }
    }
}
