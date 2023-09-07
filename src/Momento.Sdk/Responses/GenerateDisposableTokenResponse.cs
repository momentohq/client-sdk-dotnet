using Momento.Sdk.Auth.AccessControl;
using Momento.Sdk.Exceptions;
using Momento.Protos.TokenClient;

namespace Momento.Sdk.Responses;

/// <summary>
/// Parent response type for a generate disposable token request. The
/// response object is resolved to a type-safe object of one of
/// the following subtypes:
/// <list type="bullet">
/// <item><description>GenerateDisposableTokenResponse.Success</description></item>
/// <item><description>GenerateDisposableTokenResponse.Error</description></item>
/// </list>
/// Pattern matching can be used to operate on the appropriate subtype.
/// For example:
/// <code>
/// if (response is GenerateDisposableTokenResponse.Success successResponse)
/// {
///     // handle success if needed
/// }
/// else if (response is GenerateDisposableTokenResponse.Error errorResponse)
/// {
///     // handle error as appropriate
/// }
/// else
/// {
///     // handle unexpected response
/// }
/// </code>
/// </summary>
public abstract class GenerateDisposableTokenResponse
{
    /// <include file="../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : GenerateDisposableTokenResponse {
        public readonly string authToken;
        public readonly string endpoint;
        public readonly ExpiresAt expiresAt;

        public Success(_GenerateDisposableTokenResponse response)
        {
            // TODO: make sure this works (keys are right, etc.)
            authToken = Newtonsoft.Json.JsonConvert.SerializeObject(new {endpoint=response.Endpoint, api_key=response.ApiKey});
            endpoint = response.Endpoint;
            // TODO: not quite sure about this cast. I may have the method support ulong instead (or in addition).
            expiresAt = ExpiresAt.FromEpoch((int)response.ValidUntil);
        }
    }

    /// <include file="../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : GenerateDisposableTokenResponse, IError
    {
        private readonly SdkException _error;

        /// <include file="../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
        public Error(SdkException error)
        {
            _error = error;
        }

        /// <inheritdoc />
        public SdkException InnerException
        {
            get => _error;
        }

        /// <inheritdoc />
        public MomentoErrorCode ErrorCode
        {
            get => _error.ErrorCode;
        }

        /// <inheritdoc />
        public string Message
        {
            get => $"{_error.MessageWrapper}: {_error.Message}";
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}: {this.Message}";
        }
    }
}