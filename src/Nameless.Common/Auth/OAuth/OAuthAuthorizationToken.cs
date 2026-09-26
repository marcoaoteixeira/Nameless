using System.Text.Json.Serialization;

namespace Nameless.Auth.OAuth;

/// <summary>
///     Represents an OAuth authorization token.
/// </summary>
public sealed record OAuthAuthorizationToken {
    /// <summary>
    ///     Gets the access token.
    /// </summary>
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    /// <summary>
    ///     Gets the refresh token.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }
}