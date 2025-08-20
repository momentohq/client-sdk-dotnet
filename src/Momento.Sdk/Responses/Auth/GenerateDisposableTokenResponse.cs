using Momento.Protos.TokenClient;
using Momento.Sdk.Exceptions;
using System;

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
///     var authToken = successResponse.AuthToken;
///     var endpoint = successResponse.Endpoint;
///     var expiryEpochTime = successResponse.ExpiresAt.Epoch();
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
    /// <include file="../../docs.xml" path='docs/class[@name="Success"]/description/*' />
    public class Success : GenerateDisposableTokenResponse
    {
        /// <summary>
        /// The generated auth token.
        /// </summary>
        public readonly string AuthToken;
        /// <summary>
        /// The endpoint to use for subsequent requests.
        /// </summary>
        public readonly string Endpoint;
        /// <summary>
        /// The expiry time of the token.
        /// </summary>
        public readonly ExpiresAt ExpiresAt;

        /// <summary>
        /// Indicates a disposable token has been generated and exposes the attributes of the token:
        /// <list type="bullet">
        /// <item><description>AuthToken</description></item>
        /// <item><description>Endpoint</description></item>
        /// <item><description>ExpiresAt</description> (use its `Epoch()` method to retrieve the expiry timestamp as Unix epoch) </item>
        /// </list>
        /// </summary>
        /// <param name="response"></param>
        public Success(_GenerateDisposableTokenResponse response)
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(new { endpoint = response.Endpoint, api_key = response.ApiKey });
            var jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            AuthToken = System.Convert.ToBase64String(jsonBytes);
            Endpoint = response.Endpoint;
            ExpiresAt = ExpiresAt.FromEpoch((int)response.ValidUntil);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{AuthToken.Substring(0, 10)}...{AuthToken.Substring((AuthToken.Length - 10), 10)}";
        }
    }

    /// <include file="../../docs.xml" path='docs/class[@name="Error"]/description/*' />
    public class Error : GenerateDisposableTokenResponse, IError
    {
        private readonly SdkException _error;

        /// <include file="../../docs.xml" path='docs/class[@name="Error"]/constructor/*' />
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
