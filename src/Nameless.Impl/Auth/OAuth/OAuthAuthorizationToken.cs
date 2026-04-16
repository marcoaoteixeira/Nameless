using System.Text.Json.Serialization;

namespace Nameless.Auth.OAuth;

public sealed record OAuthAuthorizationToken {
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; init; }

    [JsonPropertyName("expires_in")]
    public long ExpiresIn { get; init; }
}