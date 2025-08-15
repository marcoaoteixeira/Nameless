using Nameless.Mediator.Requests;

namespace Nameless.Web.Identity.UseCases.SecurityTokens.RefreshTokens;

public record CreateRefreshTokenRequest : IRequest<CreateRefreshTokenResponse> {
    public required Guid UserID { get; init; }
}