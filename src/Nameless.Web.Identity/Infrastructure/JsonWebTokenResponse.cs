namespace Nameless.Web.Identity.Infrastructure;

/// <summary>
///     Represents the response containing JSON Web Token (JWT) and
///     the optional refresh token.
/// </summary>
public record JsonWebTokenResponse {
    /// <summary>
    ///     Gets or sets the access token (JWT).
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    ///     Gets or sets the refresh token, which is optional.
    /// </summary>
    public string? RefreshToken { get; set; }
}