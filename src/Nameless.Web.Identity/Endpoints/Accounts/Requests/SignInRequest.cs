using Nameless.Validation;

namespace Nameless.Web.Identity.Endpoints.Accounts.Requests;

[Validate]
public sealed record SignInRequest {
    public string UserName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public bool RememberMe { get; init; }

    public string? ReturnUrl { get; init; }
}