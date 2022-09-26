using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Momento.Protos.CacheClient;
using Momento.Sdk.Internal;
using static System.Reflection.Assembly;

namespace Momento.Sdk.Internal;

public class DataGrpcManager : IDisposable
{
    private readonly GrpcChannel channel;
    public Scs.ScsClient Client { get; }

    private readonly string version = "dotnet:" + GetAssembly(typeof(Momento.Sdk.Responses.CacheGetResponse)).GetName().Version.ToString();
    // Some System.Environment.Version remarks to be aware of
    // https://learn.microsoft.com/en-us/dotnet/api/system.environment.version?view=netstandard-2.0#remarks
    private readonly string runtimeVersion = "dotnet:" + System.Environment.Version;
    private readonly ILogger _logger;

    internal DataGrpcManager(string authToken, string host, ILoggerFactory? loggerFactory = null)
    {
        var url = $"https://{host}";
        this.channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions() { Credentials = ChannelCredentials.SecureSsl });
        List<Header> headers = new List<Header> { new Header(name: Header.AuthorizationKey, value: authToken), new Header(name: Header.AgentKey, value: version), new Header(name: Header.RuntimeVersionKey, value: runtimeVersion) };
        CallInvoker invoker = this.channel.Intercept(new HeaderInterceptor(headers));
        Client = new Scs.ScsClient(invoker);
        this._logger = Utils.CreateOrNullLogger<DataGrpcManager>(loggerFactory);
    }

    public void Dispose()
    {
        this.channel.Dispose();
        GC.SuppressFinalize(this);
    }
}
