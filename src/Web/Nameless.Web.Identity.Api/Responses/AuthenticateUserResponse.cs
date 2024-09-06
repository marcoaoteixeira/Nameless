namespace Nameless.Web.Identity.Api.Responses;

public sealed record AuthenticateUserResponse : ResponseBase {
    #region Public Static Read-Only Properties

    public static AuthenticateUserResponse InvalidCredentials { get; } = new() { Error = "Username or Password invalid." };
    public static AuthenticateUserResponse UserNotAllowed { get; } = new() { Error = "User not allowed." };
    public static AuthenticateUserResponse UserLockedOut { get; } = new() { Error = "User locked out." };
    public static AuthenticateUserResponse UserRequiresTwoFactorAuth { get; } = new() { Error = "Requires Two-Factor Authentication." };
    public static AuthenticateUserResponse UserNotFound { get; } = new() { Error = "User not found." };

    #endregion

    #region Public Properties

    public string? Token { get; init; }

    #endregion
}