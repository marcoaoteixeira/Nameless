using Nameless.Mediator.Requests;

namespace Nameless.Web.Identity.UseCases.SecurityTokens.RefreshTokens;

public record ValidateRefreshTokenRequest : IRequest<ValidateRefreshTokenResponse> {
    public required string Token { get; init; }
}