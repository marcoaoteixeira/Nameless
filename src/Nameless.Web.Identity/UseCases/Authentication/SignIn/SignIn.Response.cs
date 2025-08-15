using Nameless.Web.Identity.Objects;

namespace Nameless.Web.Identity.UseCases.Authentication.SignIn;

public record SignInResponse : Response {
    public string? AccessToken { get; init; }

    public string? RefreshToken { get; init; }

    public SignInErrorType ErrorType { get; init; }
}