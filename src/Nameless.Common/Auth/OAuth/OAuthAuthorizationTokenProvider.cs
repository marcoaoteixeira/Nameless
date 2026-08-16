using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Auth.OAuth;

/// <summary>
///     OAuth implementation of <see cref="IOAuthAuthorizationTokenProvider"/>
/// </summary>
public class OAuthAuthorizationTokenProvider : IOAuthAuthorizationTokenProvider {
    private const string LOG_TAG = "OAUTH_AUTHORIZATION_TOKEN_PROVIDER";

    private readonly HttpClient _client;
    private readonly OAuthOptions _options;
    private readonly ILogger<OAuthAuthorizationTokenProvider> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="OAuthAuthorizationTokenProvider"/> class.
    /// </summary>
    /// <param name="client">
    ///     The HTTP client.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public OAuthAuthorizationTokenProvider(HttpClient client, IOptions<OAuthOptions> options, ILogger<OAuthAuthorizationTokenProvider> logger) {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<OAuthAuthorizationTokenResponse> GetTokenAsync(OAuthAuthorizationTokenRequest request, CancellationToken cancellationToken) {
        Throws.When.Null(request);

        try {
            var content = CreateHttpContent(request);
            var response = await _client.PostAsync(_options.TokenEndpoint, content, cancellationToken)
                .SkipContextSync();

            response.EnsureSuccessStatusCode();

            var result = await DeserializeTokenAsync(response, cancellationToken).SkipContextSync();

            return result.Match<OAuthAuthorizationTokenResponse>(
                onSuccess: value => value,
                onFailure: failure => failure
            );
        }
        catch (Exception ex) {
            CommonLog.Failure(_logger, ex, tag: LOG_TAG);

            return Error.Failure(ex.Message);
        }
    }

    private static FormUrlEncodedContent CreateHttpContent(OAuthAuthorizationTokenRequest request) {
        var properties = request.GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .ToDictionary(
                                    prop => prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? prop.Name,
                                    prop => prop.GetValue(request)?.ToString() ?? string.Empty
                                );

        return new FormUrlEncodedContent(properties);
    }

    private async Task<Result<OAuthAuthorizationToken>> DeserializeTokenAsync(HttpResponseMessage response, CancellationToken cancellationToken) {
        try {
            var result = await response.Content
                .ReadFromJsonAsync<OAuthAuthorizationToken>(cancellationToken)
                .SkipContextSync();

            if (result is not null) { return result;}

            const string Reason = $"Unable to deserialize response content JSON to '{nameof(OAuthAuthorizationToken)}'";

            CommonLog.Warning(_logger, Reason, tag: LOG_TAG);

            return Error.Conflict(Reason);
        }
        catch (Exception ex) {
            CommonLog.Failure(_logger, ex, tag: LOG_TAG);

            return Error.Failure(ex.Message);
        }
    }
}
