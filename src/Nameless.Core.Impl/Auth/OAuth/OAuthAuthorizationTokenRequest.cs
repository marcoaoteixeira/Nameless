using System.Text.Json.Serialization;

namespace Nameless.Auth.OAuth;

/// <summary>
///     Represents an OAuth authorization token request.
/// </summary>
public sealed record OAuthAuthorizationTokenRequest : AuthorizationTokenRequest {
    /// <summary>
    ///     Gets the client ID.
    /// </summary>
    [JsonPropertyName("client_id")]
    public required string ClientId { get; init; }

    /// <summary>
    ///     Gets the client secret.
    /// </summary>
    [JsonPropertyName("client_secret")]
    public required string ClientSecret { get; init; }

    /// <summary>
    ///     Gets the grant type.
    /// </summary>
    [JsonPropertyName("grant_type")]
    public string? GrantType { get; init; }

    /// <summary>
    ///     Gets the audience.
    /// </summary>
    [JsonPropertyName("audience")]
    public string? Audience { get; init; }
}