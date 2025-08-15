using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nameless.Mediator;
using Nameless.Validation;
using Nameless.Web.Endpoints;
using Nameless.Web.Endpoints.Definitions;
using Nameless.Web.Identity.UseCases.Authentication.SignIn;

using static Microsoft.AspNetCore.Http.TypedResults;

namespace Nameless.Web.Identity.Endpoints.v1.Auth.SignIn;

/// <summary>
///     The Sign-In endpoint.
/// </summary>
public class SignInEndpoint : IEndpoint {
    private readonly IMediator _mediator;
    private readonly IValidationService _validationService;

    public SignInEndpoint(
        IMediator mediator,
        IValidationService validationService) {
        _mediator = Guard.Against.Null(mediator);
        _validationService = Guard.Against.Null(validationService);
    }

    /// <inheritdoc />
    public IEndpointDescriptor Describe() {
        var builder = EndpointDescriptorBuilder.Create<SignInEndpoint>();

        builder
            .Post("signin", nameof(ExecuteAsync))
            .AllowAnonymous()
            .WithDescription("Authenticates a user using the provided information.")
            .WithDisplayName("SignIn")
            .WithGroupName("auth")
            .Produces<SignInOutput>()
            .ProducesProblem(statusCode: StatusCodes.Status401Unauthorized)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem();

        return builder.Build();
    }

    public async Task<IResult> ExecuteAsync([FromBody] SignInInput input, CancellationToken cancellationToken) {
        Guard.Against.Null(input);

        var validationResult = await _validationService.ValidateAsync(input, cancellationToken);
        if (!validationResult.Succeeded) {
            return ValidationProblem(validationResult.ToDictionary());
        }

        var request = new SignInRequest {
            Email = input.Email,
            Password = input.Password
        };
        var signInResponse = await _mediator.ExecuteAsync(request, cancellationToken);

        return signInResponse.Succeeded
            ? Ok(signInResponse)
            : Unauthorized();
    }
}
