namespace Nameless.Web.Identity.Endpoints.v1.Auth.SignIn;

/// <summary>
///     Represents a sign-in request output.
/// </summary>
public record SignInOutput {
    /// <summary>
    ///     Gets or init the access token.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    ///     Gets or init the refresh token, if exists.
    /// </summary>
    public string? RefreshToken { get; init; }
}