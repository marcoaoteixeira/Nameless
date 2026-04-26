using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Nameless.Auth.OAuth;

/// <summary>
///     OAuth implementation of <see cref="AuthorizationTokenProvider{TRequest,TToken}"/>
/// </summary>
public class OAuthAuthorizationTokenProvider : AuthorizationTokenProvider<OAuthAuthorizationTokenRequest, OAuthAuthorizationToken> {
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="OAuthAuthorizationTokenProvider"/> class.
    /// </summary>
    /// <param name="client">
    ///     The HTTP client.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public OAuthAuthorizationTokenProvider(HttpClient client, ILogger<OAuthAuthorizationTokenProvider> logger)
        : base(client, logger) { }

    /// <inheridoc />
    protected override string TokenEndpoint => "/connect/token";

    /// <inheridoc />
    protected override HttpContent CreateHttpContent(OAuthAuthorizationTokenRequest request) {
        var properties = request.GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .ToDictionary(
                                    prop => prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? prop.Name,
                                    prop => prop.GetValue(request)?.ToString() ?? string.Empty
                                );

        return new FormUrlEncodedContent(properties);
    }
}
