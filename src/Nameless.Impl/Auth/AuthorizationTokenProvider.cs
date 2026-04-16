using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Nameless.ObjectModel;

namespace Nameless.Auth;

public abstract class AuthorizationTokenProvider<TRequest, TToken> : IAuthorizationTokenProvider<TRequest, TToken>
    where TRequest : AuthorizationTokenRequest
    where TToken : notnull {

    private readonly HttpClient _client;
    private readonly ILogger<AuthorizationTokenProvider<TRequest, TToken>> _logger;

    protected abstract string TokenEndpoint { get; }

    protected AuthorizationTokenProvider(HttpClient client, ILogger<AuthorizationTokenProvider<TRequest, TToken>> logger) {
        _client = client;
        _logger = logger;
    }

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

    protected abstract HttpContent CreateHttpContent(TRequest request);

    protected virtual Task<TToken?> DeserializeTokenAsync(HttpResponseMessage response, CancellationToken cancellationToken) {
        return response.Content.ReadFromJsonAsync<TToken>(cancellationToken);
    }
}