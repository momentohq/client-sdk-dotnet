using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Config;
using Momento.Sdk.Exceptions;
using Momento.Sdk.Internal.ExtensionMethods;
using Momento.Sdk.Responses;
using Momento.Protos.TokenClient;
using Momento.Protos.PermissionMessages;

namespace Momento.Sdk.Internal;

internal sealed class ScsTokenClient : IDisposable
{
    private readonly AuthGrpcManager grpcManager;
    private readonly string authToken;
    private readonly TimeSpan authClientOperationTimeout;
    private readonly ILogger _logger;
    private readonly CacheExceptionMapper _exceptionMapper;
    public ScsTokenClient(IAuthConfiguration config, string authToken, string endpoint)
    {
        this.grpcManager = new AuthGrpcManager(config, authToken, endpoint);
        this.authToken = authToken;
        this.authClientOperationTimeout = config.TransportStrategy.GrpcConfig.Deadline;
        this._logger = config.LoggerFactory.CreateLogger<ScsTokenClient>();
        this._exceptionMapper = new CacheExceptionMapper(config.LoggerFactory);
    }

    private DateTime CalculateDeadline()
    {
        return DateTime.UtcNow.Add(authClientOperationTimeout);
    }

    private const string RequestTypeAuthGenerateDisposableToken = "GENERATE_DISPOSABLE_TOKEN";

    public async Task<GenerateDisposableTokenResponse> GenerateDisposableToken(
        DisposableTokenScope scope, ExpiresIn expiresIn
    ) {
        Permissions permissions;
        try
        {
            permissions = PermissionsFromDisposableTokenScope(scope);
        }
        catch (ArgumentNullException e)
        {
            return _logger.LogTraceGenericRequestError(RequestTypeAuthGenerateDisposableToken,
                new GenerateDisposableTokenResponse.Error(
                    new InvalidArgumentException("Permissions parameters may not be null", null, e)
                )
            );
        }

        try
        {
            _GenerateDisposableTokenRequest request = new _GenerateDisposableTokenRequest
            {
                Expires = new _GenerateDisposableTokenRequest.Types.Expires() { ValidForSeconds = (uint)expiresIn.Seconds() },
                AuthToken = this.authToken,
                Permissions = permissions
            };
            _logger.LogTraceExecutingGenericRequest(RequestTypeAuthGenerateDisposableToken);
            var response = await grpcManager.Client.generateDisposableToken(
                request, new CallOptions(deadline: CalculateDeadline())
            );
            return _logger.LogTraceGenericRequestSuccess(RequestTypeAuthGenerateDisposableToken,
                new GenerateDisposableTokenResponse.Success(response));
        }
        catch (Exception e)
        {
            return _logger.LogTraceGenericRequestError(RequestTypeAuthGenerateDisposableToken,
                new GenerateDisposableTokenResponse.Error(_exceptionMapper.Convert(e)));
        }
    }

    private Permissions PermissionsFromDisposableTokenScope(DisposableTokenScope scope) {
        Permissions result = new();
        ExplicitPermissions explicitPermissions = new();
        foreach (DisposableTokenPermission perm in scope.Permissions) {
            var grpcPerm = DisposableTokenPermissionToGrpcPermission(perm);
            explicitPermissions.Permissions.Add(grpcPerm);
        }
        result.Explicit = explicitPermissions;
        return result;
    }

    private PermissionsType DisposableTokenPermissionToGrpcPermission(DisposableTokenPermission permission)
    {
        var result = new PermissionsType();
        // This covers CachePermission as well as CacheItemPermission, as the latter is a subclass
        // of the former and `DisposableCachePermissionToGrpcPermission` handles both.
        if (permission is DisposableToken.CachePermission cachePermission) {
            result.CachePermissions = DisposableCachePermissionToGrpcPermission(cachePermission);
        }
        else if (permission is DisposableToken.TopicPermission topicPermission) {
            result.TopicPermissions = TopicPermissionToGrpcPermission(topicPermission);
        }
        return result;
    }

    private PermissionsType.Types.CachePermissions AssignCacheSelector(
        DisposableToken.CachePermission permission, PermissionsType.Types.CachePermissions grpcPermission
    )
    {
        if (permission.CacheSelector is CacheSelector.SelectAllCaches)
        {
            grpcPermission.AllCaches = new PermissionsType.Types.All();
        }
        else if (permission.CacheSelector is CacheSelector.SelectByCacheName byName)
        {
            grpcPermission.CacheSelector = new PermissionsType.Types.CacheSelector
            {
                CacheName = byName.CacheName
            };
        }
        return grpcPermission;
    }

