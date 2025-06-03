namespace Nameless.Web.Identity.Endpoints.Accounts.Responses;

public sealed record ConfirmEmailResponse {
    public string? Message { get; init; }
}