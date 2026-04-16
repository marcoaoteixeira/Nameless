using System.Text.Json.Serialization;

namespace Nameless.Auth.OAuth;

public sealed record OAuthAuthorizationTokenRequest : AuthorizationTokenRequest {
    [JsonPropertyName("client_id")]
    public required string ClientId { get; init; }

    [JsonPropertyName("client_secret")]
    public required string ClientSecret { get; init; }

    [JsonPropertyName("grant_type")]
    public string? GrantType { get; init; }

    [JsonPropertyName("audience")]
    public string? Audience { get; init; }
}