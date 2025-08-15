using Nameless.Web.Identity.Objects;

namespace Nameless.Web.Identity.UseCases.SecurityTokens.AccessToken;

public record CreateAccessTokenResponse : Response {
    public string? Token { get; init; }
}