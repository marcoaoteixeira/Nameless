using Nameless.Validation;

namespace Nameless.Web.Identity.Endpoints.Accounts.Requests;

[Validate]
public sealed record ConfirmEmailRequest {
    public string Code { get; init; } = string.Empty;

    public string UserId { get; init; } = string.Empty;
}
