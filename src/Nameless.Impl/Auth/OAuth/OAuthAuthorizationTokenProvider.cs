using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Nameless.Auth.OAuth;

public class OAuthAuthorizationTokenProvider : AuthorizationTokenProvider<OAuthAuthorizationTokenRequest, OAuthAuthorizationToken> {
    public OAuthAuthorizationTokenProvider(HttpClient client, ILogger<OAuthAuthorizationTokenProvider> logger)
        : base(client, logger) { }

    protected override string TokenEndpoint => "/connect/token";

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
