using Nameless.Web.Identity.Security;

namespace Nameless.Web.Identity;

public record WebIdentityOptions {
    public bool LockoutOnFailure { get; init; } = false;

    public Action<RefreshTokenOptions>? ConfigureRefreshToken { get; } = null;
}