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
    ///     Gets the token type.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string? TokenType { get; init; }

    /// <summary>
    ///     Gets when the token expires (Unix epoch in milliseconds).
    /// </summary>
    [JsonPropertyName("expires_in")]
    public long ExpiresIn { get; init; }
}