using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Momento.Protos.ControlClient;
using Momento.Sdk.Internal;
using static System.Reflection.Assembly;

namespace Momento.Sdk.Internal;

internal sealed class ControlGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;
    public ScsControl.ScsControlClient Client { get; }
    private readonly string version = "dotnet:" + GetAssembly(typeof(Momento.Sdk.Responses.CacheGetResponse)).GetName().Version.ToString();
    // Some System.Environment.Version remarks to be aware of
    // https://learn.microsoft.com/en-us/dotnet/api/system.environment.version?view=netstandard-2.0#remarks
    private readonly string runtimeVersion = "dotnet:" + System.Environment.Version;
    private readonly ILogger _logger;

    public ControlGrpcManager(string authToken, string endpoint, ILoggerFactory loggerFactory)
    {
        var uri = $"https://{endpoint}";
        this.channel = GrpcChannel.ForAddress(uri, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
        List<Header> headers = new List<Header> { new Header(name: Header.AuthorizationKey, value: authToken), new Header(name: Header.AgentKey, value: version), new Header(name: Header.RuntimeVersionKey, value: runtimeVersion) };
        CallInvoker invoker = channel.Intercept(new HeaderInterceptor(headers));
        Client = new ScsControl.ScsControlClient(invoker);
        this._logger = loggerFactory.CreateLogger<ControlGrpcManager>();
    }

    public void Dispose()
    {
        this.channel.Dispose();
        GC.SuppressFinalize(this);
    }
}
