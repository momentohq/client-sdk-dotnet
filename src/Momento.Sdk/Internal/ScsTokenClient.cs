using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Momento.Sdk.Auth;
using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Responses;
using Momento.Protos.TokenClient;

namespace Momento.Sdk.Internal;

internal sealed class ScsTokenClient : IDisposable
{
    private readonly AuthGrpcManager grpcManager;
    private readonly string authToken;
    private readonly ILogger _logger;
    private readonly CacheExceptionMapper _exceptionMapper;
    public ScsTokenClient(IAuthConfiguration config, ICredentialProvider authProvider)
    {
        // TODO: make sure this is the right endpoint.
        this.grpcManager = new AuthGrpcManager(config, authProvider.AuthToken, authProvider.ControlEndpoint);
        this.authToken = authProvider.AuthToken;
        this._logger = config.LoggerFactory.CreateLogger<ScsTokenClient>();
        this._exceptionMapper = new CacheExceptionMapper(config.LoggerFactory);
    }

    private const string RequestTypeAuthGenerateDisposableToken = "GENERATE_DISPOSABLE_TOKEN";
    public async Task<GenerateDisposableTokenResponse> GenerateDisposableToken(DisposableTokenScope scope, ExpiresIn expiresIn)
    {
        _GenerateDisposableTokenRequest request = new _GenerateDisposableTokenRequest
        {
            // TODO: actually calculate the ValidForSeconds value
            Expires = new _GenerateDisposableTokenRequest.Types.Expires() { ValidForSeconds = 12345 },
            AuthToken = this.authToken,
            // TODO: implement me here
            Permissions = permissionsFromTokenScope(scope)
        };
        try
        {
            _logger.LogTraceExecutingAuthRequest(RequestTypeAuthGenerateDisposableToken);
            // TODO: I probably need to add deadline in call options here, yes?
            var response = await this.grpcManager.Client.generateDisposableToken(request, new CallOptions());
            return _logger.LogTraceAuthRequestSuccess(RequestTypeAuthGenerateDisposableToken,
                new GenerateDisposableTokenResponse.Success(response));
        }
        catch (Exception e)
        {
            return _logger.LogTraceAuthRequestError(RequestTypeAuthGenerateDisposableToken,
                new GenerateDisposableTokenResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    public void Dispose()
    {
        this.grpcManager.Dispose();
    }

}
