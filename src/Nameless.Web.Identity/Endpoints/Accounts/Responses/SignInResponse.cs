namespace Nameless.Web.Identity.Endpoints.Accounts.Responses;

public sealed record SignInResponse {
    public string? Redirect { get; init; }
}