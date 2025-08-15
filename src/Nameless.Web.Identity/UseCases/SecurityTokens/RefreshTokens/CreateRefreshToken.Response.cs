using Nameless.Web.Identity.Objects;

namespace Nameless.Web.Identity.UseCases.SecurityTokens.RefreshTokens;

public record CreateRefreshTokenResponse : Response {
    public string? Token { get; init; }
}