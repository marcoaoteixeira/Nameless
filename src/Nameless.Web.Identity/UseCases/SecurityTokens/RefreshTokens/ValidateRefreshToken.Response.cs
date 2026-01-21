using Nameless.Web.Identity.Domains.UserRefreshTokens;
using Nameless.Web.Identity.Objects;

namespace Nameless.Web.Identity.UseCases.SecurityTokens.RefreshTokens;

public record ValidateRefreshTokenResponse : Response {
    public override bool Success => Error is null &&
                                    Status.HasFlag(UserRefreshTokenStatus.Active);

    public UserRefreshTokenStatus Status { get; init; } = UserRefreshTokenStatus.Unknown;

    public Guid UserID { get; init; }
}