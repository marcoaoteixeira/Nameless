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
    private static string Tag { get; } = nameof(OAuthAuthorizationTokenProvider).ToSnakeCase().ToUpperInvariant();

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

        try { return await GetTokenAsyncCore(request, cancellationToken).SkipContextSync(); }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex.Message, ex, Tag);

            return Error.Failure(ex.Message);
        }
    }

    private async Task<OAuthAuthorizationTokenResponse> GetTokenAsyncCore(OAuthAuthorizationTokenRequest request, CancellationToken cancellationToken) {
        var content = CreateHttpContent(request);
        var response = await RequestTokenAsync(content, cancellationToken).SkipContextSync();
        var result = await DeserializeTokenAsync(response, cancellationToken).SkipContextSync();

        return result.Match<OAuthAuthorizationTokenResponse>(
            onSuccess: value => value,
            onFailure: failure => failure
        );
    }

    private async Task<HttpResponseMessage> RequestTokenAsync(FormUrlEncodedContent content, CancellationToken cancellationToken) {
        using var meter = DiagnosticsHelper.CreateStopwatchHistogram(Metrics.RequestTokenDuration);

        var response = await _client.PostAsync(_options.TokenEndpoint, content, cancellationToken)
                                    .SkipContextSync();

        response.EnsureSuccessStatusCode();

        return response;
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
        var result = await response.Content
                                   .ReadFromJsonAsync<OAuthAuthorizationToken>(cancellationToken)
                                   .SkipContextSync();

        if (result is not null) { return result; }

        Log.DeserializationFailure(_logger, type: nameof(OAuthAuthorizationToken), Tag);

        return Error.Failure(
            $"Unable to deserialize JSON response content to '{nameof(OAuthAuthorizationToken)}'"
        );
    }

    internal static class Metrics {
        private const string ROOT = "nameless.auth.token.provider";

        internal const string RequestTokenDuration = $"{ROOT}.request.duration";
    }
}
