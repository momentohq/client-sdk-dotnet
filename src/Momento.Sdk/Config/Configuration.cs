using Microsoft.Extensions.Logging;
using Momento.Sdk.Config.Middleware;
using Momento.Sdk.Config.Retry;
using Momento.Sdk.Config.Transport;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Momento.Sdk.Config;

/// <inheritdoc cref="Momento.Sdk.Config.IConfiguration" />
public class Configuration : IConfiguration
{
    /// <inheritdoc />
    public ILoggerFactory LoggerFactory { get; }

    /// <inheritdoc />
    public IRetryStrategy RetryStrategy { get; }

    /// <inheritdoc />
    public IList<IMiddleware> Middlewares { get; }

    /// <inheritdoc />
    public ITransportStrategy TransportStrategy { get; }

    /// <inheritdoc />
    public ReadConcern ReadConcern { get; }

    /// <inheritdoc cref="Momento.Sdk.Config.IConfiguration" />
    public Configuration(ILoggerFactory loggerFactory, IRetryStrategy retryStrategy,
        ITransportStrategy transportStrategy)
        : this(loggerFactory, retryStrategy, new List<IMiddleware>(), transportStrategy)
    {
    }

    /// <summary>
    /// Create a new instance of Configuration object with provided arguments: <see cref="Momento.Sdk.Config.IConfiguration.RetryStrategy" />, <see cref="Momento.Sdk.Config.IConfiguration.Middlewares" />, <see cref="Momento.Sdk.Config.IConfiguration.TransportStrategy"/>, and <see cref="Momento.Sdk.Config.IConfiguration.LoggerFactory"/>
    /// </summary>
    /// <param name="retryStrategy">Defines a contract for how and when to retry a request</param>
    /// <param name="middlewares">The Middleware interface allows the Configuration to provide a higher-order function that wraps all requests.</param>
    /// <param name="transportStrategy">This is responsible for configuring network tunables.</param>
    /// <param name="loggerFactory">This is responsible for configuring logging.</param>
    /// <param name="readConcern">The client-wide setting for read-after-write consistency. Defaults to Balanced.</param>
    public Configuration(ILoggerFactory loggerFactory, IRetryStrategy retryStrategy, IList<IMiddleware> middlewares,
        ITransportStrategy transportStrategy, ReadConcern readConcern = ReadConcern.Balanced)
    {
        LoggerFactory = loggerFactory;
        RetryStrategy = retryStrategy;
        Middlewares = middlewares;
        TransportStrategy = transportStrategy;
        ReadConcern = readConcern;
    }

    /// <inheritdoc />
    public IConfiguration WithRetryStrategy(IRetryStrategy retryStrategy)
    {
        return new Configuration(LoggerFactory, retryStrategy, Middlewares, TransportStrategy, ReadConcern);
    }

    /// <inheritdoc />
    public IConfiguration WithMiddlewares(IList<IMiddleware> middlewares)
    {
        return new Configuration(LoggerFactory, RetryStrategy, middlewares, TransportStrategy, ReadConcern);
    }

    /// <inheritdoc />
    public IConfiguration WithTransportStrategy(ITransportStrategy transportStrategy)
    {
        return new Configuration(LoggerFactory, RetryStrategy, Middlewares, transportStrategy, ReadConcern);
    }

    /// <summary>
    /// Add the specified middlewares to an existing instance of Configuration object in addition to already specified middlewares.
    /// </summary>
    /// <param name="additionalMiddlewares">The Middleware interface allows the Configuration to provide a higher-order function that wraps all requests.</param>
    /// <returns>Configuration object with custom middlewares provided</returns>
    public Configuration WithAdditionalMiddlewares(IList<IMiddleware> additionalMiddlewares)
    {
        return new(
            retryStrategy: RetryStrategy,
            middlewares: Middlewares.Concat(additionalMiddlewares).ToList(),
            transportStrategy: TransportStrategy,
            loggerFactory: LoggerFactory,
            readConcern: ReadConcern
        );
    }

    /// <summary>
    /// Add the specified client timeout to an existing instance of Configuration object as an addition to the existing transport strategy.
    /// </summary>
    /// <param name="clientTimeout">The amount of time to wait before cancelling the request.</param>
    /// <returns>Configuration object with client timeout provided</returns>
    public Configuration WithClientTimeout(TimeSpan clientTimeout)
    {
        return new(
            retryStrategy: RetryStrategy,
            middlewares: Middlewares,
            transportStrategy: TransportStrategy.WithClientTimeout(clientTimeout),
            loggerFactory: LoggerFactory,
            readConcern: ReadConcern
        );
    }

    /// <inheritdoc />
    public IConfiguration WithReadConcern(ReadConcern readConcern)
    {
        return new Configuration(
            retryStrategy: RetryStrategy,
            middlewares: Middlewares,
            transportStrategy: TransportStrategy,
            loggerFactory: LoggerFactory,
            readConcern: readConcern
        );
    }

    IConfiguration IConfiguration.WithClientTimeout(TimeSpan clientTimeout)
    {
        return WithClientTimeout(clientTimeout);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }

        var other = (Configuration)obj;
        return RetryStrategy.Equals(other.RetryStrategy) &&
               Middlewares.SequenceEqual(other.Middlewares) &&
               TransportStrategy.Equals(other.TransportStrategy) &&
               LoggerFactory.Equals(other.LoggerFactory) &&
               ReadConcern.Equals(other.ReadConcern);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