    private PermissionsType.Types.CachePermissions AssignCacheItemSelector(
        DisposableToken.CacheItemPermission permission, PermissionsType.Types.CachePermissions grpcPermission
    )
    {
        if (permission.CacheItemSelector is CacheItemSelector.SelectAllCacheItems)
        {
            grpcPermission.AllItems = new PermissionsType.Types.All();
        }
        else if (permission.CacheItemSelector is CacheItemSelector.SelectByKey byKey)
        {
            grpcPermission.ItemSelector = new PermissionsType.Types.CacheItemSelector
            {
                Key = ToByteStringExtensions.ToByteString(byKey.CacheKey)
            };
        }
        else if (permission.CacheItemSelector is CacheItemSelector.SelectByKeyPrefix byPrefix)
        {
            grpcPermission.ItemSelector = new PermissionsType.Types.CacheItemSelector
            {
                KeyPrefix = ToByteStringExtensions.ToByteString(byPrefix.CacheKeyPrefix)
            };
        }
        else
        {
            throw new UnknownException(
                "Unrecognized cache item specification in cache permission: " + Newtonsoft.Json.JsonConvert.SerializeObject(permission)
            );
        }
        return grpcPermission;
    }

    private PermissionsType.Types.CachePermissions AssignCacheRole(
        DisposableToken.CachePermission permission, PermissionsType.Types.CachePermissions grpcPermission
    )
    {
        switch(permission.Role)
        {
            case Auth.AccessControl.CacheRole.ReadWrite:
                grpcPermission.Role = Protos.PermissionMessages.CacheRole.CacheReadWrite;
                break;
            case Auth.AccessControl.CacheRole.ReadOnly:
                grpcPermission.Role = Protos.PermissionMessages.CacheRole.CacheReadOnly;
                break;
            case Auth.AccessControl.CacheRole.WriteOnly:
                grpcPermission.Role = Protos.PermissionMessages.CacheRole.CacheWriteOnly;
                break;
            default:
                throw new UnknownException(
                    "Unrecognized cache role: " + Newtonsoft.Json.JsonConvert.SerializeObject(permission)
                );
        }
        return grpcPermission;
    }

    private PermissionsType.Types.TopicPermissions TopicPermissionToGrpcPermission(
        DisposableToken.TopicPermission permission
    )
    {
        var grpcPermission = new PermissionsType.Types.TopicPermissions();
        switch (permission.Role) {
            case Auth.AccessControl.TopicRole.PublishSubscribe:
                grpcPermission.Role = Protos.PermissionMessages.TopicRole.TopicReadWrite;
                break;
            case Auth.AccessControl.TopicRole.SubscribeOnly:
                grpcPermission.Role = Protos.PermissionMessages.TopicRole.TopicReadOnly;
                break;
            case Auth.AccessControl.TopicRole.PublishOnly:
                grpcPermission.Role = Protos.PermissionMessages.TopicRole.TopicWriteOnly;
                break;
            default:
                throw new UnknownException(
                    "Unrecognized topic role: " + Newtonsoft.Json.JsonConvert.SerializeObject(permission)
                );
        }

        if (permission.CacheSelector is CacheSelector.SelectAllCaches)
        {
            grpcPermission.AllCaches = new PermissionsType.Types.All();
        }
        else if (permission.CacheSelector is CacheSelector.SelectByCacheName byName)
        {
            grpcPermission.CacheSelector = new PermissionsType.Types.CacheSelector
            {
                CacheName = byName.CacheName
            };
        }
        else
        {
            throw new UnknownException(
                "Unrecognized cache specification in topiuc permission: " + Newtonsoft.Json.JsonConvert.SerializeObject(permission)
            );
        }

        if (permission.TopicSelector is TopicSelector.SelectAllTopics)
        {
            grpcPermission.AllTopics = new PermissionsType.Types.All();
        }
        else if (permission.TopicSelector is TopicSelector.SelectByTopicName byName)
        {
            grpcPermission.TopicSelector = new PermissionsType.Types.TopicSelector
            {
                TopicName = byName.TopicName
            };
        }
        else if (permission.TopicSelector is TopicSelector.SelectByTopicNamePrefix byTopicNamePrefix)
        {
            grpcPermission.TopicSelector = new PermissionsType.Types.TopicSelector
            {
                TopicNamePrefix = byTopicNamePrefix.TopicNamePrefix
            };
        }
        else
        {
            throw new UnknownException(
                "Unrecognized topic specification in topic permission: " + Newtonsoft.Json.JsonConvert.SerializeObject(permission)
            );
        }

        return grpcPermission;
    }

    private PermissionsType.Types.CachePermissions DisposableCachePermissionToGrpcPermission(DisposableToken.CachePermission permission)
    {
        var grpcPermission = new PermissionsType.Types.CachePermissions();
        grpcPermission = AssignCacheRole(permission, grpcPermission);
        grpcPermission = AssignCacheSelector(permission, grpcPermission);
        if (permission is DisposableToken.CacheItemPermission itemPerm) {
            grpcPermission = AssignCacheItemSelector(itemPerm, grpcPermission);
        }
        return grpcPermission;
    }

    public void Dispose()
    {
        this.grpcManager.Dispose();
    }

}
