using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Nameless.ObjectModel;

namespace Nameless.Auth;

/// <summary>
///     Represents a base HTTP client authorization token provider.
/// </summary>
/// <typeparam name="TRequest">
///     The authorization token request type.
/// </typeparam>
/// <typeparam name="TToken">
///     The authorization token type.
/// </typeparam>
public abstract class AuthorizationTokenProvider<TRequest, TToken> : IAuthorizationTokenProvider<TRequest, TToken>
    where TRequest : AuthorizationTokenRequest
    where TToken : notnull {

    private readonly HttpClient _client;
    private readonly ILogger<AuthorizationTokenProvider<TRequest, TToken>> _logger;

    /// <summary>
    ///     Gets the provider token endpoint.
    /// </summary>
    protected abstract string TokenEndpoint { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="AuthorizationTokenProvider{TRequest,TToken}"/> class.
    /// </summary>
    /// <param name="client">
    ///     The HTTP client.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    protected AuthorizationTokenProvider(HttpClient client, ILogger<AuthorizationTokenProvider<TRequest, TToken>> logger) {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    ///     Retrieves an authorization token.
    /// </summary>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous
    ///     method execution. The result of the task contains the response.
    /// </returns>
    public async Task<AuthorizationTokenResponse<TToken>> GetTokenAsync(TRequest request, CancellationToken cancellationToken) {
        Throws.When.Null(request);

        HttpResponseMessage response;

        try {
            var content = CreateHttpContent(request);

            response = await _client.PostAsync(
                TokenEndpoint,
                content,
                cancellationToken
            ).SkipContextSync();

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex) {
            _logger.Failure(ex);

            return Error.Failure(ex.Message);
        }

        var result = await DeserializeTokenAsync(response, cancellationToken).SkipContextSync();

        return result is not null
            ? result
            : Error.Failure($"Unable to deserialize '{typeof(TToken).Name}' token.");
    }

    /// <summary>
    ///     Creates an HTTP content given the request.
    /// </summary>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="HttpContent"/>.
    /// </returns>
    protected abstract HttpContent CreateHttpContent(TRequest request);

    /// <summary>
    ///     Deserializes the incoming response to the authorization token.
    /// </summary>
    /// <param name="response">
    ///     The response.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous
    ///     method execution. The result of the task contains the token.
    /// </returns>
    protected virtual Task<TToken?> DeserializeTokenAsync(HttpResponseMessage response, CancellationToken cancellationToken) {
        return response.Content.ReadFromJsonAsync<TToken>(cancellationToken);
    }
}