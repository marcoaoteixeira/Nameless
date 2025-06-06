namespace Nameless.Web.Identity.Endpoints.Accounts.Responses;

public sealed record RequiresTwoFactorResponse {
    public string Redirect { get; init; } = string.Empty;

    public bool RememberMe { get; init; }

    public string? ReturnUrl { get; init; }
}