using Nameless.Mediator.Requests;

namespace Nameless.Web.Identity.UseCases.SecurityTokens.AccessToken;

public record CreateAccessTokenRequest : IRequest<CreateAccessTokenResponse> {
    public required Guid UserID { get; init; }
}