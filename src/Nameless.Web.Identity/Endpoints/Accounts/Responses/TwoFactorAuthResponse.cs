namespace Nameless.Web.Identity.Endpoints.Accounts.Responses;

public sealed record TwoFactorAuthResponse {
    public string? Redirect { get; init; }
}