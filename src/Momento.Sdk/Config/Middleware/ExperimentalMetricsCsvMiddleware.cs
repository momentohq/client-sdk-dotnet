using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;

namespace Momento.Sdk.Config.Middleware
{
    /// <summary>
    /// This middleware enables per-request client-side metrics. Metrics for each
    /// request will be written to a CSV file. This file can be analyzed or shared
    /// with Momento to diagnose performance issues.
    ///
    /// The metrics format is currently considered experimental. In a future release,
    /// once the format is considered stable, this class will be renamed to remove
    /// the Experimental prefix.
    ///
    /// WARNING: enabling this middleware may have minor performance implications,
    /// so enable with caution.
    ///
    /// WARNING: depending on your request volume, the CSV file size may grow quickly.
    /// Neither sampling nor file compression / rotation are included at this time
    /// (though they may be added in the future).
    /// </summary>
    public class ExperimentalMetricsCsvMiddleware : ExperimentalMetricsMiddleware, IDisposable
    {
        private readonly StreamWriter _writer;
        private readonly object _lock = new object();

        /// <summary>
        /// Constructor for the ExperimentalMetricsCsvMiddleware class.
        /// If the file at the specified path exists, the StreamWriter will append to it.
        /// If the file does not exist, the StreamWriter will create it.
        /// </summary>
        /// <param name="filePath">The path to the file where the middleware will write metrics data.</param>
        /// <param name="loggerFactory">Used for logging in case of errors.</param>
        public ExperimentalMetricsCsvMiddleware(string filePath, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            _writer = new StreamWriter(filePath, true, Encoding.UTF8);
        }

        /// <summary>
        /// Writes metrics for a request out to a CSV.
        /// </summary>
        public override Task EmitMetrics(ExperimentalRequestMetrics metrics)
        {
            var csvLine = $"{metrics.NumActiveRequestsAtStart}, {metrics.NumActiveRequestsAtFinish}, " +
                                     $"{metrics.RequestType}, {metrics.Status}, {metrics.StartTime}, {metrics.EndTime}," +
                                     $"{metrics.Duration}, {metrics.RequestSize}, {metrics.ResponseSize}";

            lock (_lock)
            {
                _writer.WriteLine(csvLine);
                _writer.Flush();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Closes the CSV file writer.
        /// </summary>
        public void Dispose()
        {
            _writer.Close();
        }
    }

}
