namespace Nameless.Web.Identity.Endpoints.v1.Auth.SignIn;

/// <summary>
///     Represents a sign-in request input.
/// </summary>
public record SignInInput {
    /// <summary>
    ///     Gets or init the user email.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///     Gets or init the password.
    /// </summary>
    public required string Password { get; init; }
}