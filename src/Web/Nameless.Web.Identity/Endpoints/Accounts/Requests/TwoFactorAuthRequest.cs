using Nameless.Validation;

namespace Nameless.Web.Identity.Endpoints.Accounts.Requests;

[Validate]
public sealed record TwoFactorAuthRequest {
    public string TwoFactorCode { get; init; } = string.Empty;
    
    public bool RememberMe { get; init; }

    public bool RememberMachine { get; init; }

    public string? ReturnUrl { get; init; }
}
