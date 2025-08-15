using Nameless.Mediator.Requests;

namespace Nameless.Web.Identity.UseCases.Authentication.SignIn;

public record SignInRequest : IRequest<SignInResponse> {
    public required string Email { get; init; }
    public required string Password { get; init; }
}